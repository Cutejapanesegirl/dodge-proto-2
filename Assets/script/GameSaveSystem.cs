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
        gameData.playerScale = player.transform.localScale;

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
        Debug.Log("게임 저장 완료");
    }

    public void LoadGame()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("저장된 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(filePath);
        gameData = JsonUtility.FromJson<GameData>(json);

        levelUp.currentRound = gameData.currentRound;
        levelUp.currentStage = gameData.currentStage;
        levelUp.clearedBosses = new List<int>(gameData.clearedBosses);

        player.speed = gameData.speed;
        player.transform.localScale = gameData.playerScale;
        player.gear = player.GetComponentInChildren<Gear>();

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
                Debug.LogWarning($"ItemData {gearSave.itemId} 로드 실패");
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

        foreach (var data in gameData.selectedDemerits)
        {
            Item item = FindDemeritItemById(data.itemId);
            if (item == null)
            {
                Debug.LogWarning($"Demerit itemId {data.itemId} 로드 실패");
                continue;
            }

            item.level = data.level;
            demeritManager.ApplyDemeritEffect(item.data, item.level);
        }

        Debug.Log("게임 불러오기 완료");
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
