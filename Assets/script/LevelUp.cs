using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
    DemeritManager demeritManager;

    public int currentStage = 1;
    public int currentRound = 1;
    public TMP_Text stageText;
    public GameObject StageUI;
    public GameObject GameWin;
    public GameObject DemeritUI;
    public BulletSpawner bulletSpawner;
    public GameObject firstSelectedButton; // 첫 번째로 선택될 버튼
    public GameObject stageFirstButton;
    public GameObject demeritFirstButton;
    public int BossRound = 3;
    public int ClearStage = 4;

    public int[] bossScene = { 1, 2, 3 }; // 보스 씬의 빌드 인덱스 저장
    public List<int> clearedBosses = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
        demeritManager = FindObjectOfType<DemeritManager>();

        if (demeritManager == null)
        {
            Debug.LogError("DemeritManager를 찾을 수 없습니다! 씬에 존재하는지 확인하세요.");
        }
    }

    public void Show()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        if (currentRound < BossRound)
        {
            currentRound++;
        }
        else
        {
            currentStage++;
            currentRound = 1;
            UpdateStageText();
        }

        if (currentStage >= ClearStage)
        {
            Time.timeScale = 0;
            StageUI.SetActive(false);
            GameWin.SetActive(true);  //  특정 UI 띄우기
            return;  //  씬 변경 방지
        }

        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();

        UpdateStageText();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

    }

    public void StageUIButton()
    {
        // HashSet은 검색 속도가 더 빠름
        HashSet<string> bossScenes = new HashSet<string> { "Boss1", "Boss2", "Boss3", "Boss4", "Boss5", "Boss6", "Boss7","Boss8" };

        // 현재 씬 정보 가져오기
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (bossScenes.Contains(currentSceneName))
        {
            BossDefeated();
            Debug.Log("bossd");
        }
        else
        {
            Hide();
            Debug.Log("hide");
        }
    }

    void LoadBossStage()
    {
        List<int> availableBoss = new List<int>(bossScene);
        availableBoss.RemoveAll(index => clearedBosses.Contains(index));

        if (availableBoss.Count > 0)
        {
            int randomBoss = availableBoss[Random.Range(0, availableBoss.Count)];
            SceneManager.LoadScene(randomBoss);
        }
    }

    public void BossDefeated()
    {
        int currentBoss = SceneManager.GetActiveScene().buildIndex;

        if (!clearedBosses.Contains(currentBoss))
        {
            clearedBosses.Add(currentBoss);
        }

        UpdateStageText();
        StartCoroutine(LoadMainSceneAndFocus());
    }

    IEnumerator LoadMainSceneAndFocus()
    {
        SceneManager.LoadScene(0);

        // 한 프레임 기다리기 (씬이 완전히 로드되도록)
        yield return null;

        // 이 다음부터는 새로운 씬 기준
        GameObject obj = GameObject.Find("Player");
        if (obj != null)
        {
            obj.transform.position = new Vector3(0, obj.transform.position.y, 0);
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(stageFirstButton);
    }

    public void DemeritShow()
    {
        demeritManager.Next();
        rect.localScale = Vector3.zero;
        DemeritUI.transform.localScale = Vector3.one;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(demeritFirstButton);
    }

    public void Stage()
    {
        DemeritUI.transform.localScale = Vector3.zero;
        StageUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(stageFirstButton);

    }

    public void Hide()
    {
        if (currentRound == BossRound)
        {
            LoadBossStage();
        }

        StageUI.SetActive(false);
        GameManager.instance.Resume();
    }

    void Next()
    {
        items = GetComponentsInChildren<Item>(true);

        Debug.Log("Next() 호출됨! 현재 아이템 개수: " + items.Length);
        foreach (var item in items)
        {
            Debug.Log("아이템 이름: " + item.name);
        }

        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                break;
        }

        for (int index=0; index < ran.Length; index++)
        { 
            Item ranItem = items[ran[index]];

            if (ranItem.level == ranItem.data.damages.Length)
            {
               // items[Random.Range(4, 7)].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }
    }

    void UpdateStageText()
    {
        string roundText = (currentRound == BossRound) ? "BOSS" : currentRound.ToString();
        stageText.text = $"Next Stage : {currentStage}-{roundText}";
    }
}
