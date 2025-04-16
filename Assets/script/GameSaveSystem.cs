using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameSaveSystem : MonoBehaviour
{
    public static GameSaveSystem instance;

    private string filePath;
    public GameData gameData = new GameData();

    public LevelUp levelUp;
    public PlayerController player;
    public DemeritManager demeritManager;
    private BulletSpawner[] bulletSpawners;
    public bool LoadingInProgress = false;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "GameSave.json");
        bulletSpawners = FindObjectsOfType<BulletSpawner>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (levelUp == null)
            levelUp = FindObjectOfType<LevelUp>();
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        if (demeritManager == null)
            demeritManager = FindObjectOfType<DemeritManager>();

        bulletSpawners = FindObjectsOfType<BulletSpawner>();
    }

    public void SaveGame()
    {
        gameData.currentRound = levelUp.currentRound;
        gameData.currentStage = levelUp.currentStage;
        gameData.clearedBosses = new List<int>(levelUp.clearedBosses);

        gameData.speed = player.speed;

        if (bulletSpawners.Length > 0)
        {
            gameData.currentBulletIndex = bulletSpawners[0].currentBulletIndex;
            gameData.spawnRateMin = bulletSpawners[0].spawnRateMin;
            gameData.spawnRateMax = bulletSpawners[0].spawnRateMax;
        }

        gameData.equippedGears.Clear();
        foreach (Transform child in player.transform)
        {
            Gear gear = child.GetComponent<Gear>();
            if (gear != null)
            {
                gameData.equippedGears.Add(new GearSaveData
                {
                    itemId = gear.itemId,
                    level = gear.level
                });
            }
        }

        gameData.selectedDemerits.Clear();
        foreach (var item in demeritManager.GetSelectedDemerits())
        {
            gameData.selectedDemerits.Add(new DemeritSaveData
            {
                itemId = item.data.itemId,
                level = item.level
            });
        }

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("���� ���� �Ϸ�");
    }

    public void LoadGame()
    {
        LoadingInProgress = true;

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("����� ������ �����ϴ�.");
            return;
        }

        string json = File.ReadAllText(filePath);
        gameData = JsonUtility.FromJson<GameData>(json);

        levelUp.currentRound = gameData.currentRound;
        levelUp.currentStage = gameData.currentStage;
        levelUp.clearedBosses = new List<int>(gameData.clearedBosses);

        player.speed = gameData.speed;

        foreach (var spawner in bulletSpawners)
        {
            spawner.currentBulletIndex = gameData.currentBulletIndex;
            spawner.spawnRateMin = gameData.spawnRateMin;
            spawner.spawnRateMax = gameData.spawnRateMax;
        }

        foreach (Transform child in player.transform)
        {
            Gear gear = child.GetComponent<Gear>();
            if (gear != null)
                Destroy(child.gameObject);
        }


        foreach (GearSaveData gearSave in gameData.equippedGears)
        {
            ItemData itemData = Resources.Load<ItemData>($"ItemData/Item_{gearSave.itemId}");
            if (itemData == null)
            {
                Debug.LogWarning($"ItemData {gearSave.itemId} �ε� ����");
                continue;
            }

            GameObject newGear = new GameObject();
            Gear gear = newGear.AddComponent<Gear>();
            gear.itemId = gearSave.itemId;
            gear.level = gearSave.level;

            if (itemData.itemType == ItemData.ItemType.Item)
                gear.ShieldEffect = Resources.Load<GameObject>("Prefabs/ShieldEffect");

            gear.Init(itemData);
            for (int i = 1; i < gearSave.level; i++)
                gear.LevelUp(itemData.damages[i]);
        }


        Debug.Log("���� �ҷ����� �Ϸ�");

        LoadingInProgress = false;

    }

    private Item FindDemeritItemById(int id)
    {
        foreach (var item in demeritManager.GetComponentsInChildren<Item>(true))
        {
            if (item.data.itemId == id)
                return item;
        }
        return null;
    }
}
