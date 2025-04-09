using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
   void Update()
    {
            GameManager.instance.pool.Get(1);
    }
}
