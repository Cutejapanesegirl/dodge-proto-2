using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 라이브러리
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Threading; // 씬 


public class GameManager : MonoBehaviour
{

    public TMP_Text timeText; // 생존 시간을 표시할
    public static GameManager instance;
    public GameObject gameoverText;
    public PoolManager pool;
    public PlayerController player;
    public LevelUp uiLevelUp;
    public GameObject uiResult;
    public GameObject SubMenu;
    public BulletSpawner bulletSpawner;
    public Rotator rotator;
    public GameObject RetryButton;

    private TMP_Text recordText;
    private GameObject recordObject;
    private float surviveTime; //생존 시간
    public bool isLive;

    void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Time.timeScale = 0;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindNewRotator();
        AssignBulletSpawnerTarget();
    }

    // 새로운 rotator를 찾아 할당
    void FindNewRotator()
    {
        rotator = FindObjectOfType<Rotator>();
    }

    void AssignBulletSpawnerTarget()
    {
        PlayerController player = FindObjectOfType<PlayerController>(); // Player 찾기
        if (player == null) return; // Player가 없으면 아무것도 하지 않음

        BulletSpawner[] bulletSpawners = FindObjectsOfType<BulletSpawner>(); // 모든 BulletSpawner 찾기
        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.target = player.transform; // 모든 BulletSpawner의 target 설정
        }
    }

    public void GameStart()
    {
        surviveTime = 0f;
        Resume();
    }

    public void GameOver()
    {
        uiResult.SetActive(true);
        Stop();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(RetryButton);
    }

    public void GameRetry()
    {
        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
            GameManager.instance = null; // 참조 초기화
        }

        // Canvas도 DontDestroyOnLoad 상태라면 함께 삭제 (필요 시)
        if (CanvasManager.instance != null)
        {
            Destroy(CanvasManager.instance.gameObject);
            CanvasManager.instance = null;
        }

        if (player != null)
        {
            Destroy(player.gameObject);
        }

        if (PoolManager.instance != null)
        {
            Destroy(PoolManager.instance.gameObject);
            PoolManager.instance = null;
        }

        if (GameSaveSystem.instance != null)
        {
            Destroy(GameSaveSystem.instance.gameObject);
            GameSaveSystem.instance = null;
        }

        if (Rotator1.instance != null)
        {
            Destroy(Rotator1.instance.gameObject);
            Rotator1.instance = null;
        }

        SceneManager.LoadScene(0);  // 씬 리셋
    }

    // Update is called once per frame
    void Update()
    {
        // 게임오버가 아닐동안
        if (!isLive)
        {
            return;
        }

        // 생존 시간 갱신
        surviveTime += Time.deltaTime;
        timeText.text = $"Time: {(int)surviveTime}";

        if (surviveTime >= rotator.gametimer)
        {
            uiLevelUp.Show();
            surviveTime = 0f;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (SubMenu.activeSelf)
            {
                SubMenu.SetActive(false);
                Time.timeScale = 1f;
            }

            else
            {
                SubMenu.SetActive(true);
                Time.timeScale = 0f;
            }

        }
    }

    public void OnSaveAndRetryButton()
    {
        StartCoroutine(SaveAndRetryRoutine());
    }

    private IEnumerator SaveAndRetryRoutine()
    {
        GameSaveSystem.instance.SaveGame(); // 저장 실행

        yield return null; // 한 프레임 대기 (또는 필요시 WaitForSeconds 사용)

        GameRetry(); // 저장이 끝난 다음에 리트라이
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        GameObject obj = GameObject.Find("Player");
        if (obj != null)
        {
            obj.transform.position = new Vector3(0, obj.transform.position.y, 0);
        }
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 종료
#else
            Application.Quit(); // 빌드된 게임에서 종료
#endif
    }
}