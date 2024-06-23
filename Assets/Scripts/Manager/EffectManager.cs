using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public TextMeshProUGUI[] centerTMPs = new TextMeshProUGUI[2];
    public Image backGround;

    public IncountManager incountManager;

    public Color backDefault; 
    public Color targetColor = Color.white; // 번갈아 바뀔 색상 (예: 흰색)
    private Color startTextColor = Color.black;
    private Color endTextColor = Color.white;

    private void Start()
    {
        
        incountManager = FindAnyObjectByType<IncountManager>();

        centerTMPs = new TextMeshProUGUI[2];
        centerTMPs[0] = incountManager.FirstTMP;
        centerTMPs[1] = incountManager.SecondTMP;

        Transform child = incountManager.transform.GetChild(0);
        child = child.GetChild(1);
        backGround = child.GetComponent<Image>();
        
        backDefault = backGround.color;
        incountManager.onDeepBreath += DeepBreath;
        incountManager.onChagneColor += ChangeColors;
    }

    private void DeepBreath()
    {
        // 색상 전환 코루틴 시작
        StartCoroutine(ColorChangeCoroutine());
    }

    private void ChangeColors(bool isOnChange)
    {
        if(isOnChange)
        {
            TextWhite_ImageBlack();
        }
        else
        {
            TextBlack_ImageWhite();
        }
    }

    private void TextWhite_ImageBlack()
    {
        float duration = 0.5f;
        // 알파값 전환 (0 -> 1)
        StartCoroutine(LerpAlpha(0, 1, duration));

        // TMP 색상 전환
        StartCoroutine(LerpColors(startTextColor, endTextColor, duration));
    } 
    private void TextBlack_ImageWhite()
    {
        float duration = 0.5f;
        // 알파값 전환 (0 -> 1)
        StartCoroutine(LerpAlpha(1, 0, duration));

        // TMP 색상 전환
        StartCoroutine(LerpColors(endTextColor, startTextColor, duration));
    }

    private IEnumerator ColorChangeCoroutine()
    {
        int repeatCount = 2; // 4번 반복
        float duration = 0.5f; // 각 반복은 1초 동안 지속

        for (int i = 0; i < repeatCount; i++)
        {
            // 알파값 전환 (0 -> 1)
            yield return StartCoroutine(LerpAlpha(0, 1, duration));

            // TMP 색상 전환
            yield return StartCoroutine(LerpColors(startTextColor, endTextColor, duration));

            // 알파값 전환 (1 -> 0)
            yield return StartCoroutine(LerpAlpha(1, 0, duration));

            // TMP 색상 전환
            yield return StartCoroutine(LerpColors(endTextColor, startTextColor, duration));
        }

        // 종료 전에 원래 알파값으로 즉시 변경
        backGround.color = new Color(backDefault.r, backDefault.g, backDefault.b, 0); // 알파값 0으로 설정
        foreach (var tmp in centerTMPs)
        {
            tmp.color = startTextColor;
        }
    }

    private IEnumerator LerpAlpha(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color bgColor = backGround.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // 알파값 전환
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            backGround.color = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);

            yield return null;
        }
    }

    private IEnumerator LerpColors(Color startColor, Color endColor, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // TMP 색상 전환
            foreach (var tmp in centerTMPs)
            {
                tmp.color = Color.Lerp(startColor, endColor, t);
            }

            yield return null;
        }
    }

    /*
    private IEnumerator ColorChangeCoroutine()
    {
        float duration = 1f; // 4초 동안 색상 전환
        while (true)
        {
            // 색상 전환 (backDefault -> targetColor)
            yield return StartCoroutine(LerpColors(backDefault, targetColor, startTextColor, endTextColor, duration));

            // 색상 전환 (targetColor -> backDefault)
            yield return StartCoroutine(LerpColors(targetColor, backDefault, endTextColor, startTextColor, duration));
        }
    }

    private IEnumerator LerpColors(Color startBGColor, Color endBGColor, Color startTMPColor, Color endTMPColor, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // 배경 색상 전환
            backGround.color = Color.Lerp(startBGColor, endBGColor, t);

            // TMP 색상 전환
            foreach (var tmp in centerTMPs)
            {
                tmp.color = Color.Lerp(startTMPColor, endTMPColor, t);
            }

            yield return null;
        }
    }*/
}
