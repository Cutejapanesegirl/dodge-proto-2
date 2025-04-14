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
    public GameObject DemeritUI;
    public GameObject firstSelectedButton;
    public GameObject stageFirstButton;
    public GameObject demeritFirstButton;

    public BulletSpawner bulletSpawner;
    public List<int> clearedBosses = new List<int>();

    private RectTransform rect;
    private Item[] items;
    private DemeritManager demeritManager;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
        demeritManager = FindObjectOfType<DemeritManager>();
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

        ActivateRandomItems();
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

    void ActivateRandomItems()
    {
        items = GetComponentsInChildren<Item>(true);
        foreach (var item in items)
        {
            item.gameObject.SetActive(false);
        }

        List<Item> selectable = items.Where(i => i.level < i.data.damages.Length).ToList();
        List<Item> chosen = new List<Item>();

        while (chosen.Count < 3 && selectable.Count > 0)
        {
            int r = Random.Range(0, selectable.Count);
            chosen.Add(selectable[r]);
            selectable.RemoveAt(r);
        }

        foreach (var item in chosen)
        {
            item.gameObject.SetActive(true);
        }
    }

    void UpdateStageText()
    {
        string roundText = (currentRound == BossRound) ? "BOSS" : currentRound.ToString();
        stageText.text = $"Next Stage : {currentStage}-{roundText}";
    }
}
