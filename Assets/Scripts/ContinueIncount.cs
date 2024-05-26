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
    [Header("현재 선택지 관련")]
    public ChoiseData cerChoiseData = null;
    public ChoiseData[] cerChoise = null;
    public int cerIncountIndex = 0;
    public int cerChoiseIndex = 0;

    [Header("선택지들")]
    public ChoiseData[] choise1 = null;

    [Header("변수")]    
    //몇 번 버튼을 눌렀는지 알리는 변수
    [SerializeField] private int selectChoiseNum = -1;

    // /// 마지막 선택지 없는 choiseData는 클릭후 다음 인카운트로 넘어간다고 알림
    public bool isChoisOn = true;

    private IncountManager incountManager;
    private UIBlur uiBlur = null;

    // --------------TMPe들 ----------------------
    private TextMeshProUGUI firstTMP;
    private TextMeshProUGUI secondTMP;

    private CanvasGroup firstTMPCanvasGroup;
    private CanvasGroup secondTMPCanvasGroup;

    private Button[] buttons = new Button[4];
    private TextMeshProUGUI[] btnTMPs = new TextMeshProUGUI[4];
    private CanvasGroup[] btnCanvasGroups = new CanvasGroup[4];

    private TextMeshProUGUI lastFirstTMP;
    private TextMeshProUGUI lastSecondTMP;

    private CanvasGroup lastFirstTMPCanvasGroup;
    private CanvasGroup lastSecondTMPCanvasGroup;

    // ---------위치------------------
    private CanvasGroup thisCanvasGroup;
    private RectTransform blurRect;

    Transform child0;    //Blur Image
    Transform child1;    //DIalogueGroup (1

    public Action onEndChoise;

    public int SelectChoiseNum
    {
        get => selectChoiseNum;
        set
        {
            selectChoiseNum = value;
            bool isCon = cerChoise[cerChoiseIndex].isContinues[selectChoiseNum] ? true : false;
            if (isCon)
            {
                cerChoiseIndex++;
                // 선택지의 후 내용이 나옴
                ChoiseTMPUpdate(cerChoise[cerChoiseIndex]);
            }
            else
            {
                // 이전 선택지로 돌아감
                OnActive(false);
            }
        }
    }

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

    /// <summary>
    /// 눌렀을 때 isChoisOn이 true라면 다음 인카운트로 넘어가는 액션 발동
    /// </summary>
    /// <param name="context"></param>
    private void OnClick(InputAction.CallbackContext context)
    {
        if (!isChoisOn)
        {
            EndIncount();
            onEndChoise?.Invoke();
        }
        else
        {
            Debug.Log("dfsdsds");
        }
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        incountManager = GetComponentInParent<IncountManager>();
        incountManager.onContinue += TestFunc;
        uiBlur = GetComponentInChildren<UIBlur>();
        thisCanvasGroup = GetComponent<CanvasGroup>();  
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

        h_child = child1.GetChild(2);      //Last
        g_child = h_child.GetChild(0);
        lastFirstTMP = g_child.GetComponent<TextMeshProUGUI>();
        lastFirstTMPCanvasGroup = g_child.GetComponent<CanvasGroup>();
        g_child = h_child.GetChild(1);
        lastSecondTMP = g_child.GetComponent<TextMeshProUGUI>();
        lastSecondTMPCanvasGroup = g_child.GetComponent<CanvasGroup>();

        g_child = child1.GetChild(1);                //Option
        btnTMPs = g_child.GetComponentsInChildren<TextMeshProUGUI>(true);
        buttons = g_child.GetComponentsInChildren<Button>(true);
        btnCanvasGroups = g_child.GetComponentsInChildren<CanvasGroup>(true);

        buttons[0].onClick.AddListener(() => OnOneOptionClick());
        buttons[1].onClick.AddListener(() => OnTwoOptionClick());
        buttons[2].onClick.AddListener(() => OnThreeOptionClick());
        buttons[3].onClick.AddListener(() => OnFourOptionClick());

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

        child1.gameObject.SetActive(false);
        child0 = transform.GetChild(0);
        blurRect = child0.GetComponent<RectTransform>();
        BlurUpDown(2000);

    }

    private void BlurUpDown(float range)
    {
        Vector2 currentPos = blurRect.anchoredPosition;
        currentPos.y = range;
        blurRect.anchoredPosition = currentPos;
    }

    public void TestFunc(int incountIndex, int choiseNum)
    {
        isChoisOn = true;

        //텍스트 안 보이게 처리
        firstTMPCanvasGroup.alpha = 0;
        secondTMPCanvasGroup.alpha = 0;
        lastFirstTMPCanvasGroup.alpha = 0;
        lastSecondTMPCanvasGroup.alpha = 0;

        OnActive(true);             //자식 오브젝트 활성화 ---------후에 페이드인 처리----------
        uiBlur.BeginBlur(2.0f);     //블러 시작 처리

        cerChoiseIndex = choiseNum;
        switch (incountIndex)
        {
            case 1:
                //cerChoiseData = choise1[0];
                cerChoise = choise1;
                break;
            case 2:
                break;
            case 3:
                break;  

        }

        ChoiseTMPUpdate(cerChoise[cerChoiseIndex]);

    }

    private void OnActive(bool isOn)
    {
        if (isOn)
        {
            BlurUpDown(0);
        }
        else
        {
            BlurUpDown(2000);
        }
        child1.gameObject.SetActive(isOn);

    }

    private void EndIncount()
    {
        uiBlur.EndBlur(2.0f);       //블러 종료 처리
        StartCoroutine( FadeInCo_EndIncount(thisCanvasGroup, false));

    }

    #region 버튼과 Fadeinout

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
        // 선택지가 비어있지 않다면 -- 후속선택지가 있다면
        if(selectChoise.incountIndex < 900)
        {
            firstTMP.text = selectChoise.msg1;
            secondTMP.text = selectChoise.msg2;

            StartCoroutine(FadeInCo(firstTMPCanvasGroup, true));

            StartCoroutine(FadeInCo(secondTMPCanvasGroup, true, 0.8f));

            //모든 버튼 안 보이게 처리
            for(int i = 0; i < btnCanvasGroups.Length; i++)
            {
                btnCanvasGroups[i].alpha = 0;
            }

            ///버튼 활성화
            for (int i = 0; i < selectChoise.choiseCount; i++)
            {
                btnTMPs[i].text = selectChoise.choiseList[i];

                //필요한 버튼만 입력 활성화 및 보이게 하기
                StartCoroutine(FadeInCo(btnCanvasGroups[i], true, i * 0.2f));
                buttons[i].interactable = true;
            }
        }
        else
        { 
            //선택한 버튼 만 보이도록 함
            StartCoroutine(FadeInCo_Button(btnCanvasGroups[selectChoiseNum]));

            lastFirstTMP.text = selectChoise.msg1;
            lastSecondTMP.text = selectChoise.msg2;

            StartCoroutine(FadeInCo(lastFirstTMPCanvasGroup, true));

            StartCoroutine(FadeInCo(lastSecondTMPCanvasGroup, true, 0.8f));
            /// 마지막 선택지 없는 choiseData는 클릭후 다음 인카운트로 넘어간다고 알림
            isChoisOn = false;
        }
    }


    IEnumerator FadeInCo(CanvasGroup canvasGroup, bool fadeIn, float delay = 0, float duration = 1)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (timer < duration)
        {
            float currentValue = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            canvasGroup.alpha = currentValue;
            timer += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator FadeInCo_EndIncount(CanvasGroup canvasGroup, bool fadeIn, float delay = 0, float duration = 1)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (timer < duration)
        {
            float currentValue = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            canvasGroup.alpha = currentValue;
            timer += Time.deltaTime;
            yield return null;
        }

        //종료 후 비활성화
        OnActive(false);
    }

    /// <summary>
    /// 현재 선택된 버튼만 제외하고 fade Out 하는 코루틴
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="fadeIn"></param>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator FadeInCo_Button(CanvasGroup canvasGroup, bool fadeIn = false, float delay = 0, float duration = 1)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (timer < duration)
        {
            float currentValue = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            foreach(var canvas in btnCanvasGroups)
            {
                if(canvas == canvasGroup)
                {
                    continue;
                }
                canvas.alpha = currentValue;
            }
            timer += Time.deltaTime;
            yield return null;
        }

    }
    #endregion

}
