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

    public string[] dividedStrings;


    public float transitionDuration = 1f; // 변화에 걸리는 시간
    public float currentValue = 0;

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
                textIndex = 0;
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

    public Button leftChoise;
    public Button rightChoise;

    public TextMeshProUGUI leftButtonTMP;
    public TextMeshProUGUI rightButtonTMP;

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
        leftChoise.onClick.AddListener(() => OnLeftOptionClick());
        rightChoise.onClick.AddListener(() => OnRightOptionClick());
        inputActions = new PlayerInputActions();
    }


    private void Start()
    {
        // 초기화: 텍스트 숨기기
        firstTMPCanvasGroup.alpha = 0f;
        secondTMPCanvasGroup.alpha = 0f;

        firstTMP.text = incountDatas[incountIndex].incountSession[0].msg;
        secondTMP.text = incountDatas[incountIndex].incountSession[1].msg;

        maxTextIndex = incountDatas[incountIndex].incountSession.Count;

    }


    private void OnClick(InputAction.CallbackContext context)
    {
        if (lengthCount == maxLengthCount)
        {
            lengthCount = 0;
            GoNextText();
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

    private void GoNextText()
    {
        // StartCoroutine(FadeInCo(firstTMPCanvasGroup, false));
        // StartCoroutine(FadeInCo(secondTMPCanvasGroup, false));
        StartCoroutine(FadeOutCo(firstTMPCanvasGroup, secondTMPCanvasGroup));

    }


    IEnumerator FadeInCo(CanvasGroup canvasGroup, bool fadeIn)
    {
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
        TMPUpdate();
    }
    private void TMPUpdate()
    {

        TextIndex++; 
        firstTMP.text = incountDatas[incountIndex].incountSession[textIndex].msg;
        TextIndex++;
        if (TextIndex == 0)
            return;
        secondTMP.text = incountDatas[incountIndex].incountSession[textIndex].msg;
    }


    public void OnLeftOptionClick()
    {

    }
    public void OnRightOptionClick()
    {

    }
}
