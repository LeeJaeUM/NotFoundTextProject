using Krivodeling.UI.Effects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContinueIncount : MonoBehaviour
{
    public IncountManager incountManager;
    public UIBlur uiBlur = null;

    public TextMeshProUGUI firstTMP;
    public TextMeshProUGUI secondTMP;

    public CanvasGroup firstTMPCanvasGroup;
    public CanvasGroup secondTMPCanvasGroup;

    public Button[] buttons = new Button[4];
    public TextMeshProUGUI[] btnTMPs = new TextMeshProUGUI[4];
    public CanvasGroup[] btnCanvasGroups = new CanvasGroup[4];

    Transform child0;    //Blur Image
    Transform child1;    //DIalogueGroup (1)


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

        child1.gameObject.SetActive(false);
        child0 = transform.GetChild(0);
        child0.gameObject.SetActive(false);
    }


    public void TestFunc(int choiseNum)
    {
        OnActive(true);             //자식 오브젝트 활성화
        uiBlur.BeginBlur(2.0f);     //블러 시작 처리



        //uiBlur.EndBlur(2.0f);       //블러 종료 처리
    }

    private void OnActive(bool isOn)
    {
        child0.gameObject.SetActive(isOn); 
        child1.gameObject.SetActive(isOn); 
    }
}
