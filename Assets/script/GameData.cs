using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]

public class GameData
{
    public int dataVersion = 1;

    public int currentRound;
    public int currentStage;
    public List<int> clearedBosses = new List<int>();
    public List<GearSaveData> equippedGears = new List<GearSaveData>();
    public List<DemeritSaveData> selectedDemerits = new List<DemeritSaveData>();

    public float speed;
    public Vector3 playerScale;

    public int currentBulletIndex = 0;
    public float spawnRateMin = 0.1f;
    public float spawnRateMax = 3f;
}

[Serializable]
public class GearSaveData
{
    public int itemId;
    public int level;
}

[Serializable]
public class DemeritSaveData
{
    public int itemId;
    public int level;
}
