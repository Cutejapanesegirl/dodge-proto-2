using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    TMP_Text textLevel;
    TMP_Text textName;
    TMP_Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];

        textName.text = data.itemName;
    }

    void OnEnable()
    {
        if (data == null)
        {
            Debug.LogWarning($"{name}의 data가 null입니다. OnEnable 중단.");
            return;
        }

        if (gear == null)
        {
            StartCoroutine(DelayedReconnect());
        }
        if (gear != null)
        {
            level = gear.level;
        }
        else
        {
            level = GameSaveSystem.instance?.gameData.selectedDemerits
                       .FirstOrDefault(d => d.itemId == data.itemId)?.level ?? 0;
        }

        UpdateUI();

    }

    IEnumerator DelayedReconnect()
    {
        // 최대 1초(1f)까지 대기하며 GameManager와 player가 준비될 때까지 기다림
        float timeout = 1f;
        while ((GameManager.instance == null || GameManager.instance.player == null) && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        TryReconnectGear(); // 이제 안전하게 시도       // 레벨 텍스트 등 업데이트 함수 따로 분리해도 좋음
    }

    public void OnClick()
    {

        Debug.Log($"[Before] {name} Level: {level}");

        switch (data.itemType)
        {
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.Size:
            case ItemData.ItemType.Item:
                if (gear == null)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();

                    if (data.itemType == ItemData.ItemType.Item)
                    {
                        gear.ShieldEffect = Resources.Load<GameObject>("Prefabs/ShieldEffect");
                    }

                    gear.Init(data); // Gear에 itemId, level = 1 설정됨
                }
                else
                {
                    float nextRate = data.damages[Mathf.Min(level, data.damages.Length - 1)];
                    gear.originalRate = nextRate;
                    gear.LevelUp(nextRate);
                }
                break;

            case ItemData.ItemType.Speed:
            case ItemData.ItemType.Spawn:
                if (gear == null)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);  // Init에서 itemType을 보고 디메리트 처리
                }
                else
                {
                    float nextRate = data.damages[Mathf.Min(level, data.damages.Length - 1)];
                    gear.originalRate = nextRate;
                    gear.LevelUp(nextRate);
                }
                break;
        }

        level++;

        Debug.Log($"[After] {name} Level: {level}");

        if (level >= data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }

        if (level >= data.damages.Length) return; // 최대 레벨 도달 시 중복 방지

    }

    public void LoadDemeritLevel(int loadedLevel)
    {
        level = 0;
        for (int i = 0; i < loadedLevel; i++)
        {
            OnClick();
        }
    }

    void TryReconnectGear()
    {
        if (GameManager.instance == null || GameManager.instance.player == null)
        {
            Debug.LogWarning("GameManager 또는 Player가 아직 초기화되지 않았습니다. TryReconnectGear 중단.");
            return;
        }

        Gear[] gears = GameManager.instance.player.GetComponentsInChildren<Gear>(true);
        foreach (var g in gears)
        {
            if (g.itemId == data.itemId)
            {
                gear = g;
                break;
            }
        }
    }

    void SyncDemeritLevelFromSave()
    {
        if (GameSaveSystem.instance != null && GameSaveSystem.instance.demeritManager != null)
        {
            var data = GameSaveSystem.instance.gameData.selectedDemerits.Find(d => d.itemId == this.data.itemId);
            if (data != null)
            {
                level = data.level;
            }
        }
    }

    void UpdateUI()
    {
        Debug.Log($"{name} OnEnable 호출됨. 현재 Level: {level}");
        textLevel.text = $"Lv. {level + 1}";

        if (data.itemType == ItemData.ItemType.Size ||
            data.itemType == ItemData.ItemType.Shoe ||
            data.itemType == ItemData.ItemType.Item ||
            data.itemType == ItemData.ItemType.Speed ||
            data.itemType == ItemData.ItemType.Spawn)
        {
            float percentage = data.damages[Mathf.Min(level, data.damages.Length - 1)] * 100f;
            textDesc.text = string.Format(data.itemDesc, percentage);
        }
    }
}

