using System.Collections.Generic;
using UnityEngine;

public class DemeritManager : MonoBehaviour
{
    public BulletSpawner[] bulletSpawners;
    private Item[] demeritItems;

    private void Awake()
    {
        bulletSpawners = FindObjectsOfType<BulletSpawner>();
        demeritItems = GetComponentsInChildren<Item>(true);
    }

    public void ResetSpawnerDefaults()
    {
        foreach (var spawner in bulletSpawners)
        {
            if (spawner == null) continue;
            spawner.currentBulletIndex = 0;
            spawner.spawnRateMin = 0.5f;
            spawner.spawnRateMax = 3f;
        }
    }

    public void ApplyDemeritEffect(ItemData data, int level)
    {
        float rate = data.damages[Mathf.Min(level, data.damages.Length - 1)];

        switch (data.itemType)
        {
            case ItemData.ItemType.Speed:
                for (int i = 0; i < level; i++) UpgradeBullet();
                break;
            case ItemData.ItemType.Spawn:
                for (int i = 0; i < level; i++) UpgradeSpawner(rate);
                break;
        }
    }

    public void UpgradeBullet()
    {
        foreach (var spawner in bulletSpawners)
        {
            if (spawner == null || spawner.bulletPrefabs.Length == 0) continue;
            spawner.currentBulletIndex = (spawner.currentBulletIndex + 1) % spawner.bulletPrefabs.Length;
        }
    }

    public void UpgradeSpawner(float rate)
    {
        foreach (var spawner in bulletSpawners)
        {
            if (spawner == null) continue;
            spawner.spawnRateMin = Mathf.Max(0.05f, spawner.spawnRateMin - rate);
            spawner.spawnRateMax = Mathf.Max(0.2f, spawner.spawnRateMax - rate);
        }
    }

    //저장용 레벨 정보 반환
    public List<Item> GetSelectedDemerits()
    {
        List<Item> selected = new List<Item>();

        foreach (var item in demeritItems)
        {
            if (item.level > 0)
                selected.Add(item);
        }

        return selected;
    }

    //  DemeritUI에서 3개 랜덤 활성화
    public void Next()
    {
        demeritItems = GetComponentsInChildren<Item>(true);

        foreach (var item in demeritItems)
        {
            item.gameObject.SetActive(false);
        }

        List<Item> available = new List<Item>();
        foreach (var item in demeritItems)
        {
            if (item.level < item.data.damages.Length)
                available.Add(item);
        }

        for (int i = 0; i < 3 && available.Count > 0; i++)
        {
            int r = Random.Range(0, available.Count);
            available[r].gameObject.SetActive(true);
            available.RemoveAt(r);
        }
    }
}
