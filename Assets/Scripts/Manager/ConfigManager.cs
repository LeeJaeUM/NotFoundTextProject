using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public GameObject childObj;
    public bool isOnConfig;

    // 자식 오브젝트의 버튼을 찾는 변수
    private Button titleButton;
    private Button exitGameButton;

    PlayerInputActions inputActions;

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.ESC.performed += OnEsc;
    }

    private void OnDisable()
    {
        inputActions.Player.ESC.performed -= OnEsc;
        inputActions.Player.Disable();
    }

    private void OnEsc(InputAction.CallbackContext context)
    {
        isOnConfig = !isOnConfig;
        childObj.SetActive(isOnConfig);
        StartCoroutine(FadeinConfig(isOnConfig));
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // 자식 오브젝트에서 버튼 찾기
        Transform child = transform.GetChild(0);
        child = child.GetChild(1);
        titleButton = child.GetChild(0).GetComponent<Button>();
        exitGameButton = child.GetChild(1).GetComponent<Button>();

        childObj = transform.GetChild(0).gameObject;

        // 각 버튼의 클릭 이벤트에 메서드 연결
        titleButton.onClick.AddListener(OnTitleButtonClicked);
        exitGameButton.onClick.AddListener(OnExitGameButtonClicked);

        childObj.SetActive(false);
    }    
    
    // 버튼 클릭 메서드
    void OnTitleButtonClicked()
    {
        // Title 씬으로 전환
        SceneManager.LoadScene("Title");
    }

    void OnExitGameButtonClicked()
    {
        // 게임 종료
        Application.Quit();

        // 에디터에서는 종료되지 않으므로 에디터에서 실행 중일 때 메시지 출력
#if UNITY_EDITOR
        Debug.Log("Game is exiting... (Only works in build)");
#endif
    }

    IEnumerator FadeinConfig(bool isOn)
    {
        float timer = 0f;
        float startAlpha = isOn ? 0f : 1f;
        float endAlpha = isOn ? 1f : 0f;
        float transitionDuration = 0.4f;
        while (timer < transitionDuration)
        {
            float currentValue = Mathf.Lerp(startAlpha, endAlpha, timer / transitionDuration);
            canvasGroup.alpha = currentValue;
            timer += Time.deltaTime;
            yield return null;
        }

    }
}
