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
            Debug.LogWarning($"{name}�� data�� null�Դϴ�. OnEnable �ߴ�.");
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
        // �ִ� 1��(1f)���� ����ϸ� GameManager�� player�� �غ�� ������ ��ٸ�
        float timeout = 1f;
        while ((GameManager.instance == null || GameManager.instance.player == null) && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        TryReconnectGear(); // ���� �����ϰ� �õ�       // ���� �ؽ�Ʈ �� ������Ʈ �Լ� ���� �и��ص� ����
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

                    gear.Init(data); // Gear�� itemId, level = 1 ������
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
                    gear.Init(data);  // Init���� itemType�� ���� ��޸�Ʈ ó��
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

        if (level >= data.damages.Length) return; // �ִ� ���� ���� �� �ߺ� ����

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
            Debug.LogWarning("GameManager �Ǵ� Player�� ���� �ʱ�ȭ���� �ʾҽ��ϴ�. TryReconnectGear �ߴ�.");
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
        Debug.Log($"{name} OnEnable ȣ���. ���� Level: {level}");
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

