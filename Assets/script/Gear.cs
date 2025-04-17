using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gear : MonoBehaviour
{
    public static Gear instance;
    public ItemData.ItemType type;
    public int itemId;
    public int level;
    public float rate;
    public float timer;
    public float cooltime = 10f;
    public bool hasShield = false;
    private int cooltimeReductionCount = 0;

    public GameObject ShieldEffect;
    public GameObject activeShield;
    public List<BulletSpawner> bulletSpawners;

    public PlayerController player;

    public float originalRate;
    private bool isEffectRemoved = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        player = GetComponentInParent<PlayerController>();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (!GameManager.instance.isLive) return;

        if (type == ItemData.ItemType.Item)
        {
            timer += Time.deltaTime;
            if (timer >= cooltime)
            {
                timer = 0f;
                Shield();
                Debug.Log("½¯µå°¡ Àç »ý¼ºµÊ!");
            }
        }
    }

    public void Init(ItemData data)
    {

        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        type = data.itemType;
        itemId = data.itemId;
        level = 1;
        rate = data.damages[0];

        ApplyGear();

        if (type == ItemData.ItemType.Item)
        {
            hasShield = true;
            Shield();
        }

        Debug.Log($"Gear Init: {data.itemId}, rate = {rate}, level = {level}");

    }

    public void LevelUp(float rate)
    {
        level++;
        this.rate = rate;
        ApplyGear();
    }

    public void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
            case ItemData.ItemType.Size:
                SizeDown();
                break;
            case ItemData.ItemType.Item:
                if (cooltimeReductionCount < 5)
                {
                    cooltime -= rate;
                    cooltimeReductionCount++;
                    Debug.Log("ÄðÅ¸ÀÓ °¨¼Ò Àû¿ë! ÇöÀç È½¼ö: " + cooltimeReductionCount);
                }
                timer = 0f;
                break;
            case ItemData.ItemType.Speed:
                foreach (var spawner in bulletSpawners)
                    spawner.currentBulletIndex = Mathf.Min(spawner.bulletPrefabs.Length - 1, level);
                break;
            case ItemData.ItemType.Spawn:
                foreach (var spawner in bulletSpawners)
                {
                    if (spawner == null) continue;
                    spawner.spawnRateMin = Mathf.Max(0.05f, spawner.spawnRateMin - rate);
                    spawner.spawnRateMax = Mathf.Max(0.2f, spawner.spawnRateMax - rate);
                }
                break;
        }
    }

    void SpeedUp()
    {
        float speed = 8;
        GameManager.instance.player.speed = speed + speed * rate;
    }

    void SizeDown()
    {
        GameManager.instance.player.transform.localScale -= GameManager.instance.player.transform.localScale * rate;
    }

    void Shield()
    {
        if (player == null)
            player = GameManager.instance.player;

        if (player != null)
        {
            player.gear = this;
            hasShield = true;

            if (activeShield != null)
                Destroy(activeShield);

            activeShield = Instantiate(ShieldEffect, player.transform.position, Quaternion.identity, player.transform);
        }
    }

    public void RemoveGearEffects()
    {
        if (isEffectRemoved) return;

        originalRate = rate;
        isEffectRemoved = true;

        switch (type)
        {
            case ItemData.ItemType.Shoe:
                GameManager.instance.player.speed = GameManager.instance.player.baseSpeed;
                break;
            case ItemData.ItemType.Size:
                GameManager.instance.player.transform.localScale = GameManager.instance.player.baseScale;
                break;
            case ItemData.ItemType.Item:
                if (activeShield != null)
                    Destroy(activeShield);
                break;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Boss8")
        {
            RemoveGearEffects();
            return;
        }

        if (isEffectRemoved)
        {
            isEffectRemoved = false;
            rate = originalRate;
            ApplyGear();
            Debug.Log("Gear È¿°ú º¹±¸µÊ! rate: " + rate);
        }
    }
}