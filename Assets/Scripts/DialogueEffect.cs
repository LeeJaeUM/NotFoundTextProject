using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueEffect : MonoBehaviour
{
    private float stepDelay = 0.05f;

    public int lengthCount = 0;
    
    Coroutine coroutine;

    public TextMeshProUGUI textTMP;
    private IncountManager incountManager;

    private void Awake()
    {
        textTMP = GetComponent<TextMeshProUGUI>();
        incountManager = GetComponentInParent<IncountManager>();
    }

    private void Start()
    {
        incountManager.onLengthCountPush += Settings;
        incountManager.onChoiseStart += ChoiseStart;
    }

    private void ChoiseStart()
    {
        StopAllCoroutines();
    }

    public void Settings(int num, string text)
    {
        if (lengthCount == num)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(TypeText(text));
        }
    }

    private IEnumerator TypeText(string text)
    {
        /*
        textMeshProUGUI.text = ""; // 기존 텍스트 초기화
        string[] words = text.Split(' '); // 텍스트를 띄어쓰기를 기준으로 나눔
        string currentText = "";

        foreach (string word in words)
        {
            if (currentText.Length > 0)
            {
                currentText += " "; // 단어 사이에 띄어쓰기를 추가
            }
            currentText += word;
            textMeshProUGUI.text = currentText;
            yield return new WaitForSeconds(stepDelay);
        }*/


        textTMP.text = ""; // 기존 텍스트 초기화
        string currentText = "";

        foreach (char letter in text)
        {
            currentText += letter;
            textTMP.text = currentText;
            yield return new WaitForSeconds(stepDelay);
        }
    }
}
