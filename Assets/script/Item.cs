using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        Debug.Log($"{name} OnEnable È£ÃâµÊ. ÇöÀç Level: {level}");
        textLevel.text = $"Lv. {level + 1}";

        switch (data.itemType)
        {
            case ItemData.ItemType.Size:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.Item:
            case ItemData.ItemType.Speed:
            case ItemData.ItemType.Spawn:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
        }
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Size:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.Item:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();

                    if (data.itemType == ItemData.ItemType.Item)
                    {
                        gear.ShieldEffect = Resources.Load<GameObject>("Prefabs/ShieldEffect");
                    }

                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];

                    gear.originalRate = nextRate;
                    gear.LevelUp(nextRate);
                }
                break;
            case ItemData.ItemType.Speed:
            case ItemData.ItemType.Spawn:
                break;

                //if (level == 0)
                //{
                //    GameObject newWeapon = new GameObject();
                //    weapon = newWeapon.AddComponent<Weapon>();
                //    weapon.Init(data);
                //}
                //else
                //{
                //    float nextDamage = data.damages[level];

                //    int nextCount = 0;
                //    nextCount += data.counts[level];

                //    weapon.Levelup(nextDamage, nextCount);
                //}
                //break;
        }

        Debug.Log($"[Before] {name} Level: {level}");

        level++;

        Debug.Log($"[After] {name} Level: {level}");

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
