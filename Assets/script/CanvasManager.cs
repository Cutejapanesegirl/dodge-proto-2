using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;

    private GameObject levelUpUI;
    private GameObject gameStartUI;
    private GameObject gameResultUI;
    private GameObject hudUI;
    private GameObject stageUI;

    private Canvas currentCanvas; // 현재 씬의 Canvas를 참조할 변수

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 기존의 CanvasManager를 삭제
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // CanvasManager는 씬 변경 시에도 유지

        // 씬이 변경될 때마다 UI 오브젝트들을 갱신
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 씬 변경 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 씬에서 UI 오브젝트들 다시 할당
        UpdateUI();

        // 새 씬의 Canvas를 찾거나 새로 할당
        Canvas newCanvas = FindObjectOfType<Canvas>();

        if (newCanvas != null)
        {
            // 기존의 CanvasManager에 새로운 Canvas를 할당
            currentCanvas = newCanvas;

            // 새로운 Canvas도 DontDestroyOnLoad로 유지
            DontDestroyOnLoad(currentCanvas.gameObject);
        }
        else
        {
            Debug.LogWarning("새로운 씬에 Canvas가 없습니다.");
        }
    }

    public void UpdateUI()
    {
        // 현재 씬에서 필요한 UI 오브젝트들을 다시 찾기
        levelUpUI = GameObject.Find("LevelUP");
        gameStartUI = GameObject.Find("GameStart");
        gameResultUI = GameObject.Find("GameResult");
        hudUI = GameObject.Find("HUD");
        stageUI = GameObject.Find("StageUI");

        // 각 UI가 null이 아닌지 체크하여 필요 시 처리 추가
    }
}
