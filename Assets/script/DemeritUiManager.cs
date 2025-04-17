using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemeritUIManager : MonoBehaviour
{
    private Item[] items;

    void Awake()
    {
        items = GetComponentsInChildren<Item>(true);
    }

    public void ShowRandomDemerits(int count = 3)
    {
        foreach (var item in items)
            item.gameObject.SetActive(false);

        var selected = new List<int>();
        while (selected.Count < count)
        {
            int index = Random.Range(0, items.Length);
            if (!selected.Contains(index))
                selected.Add(index);
        }

        foreach (int index in selected)
        {
            items[index].gameObject.SetActive(true);
        }
    }
}

