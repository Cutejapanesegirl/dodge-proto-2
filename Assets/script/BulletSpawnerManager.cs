using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawnerManager : MonoBehaviour
{
    public GameObject spawnerPrefab; // 생성할 스포너 프리팹
    public float initialDelay = 1f;  // 최초 스폰 대기 시간
    public float spawnInterval = 1f; // 스포너 추가 간격
    public float spawnDelay = 1f;    // 각 스포너 간 생성 간격
    public float spawnHeight = 1f;   // 스포너 생성 높이
    public Transform playerTransform;
    public Transform rotatingPlatform;

    private int spawnerCount = 1; // 생성된 스포너 개수
    private const float squareSize = 15f; // 스폰할 영역 크기

    private void Start()
    {
        StartCoroutine(StartSpawningAfterDelay());

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerController>()?.transform;
            if (playerTransform == null)
                Debug.LogError("PlayerController를 찾을 수 없습니다.");
        }

        if (rotatingPlatform == null)
            Debug.LogError("회전하는 플랫폼이 설정되지 않았습니다.");
    }

    private IEnumerator StartSpawningAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        StartCoroutine(SpawnSpawnersAroundSquare());
    }

    private IEnumerator SpawnSpawnersAroundSquare()
    {
        List<Vector3> spawnPositions = GenerateSpawnPositions();
        Shuffle(spawnPositions);

        foreach (Vector3 position in spawnPositions)
        {
            SpawnSpawner(position);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private List<Vector3> GenerateSpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        float halfSize = squareSize / 2f;
        float midPoint = halfSize / 2f;

        // 사각형 경계를 따라 위치 추가
        for (float i = -halfSize; i <= halfSize; i += 2f)
        {
            if (Mathf.Abs(i) > midPoint)
            {
                positions.Add(new Vector3(i, spawnHeight, halfSize));  // 상단
                positions.Add(new Vector3(i, spawnHeight, -halfSize)); // 하단
                positions.Add(new Vector3(halfSize, spawnHeight, i));  // 우측
                positions.Add(new Vector3(-halfSize, spawnHeight, i)); // 좌측
            }
        }
        return positions;
    }

    private void SpawnSpawner(Vector3 position)
    {

        GameObject newSpawner = Instantiate(spawnerPrefab, position, Quaternion.identity, rotatingPlatform); 
        newSpawner.name = $"Bullet Spawner {spawnerCount++}";

        BulletSpawner bulletSpawner = newSpawner.GetComponent<BulletSpawner>();
        if (bulletSpawner != null)
            bulletSpawner.target = playerTransform;
        else
            Debug.LogError("BulletSpawner 스크립트를 찾을 수 없습니다.");
    }

    private void Shuffle(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}
