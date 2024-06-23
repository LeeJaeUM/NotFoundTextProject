using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    // 자식 오브젝트의 버튼을 찾는 변수
    private Button startGameButton;
    private Button exitGameButton;
    private Button emptyButton;

    void Start()
    {
        // 자식 오브젝트에서 버튼 찾기
        Transform child = transform.GetChild(0);
        startGameButton = child.GetComponent<Button>();
        child = transform.GetChild(1);
        exitGameButton = child.GetComponent<Button>();

        // 각 버튼의 클릭 이벤트에 메서드 연결
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
    }

    // 버튼 클릭 메서드
    void OnStartGameButtonClicked()
    {
        // Main 씬으로 전환
        SceneManager.LoadScene("Main");
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

}
