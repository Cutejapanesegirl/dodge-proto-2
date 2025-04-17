using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class LevelUp : MonoBehaviour
{
    public int currentStage = 1;
    public int currentRound = 1;
    public int BossRound = 3;
    public int ClearStage = 4;

    public TMP_Text stageText;
    public GameObject StageUI;
    public GameObject GameWin;
    public GameObject DemeritUIManager;
    public GameObject firstSelectedButton;
    public GameObject stageFirstButton;
    public GameObject demeritFirstButton;

    public BulletSpawner bulletSpawner;
    public List<int> clearedBosses = new List<int>();

    private RectTransform rect;
    private Item[] items;
    private DemeritUIManager demeritUIManager;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
        demeritUIManager = FindObjectOfType<DemeritUIManager>();
    }

    public void Show()
    {
        ClearAllBullets();

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
            GameWin.SetActive(true);
            return;
        }

        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();

        UpdateStageText();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    void ClearAllBullets()
    {
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(bullet);
        }
    }

    public void StageUIButton()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Boss"))
        {
            BossDefeated();
        }
        else
        {
            Hide();
        }
    }

    public void BossDefeated()
    {
        int bossIndex = SceneManager.GetActiveScene().buildIndex;
        if (!clearedBosses.Contains(bossIndex))
        {
            clearedBosses.Add(bossIndex);
        }

        UpdateStageText();
        StartCoroutine(LoadMainSceneAndFocus());
    }

    IEnumerator LoadMainSceneAndFocus()
    {
        SceneManager.LoadScene(0);
        yield return null;

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(0, player.transform.position.y, 0);
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(stageFirstButton);
    }

    public void DemeritShow()
    {
        demeritUIManager.ShowRandomDemerits();
        rect.localScale = Vector3.zero;
        DemeritUIManager.transform.localScale = Vector3.one;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(demeritFirstButton);
    }

    public void Stage()
    {
        DemeritUIManager.transform.localScale = Vector3.zero;
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

    void LoadBossStage()
    {
        int[] allBossScenes = { 1, 2, 3, 4, 5, 6, 7, 8 };
        List<int> availableBosses = allBossScenes.Except(clearedBosses).ToList();

        if (availableBosses.Count > 0)
        {
            int random = Random.Range(0, availableBosses.Count);
            SceneManager.LoadScene(availableBosses[random]);
        }
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

        for (int index = 0; index < ran.Length; index++)
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
