using Krivodeling.UI.Effects;
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
    private Button[] buttons = new Button[4];
    private TextMeshProUGUI[] btnTMPs = new TextMeshProUGUI[4];
    private CanvasGroup[] btnCanvasGroups = new CanvasGroup[4];

    public bool isChoise = false;           //선택지 진입 확인 bool 변수
    public bool isCon = false;              // 연속 선택지인지 확인
    public bool[] isContinues = new bool[4];// 선택지가 연속 선택지인지 ChoiseData에서 받음
    public int choiseIndex = 0;

    public Action<int, int> onContinue;

    public Action<int> onSFXPlay;

    //몇 번 선택지인지 알리는 변수
    [SerializeField]
    private int selectChoiseNum = -1;
    public int SelectChoiseNum
    {
        get => selectChoiseNum;
        set
        {
            selectChoiseNum = value;
            isCon = isContinues[selectChoiseNum] ? true : false;
            if (isCon)
            {
                onContinue(choiseIndex, selectChoiseNum);
            }
            else
            {
                EndChoise();
            }
        }
    }

    public float transitionDuration = 0.5f;   // 변화에 걸리는 시간
    public float currentValue = 0;          // 페이드 인/아웃에 쓰는 변수

    public int curSfxIndex = 0;

    //현재 인카운트의 몇번째 텍스트인지 판단하는 숫자
    public int textIndex = 2;
    public int TextIndex
    {
        get => textIndex;
        set
        {
            textIndex = value;

            //음향 출력
            if(textIndex > 0 && textIndex < 98)
            {
                curSfxIndex = incountDatas[incountIndex].incountSession[textIndex - 1].audioIndex;
                onSFXPlay?.Invoke(curSfxIndex);
                //AudioManager.instance.PlaySfx((AudioManager.Sfx)curSfxIndex);
            }

            //현재 인카운트의 텍스트를 전부 표현한 뒤라면
            if (textIndex >= maxTextIndex)
            {
                if(maxTextIndex % 2 != 0)
                    secondTMP.text = string.Empty;  
                textIndex = -1;

                //다음 인카운트로 넘어감
                incountIndex++;
                maxTextIndex = incountDatas[incountIndex].incountSession.Count;
            }
        }
    }

    public int maxTextIndex = 0;

    //텍스트를 두 개로 나누어서 표시하기 위한 카운트
    public int lengthCount = 0;
    public int LengthCount
    {
        get => lengthCount;
        set
        {
            if(lengthCount != value)
            {
                lengthCount = value;
                if (lengthCount == 1)
                    onLengthCountPush?.Invoke(lengthCount, firstTMP.text);
                else if(lengthCount == 2)
                    onLengthCountPush?.Invoke(lengthCount, secondTMP.text);
            }
        }
    }
    public Action<int, string> onLengthCountPush;
    public Action onChoiseStart;

    public int maxLengthCount = 2;

    //현재 인카운트의 넘버
    public int incountIndex = 0;

    private TextMeshProUGUI firstTMP;
    private TextMeshProUGUI secondTMP;

    private CanvasGroup firstTMPCanvasGroup;
    private CanvasGroup secondTMPCanvasGroup;

    private ContinueIncount continueIncount;

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

    private void OnClick(InputAction.CallbackContext context)
    {
        if (!isChoise)
            DefaultLog();
        else
        {

            Debug.Log(selectChoiseNum);
        }
    }

    private void Awake()
    {
        //연속 선택지 델리게이트 연결
        continueIncount = GetComponentInChildren<ContinueIncount>();
        continueIncount.onEndChoise += EndChoise;

        // 컴포넌트 위치 찾기
        Transform child = transform.GetChild(0);
        child = child.GetChild(1);
        Transform centerC = child.GetChild(1);

        child = child.GetChild(2);
        btnTMPs = child.GetComponentsInChildren<TextMeshProUGUI>(true);
        buttons = child.GetComponentsInChildren<Button>(true);
        btnCanvasGroups = child.GetComponentsInChildren<CanvasGroup>(true);

        //리스너 연결
        buttons[0].onClick.AddListener(() => OnOneOptionClick());
        buttons[1].onClick.AddListener(() => OnTwoOptionClick());
        buttons[2].onClick.AddListener(() => OnThreeOptionClick());
        buttons[3].onClick.AddListener(() => OnFourOptionClick());

        //시작 시 버튼 제한, 초기화, 안 보이게 처리
        for (int i = 0; i < buttons.Length; i++)
        {
            btnCanvasGroups[i].alpha = 0;
            buttons[i].interactable = false;
            btnTMPs[i].text = string.Empty;
        }

        Transform c1 = centerC.GetChild(0);
        firstTMP = c1.GetComponent<TextMeshProUGUI>();
        firstTMPCanvasGroup = c1.GetComponent<CanvasGroup>();   

        Transform c2 = centerC.GetChild(1);
        secondTMP = c2.GetComponent<TextMeshProUGUI>();
        secondTMPCanvasGroup = c2.GetComponent<CanvasGroup>();

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


    private void DefaultLog()
    {
        //시작 체크 용도
        if (textIndex < 0)
        {
            StartCoroutine(FadeOutCo(firstTMPCanvasGroup, secondTMPCanvasGroup));
            TextIndex = 0;
            LengthCount = 0;
            return;
        }

        //선택지 확인 용도
        if (incountDatas[incountIndex].incountSession[textIndex].choiseIndex != 0)
        {
            onChoiseStart?.Invoke();
            int choiseIndex = incountDatas[incountIndex].incountSession[textIndex].choiseIndex;
            ChoiseTMPUpdate(choiseDatas[choiseIndex]);

            //선택지에 진입한 것을 확인하는
            isChoise = true;

            return;
        }

        ///중앙 텍스트 용도
        if (lengthCount == maxLengthCount)
        {
            LengthCount = 0;
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
            LengthCount++;
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
        firstTMP.text = string.Empty;
        secondTMP.text = string.Empty;
    }

    #region Buttons
    /// <summary>
    /// 선택지 진입 시 바뀔 내용 : 중앙텍스트, 버튼 활성화
    /// </summary>
    /// <param name="selectChoise"></param>
    private void ChoiseTMPUpdate(ChoiseData selectChoise)
    {
        Debug.Log($"{selectChoise.name}은 이 선택지의 이름, 선택지는 {selectChoise.choiseCount} 개");
        firstTMP.text = selectChoise.msg1;
        secondTMP.text = selectChoise.msg2;

        StartCoroutine(FadeInCo(firstTMPCanvasGroup, true, true));
        StartCoroutine(FadeInCo(secondTMPCanvasGroup, true, true));

        choiseIndex = selectChoise.continuousIndex;

        ///버튼 활성화
        for (int i = 0; i < selectChoise.choiseCount; i++)
        {
            Debug.Log($"i는 {i}");
            btnTMPs[i].text = selectChoise.choiseList[i];


            //필요한 버튼만 입력 활성화 및 보이게 하기
            StartCoroutine(FadeInCo(btnCanvasGroups[i], true, true));
            buttons[i].interactable = true;

            isContinues[i] = selectChoise.isContinues[i];
        }
    }

    /// <summary>
    /// 버튼 입력 후 안 보이게 처리하는 함수
    /// </summary>
    private void ButtonFadeOut()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            btnTMPs[i].text = string.Empty;     // 버튼 tmp 초기화
            StartCoroutine(FadeInCo(btnCanvasGroups[i], false, true));
        }

    }

    public void OnOneOptionClick()
    {
        SelectChoiseNum = 0;
    }
    public void OnTwoOptionClick()
    {
        SelectChoiseNum = 1;
    }
    public void OnThreeOptionClick()
    {
        SelectChoiseNum = 2;
    }
    public void OnFourOptionClick()
    {
        SelectChoiseNum = 3;
    }
    #endregion
/*
    public void ClickControll(bool isOn)
    {
        if(isOn)
            OnEnable();
        else
            OnDisable();
    }
*/

    private void EndChoise()
    {
        Debug.Log("인캉누트 종료");
        isChoise = false;           //선택지 종료
        isCon = false;
        isContinues = new bool[4];
        ButtonFadeOut();
        StartCoroutine(FadeOutCo(firstTMPCanvasGroup, secondTMPCanvasGroup));
        TextIndex = 99; //최대값을 넘겨서 다음으로 넘김
    }
}
