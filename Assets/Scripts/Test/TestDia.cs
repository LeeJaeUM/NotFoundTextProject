using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestDia : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    private PlayerInputActions inputActions;
    public float stepDelay = 0.8f;
    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        inputActions = new PlayerInputActions();
    }


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
        Settings();
    }

    void Settings()
    {
        StartCoroutine(TypeText(textMeshProUGUI.text));
    }

    private IEnumerator TypeText(string text)
    { 
        
        textMeshProUGUI.text = ""; // 기존 텍스트 초기화
        string currentText = "";

        foreach (char letter in text)
        {
            currentText += letter;
            textMeshProUGUI.text = currentText;
            yield return new WaitForSeconds(stepDelay);
        }

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
    }
}
