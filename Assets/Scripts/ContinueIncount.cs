using Krivodeling.UI.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ContinueIncount : MonoBehaviour
{
    public ChoiseData cerChoiseData = null;
    public ChoiseData[] choise1 = null;

    public bool isChoisOn = false;
    public int lengthCount = 0;

    private IncountManager incountManager;
    private UIBlur uiBlur = null;

    private TextMeshProUGUI firstTMP;
    private TextMeshProUGUI secondTMP;

    private CanvasGroup firstTMPCanvasGroup;
    private CanvasGroup secondTMPCanvasGroup;

    private Button[] buttons = new Button[4];
    private TextMeshProUGUI[] btnTMPs = new TextMeshProUGUI[4];
    private CanvasGroup[] btnCanvasGroups = new CanvasGroup[4];

    Transform child0;    //Blur Image
    Transform child1;    //DIalogueGroup (1)

    float transitionDuration = 1.0f;

    //몇 번 선택지인지 알리는 변수
    [SerializeField]
    private int selectChoiseNum = -1;
    public int SelectChoiseNum
    {
        get => selectChoiseNum;
        set
        {
            selectChoiseNum = value;
            bool isCon = cerChoiseData.isContinues[selectChoiseNum] ? true : false;
            if (isCon)
            {
                // 선택지의 후 내용이 나옴
            }
            else
            {
                // 이전 선택지로 돌아감
                BackIncount();
            }
        }
    }

    private void Awake()
    {
        incountManager = GetComponentInParent<IncountManager>();
        incountManager.onContinue += TestFunc;
        uiBlur = GetComponentInChildren<UIBlur>();    
    }

    private void Start()
    {

        child1 = transform.GetChild(1);    //DIalogueGroup (1)
        Transform h_child = child1.GetChild(0);      //Center
        Transform g_child = h_child.GetChild(0);
        firstTMP = g_child.GetComponent<TextMeshProUGUI>();
        firstTMPCanvasGroup = g_child.GetComponent<CanvasGroup>();
        g_child = h_child.GetChild(1);
        secondTMP = g_child.GetComponent<TextMeshProUGUI>();
        secondTMPCanvasGroup = g_child.GetComponent<CanvasGroup>();

        g_child = child1.GetChild(1);                //Option
        btnTMPs = g_child.GetComponentsInChildren<TextMeshProUGUI>(true);
        buttons = g_child.GetComponentsInChildren<Button>(true);
        btnCanvasGroups = g_child.GetComponentsInChildren<CanvasGroup>(true);

        buttons[0].onClick.AddListener(() => OnOneOptionClick());
        buttons[1].onClick.AddListener(() => OnTwoOptionClick());
        buttons[2].onClick.AddListener(() => OnThreeOptionClick());
        buttons[3].onClick.AddListener(() => OnFourOptionClick());
        /*
        //시작 시 버튼 입력 제한 걸기
        foreach (var button in buttons)
        {
            button.interactable = false;
        }

        // 버튼 tmp 초기화
        foreach (var tmp in btnTMPs)
        {
            tmp.text = string.Empty;
        }
        */
        child1.gameObject.SetActive(false);
        child0 = transform.GetChild(0);
        child0.gameObject.SetActive(false);
    }

    public void TestFunc(int incountIndex, int choiseNum)
    {
        OnActive(true);             //자식 오브젝트 활성화 ---------후에 페이드인 처리----------

        uiBlur.BeginBlur(2.0f);     //블러 시작 처리

        switch (incountIndex)
        {
            case 1:
                ChoiseTMPUpdate(choise1[0]);
                cerChoiseData = choise1[0];
                break;
            case 2:
                break;
            case 3:
                break;  

        }


        //uiBlur.EndBlur(2.0f);       //블러 종료 처리
    }

    private void OnActive(bool isOn)
    {
        child0.gameObject.SetActive(isOn); 
        child1.gameObject.SetActive(isOn); 

    }

    private void BackIncount()
    {
        OnActive(false);
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


    private void ChoiseTMPUpdate(ChoiseData selectChoise)
    {
        firstTMP.text = selectChoise.msg1;
        secondTMP.text = selectChoise.msg2;

        StartCoroutine(FadeInCo(firstTMPCanvasGroup, true));
        
        StartCoroutine(FadeInCo(secondTMPCanvasGroup, true, 0.8f));

        ///버튼 활성화
        for (int i = 0; i < selectChoise.choiseCount; i++)
        {
            btnTMPs[i].text = selectChoise.choiseList[i];

            //필요한 버튼만 입력 활성화 및 보이게 하기
            StartCoroutine(FadeInCo(btnCanvasGroups[i], true, i * 0.2f));
            buttons[i].interactable = true;
        }
    }


    IEnumerator FadeInCo(CanvasGroup canvasGroup, bool fadeIn, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (timer < transitionDuration)
        {
            float currentValue = Mathf.Lerp(startAlpha, endAlpha, timer / transitionDuration);
            canvasGroup.alpha = currentValue;
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
