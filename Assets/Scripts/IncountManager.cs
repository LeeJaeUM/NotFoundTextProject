using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IncountManager : MonoBehaviour
{
    public IncountData[] incountDatas;
    public ChoiseData[] choiseDatas;
    public Button[] buttons = new Button[4];
    public TextMeshProUGUI[] btnTMPs = new TextMeshProUGUI[4];
    public CanvasGroup[] btnCanvasGroups = new CanvasGroup[4];

    public bool isChoise = false;           //선택지 진입 확인 bool 변수

    //몇 번 선택지인지 알리는 변수
    [SerializeField]
    private int selectChoiseNum = 0;
    public int SelectChoiseNum
    {
        get => selectChoiseNum;
        set
        {
            selectChoiseNum = value;
            isChoise = false;
            ButtonFadeOut();
        }
    }

    public float transitionDuration = 1f;   // 변화에 걸리는 시간
    public float currentValue = 0;          // 페이드 인/아웃에 쓰는 변수

    //현재 인카운트의 몇번째 텍스트인지 판단하는 숫자
    public int textIndex = 2;
    public int TextIndex
    {
        get => textIndex;
        set
        {
            textIndex = value;

            //현재 인카운트의 텍스트를 전부 표현한 뒤라면
            if (textIndex >= maxTextIndex)
            {
                textIndex = -1;
                maxTextIndex = incountDatas[incountIndex].incountSession.Count;
                incountIndex++;
            }
        }
    }

    public int maxTextIndex = 0;

    //텍스트를 두 개로 나누어서 표시하기 위한 카운트
    public int lengthCount = 0;
    public int maxLengthCount = 2;

    //현재 인카운트의 넘버
    public int incountIndex = 0;

    public TextMeshProUGUI firstTMP;
    public TextMeshProUGUI secondTMP;

    public CanvasGroup firstTMPCanvasGroup;
    public CanvasGroup secondTMPCanvasGroup;

    PlayerInputActions inputActions;
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        inputActions.Player.Click.performed -= OnClick;
        inputActions.Player.Disable();
    }
    private void Awake()
    {
        Transform child = transform.GetChild(0);
        child = child.GetChild(1);
        child = child.GetChild(2);
        btnTMPs = child.GetComponentsInChildren<TextMeshProUGUI>(true);
        buttons = child.GetComponentsInChildren<Button>(true);
        btnCanvasGroups = child.GetComponentsInChildren<CanvasGroup>(true);

        buttons[0].onClick.AddListener(() => OnOneOptionClick());
        buttons[1].onClick.AddListener(() => OnTwoOptionClick());
        buttons[2].onClick.AddListener(() => OnThreeOptionClick());
        buttons[3].onClick.AddListener(() => OnFourOptionClick());

        //시작 시 버튼 입력 제한 걸기
        foreach (var button in buttons)
        {
            button.interactable = false;
        }

        inputActions = new PlayerInputActions();
    }


    private void Start()
    {
        // 초기화: 텍스트 숨기기
        firstTMPCanvasGroup.alpha = 0f;
        secondTMPCanvasGroup.alpha = 0f;

        firstTMP.text = string.Empty;
        secondTMP.text = string.Empty;

        maxTextIndex = incountDatas[incountIndex].incountSession.Count;

    }


    private void OnClick(InputAction.CallbackContext context)
    {
        if(!isChoise)
            DefaultLog();
        else
        {

            Debug.Log(selectChoiseNum);
        }
    }

    private void DefaultLog()
    {
        //시작 체크 용도
        if (textIndex < 0)
        {
            StartCoroutine(FadeOutCo(firstTMPCanvasGroup, secondTMPCanvasGroup));
            TextIndex = 0;
            lengthCount = 0;
            return;
        }

        //선택지 확인 용도
        if (incountDatas[incountIndex].incountSession[textIndex].msgIndex != 0)
        {
            int choiseIndex = incountDatas[incountIndex].incountSession[textIndex].msgIndex;
            ChoiseTMPUpdate(choiseDatas[choiseIndex]);

            //선택지에 진입한 것을 확인하는
            isChoise = true;

            return;
        }

        ///중앙 텍스트 용도
        if (lengthCount == maxLengthCount)
        {
            lengthCount = 0;
            StartCoroutine(FadeOutCo(firstTMPCanvasGroup, secondTMPCanvasGroup));
        }
        else
        {
            if (lengthCount == 0)
            {
                StartCoroutine(FadeInCo(firstTMPCanvasGroup, true));
            }
            else if (lengthCount == 1)
            {
                StartCoroutine(FadeInCo(secondTMPCanvasGroup, true));
            }
            lengthCount++;
        }
    }


    IEnumerator FadeInCo(CanvasGroup canvasGroup, bool fadeIn, bool isChoise = false)
    {
        float timer = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        
        if (fadeIn && !isChoise)
        {
            if (TextIndex % 2 == 0)
                firstTMP.text = incountDatas[incountIndex].incountSession[textIndex].msg;
            else
            {
                if (TextIndex != 0)
                    secondTMP.text = incountDatas[incountIndex].incountSession[textIndex].msg;
            }
            TextIndex++;
        }

        while (timer < transitionDuration)
        {
            float currentValue = Mathf.Lerp(startAlpha, endAlpha, timer / transitionDuration);
            canvasGroup.alpha = currentValue;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOutCo(CanvasGroup canvasGroup1, CanvasGroup canvasGroup2)
    {
        float timer = 0f;
        float startAlpha =  1f;
        float endAlpha = 0f;

        while (timer < transitionDuration)
        {
            float currentValue = Mathf.Lerp(startAlpha, endAlpha, timer / transitionDuration);
            canvasGroup1.alpha = currentValue;
            canvasGroup2.alpha = currentValue;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    #region Buttons
    private void ChoiseTMPUpdate(ChoiseData selectChoise)
    {
        firstTMP.text = selectChoise.msg1;
        secondTMP.text = selectChoise.msg2;

        StartCoroutine(FadeInCo(firstTMPCanvasGroup, true, true));
        StartCoroutine(FadeInCo(secondTMPCanvasGroup, true, true));

        for (int i = 0; i < selectChoise.choiseCount; i++)
        {
            btnTMPs[i].text = selectChoise.choiseList[i];

            //필요한 버튼만 입력 활성화 및 보이게 하기
            StartCoroutine(FadeInCo(btnCanvasGroups[i], true, true));
            buttons[i].interactable = true;
        }
    }

    private void ButtonFadeOut()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;

            StartCoroutine(FadeInCo(btnCanvasGroups[i], false, true));
        }
        
    }

    public void OnOneOptionClick()
    {
        SelectChoiseNum = 1;
    }
    public void OnTwoOptionClick()
    {
        SelectChoiseNum = 2;
    }
    public void OnThreeOptionClick()
    {
        SelectChoiseNum = 3;
    }
    public void OnFourOptionClick()
    {
        SelectChoiseNum = 4;
    }
    #endregion


}
