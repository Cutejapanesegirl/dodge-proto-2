using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    public bool hasShield = false;

    public PlayerController player;

    void Awake()
    {
        player = GameManager.instance.player;
    }


    void Update()
    {

    }

    public void Levelup(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if ( id == 0 )
        {
            
        }
    }

    public void Init(ItemData data)
    {
        // Basic Set
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        damage = data.damages[0];
        count = data.baseCount;

        for (int index=0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:

                break;
            default:
                speed = 0.1f;
                break;

        }
    }



}
