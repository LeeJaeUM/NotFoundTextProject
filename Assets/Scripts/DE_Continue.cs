using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DE_Continue : MonoBehaviour
{
    private float stepDelay = 0.03f;

    public int tmpIndex = 0;

    Coroutine coroutine;

    public TextMeshProUGUI textTMP;
    private ContinueIncount continueIncount;

    private void Awake()
    {
        textTMP = GetComponent<TextMeshProUGUI>();
        continueIncount = GetComponentInParent<ContinueIncount>();
    }

    private void Start()
    {
        continueIncount.onDialogues += Settings;
    }

    public void Settings(int num, string text)
    {
        if (tmpIndex == num)
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
