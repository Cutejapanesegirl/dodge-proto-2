using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ���̺귯��
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Threading; // �� 


public class GameManager : MonoBehaviour
{

    public TMP_Text timeText; // ���� �ð��� ǥ����
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
    private float surviveTime; //���� �ð�
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

    // ���ο� rotator�� ã�� �Ҵ�
    void FindNewRotator()
    {
        rotator = FindObjectOfType<Rotator>();
    }

    void AssignBulletSpawnerTarget()
    {
        PlayerController player = FindObjectOfType<PlayerController>(); // Player ã��
        if (player == null) return; // Player�� ������ �ƹ��͵� ���� ����

        BulletSpawner[] bulletSpawners = FindObjectsOfType<BulletSpawner>(); // ��� BulletSpawner ã��
        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.target = player.transform; // ��� BulletSpawner�� target ����
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
            GameManager.instance = null; // ���� �ʱ�ȭ
        }

        // Canvas�� DontDestroyOnLoad ���¶�� �Բ� ���� (�ʿ� ��)
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

        SceneManager.LoadScene(0);  // �� ����
    }

    // Update is called once per frame
    void Update()
    {
        // ���ӿ����� �ƴҵ���
        if (!isLive)
        {
            return;
        }

        // ���� �ð� ����
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
        GameSaveSystem.instance.SaveGame(); // ���� ����

        yield return null; // �� ������ ��� (�Ǵ� �ʿ�� WaitForSeconds ���)

        GameRetry(); // ������ ���� ������ ��Ʈ����
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
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ����
#else
            Application.Quit(); // ����� ���ӿ��� ����
#endif
    }
}