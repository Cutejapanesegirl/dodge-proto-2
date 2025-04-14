using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject[] bulletPrefabs; // 생성할 총알의 원본 프리팹
    public float spawnRateMin = 0.1f; // 최소 생성 주기
    public float spawnRateMax = 3f; // 최대 생성 주기
    public bool useTargeting = false; // 타겟을 향해 발사할지 여부 (false: 기본 발사, true: 타겟팅)

    public Transform target; // 타겟
    private float spawnRate; // 생성 주기
    private float timeAfterSpawn; // 최근 생성 시점에서 지난 시간
    public int currentBulletIndex = 0;

    void Start()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);

        if (bulletPrefabs == null || bulletPrefabs.Length == 0)
        {
            AssignDefaultBulletPrefab();
            useTargeting = true;
        }

        // 타겟팅을 사용할 경우, PlayerController를 찾아서 타겟으로 설정
        if (useTargeting)
        {
            target = FindObjectOfType<PlayerController>()?.transform;
        }
    }

    void Update()
    {

        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;

            GameObject bullet = Instantiate(bulletPrefabs[currentBulletIndex], transform.position, transform.rotation);

            // 타겟팅 여부에 따라 방향 설정
            if (useTargeting == true)
            {
                bullet.transform.LookAt(target); // 타겟을 바라보도록 설정
            }
            else
            {
                bullet.transform.forward = transform.forward;
                // transform.forward 방향으로 발사되도록 회전 설정
            }

            spawnRate = Random.Range(spawnRateMin, spawnRateMax);

        }
    }

    void AssignDefaultBulletPrefab()
    {
        // Resources/Prefabs/Bullet.prefab 경로에서 기본 총알 프리팹을 로드
        GameObject defaultBullet = Resources.Load<GameObject>("Prefabs/Bullet");

        if (defaultBullet != null)
        {
            bulletPrefabs = new GameObject[] { defaultBullet };  // 기본 총알을 배열에 추가
            currentBulletIndex = 0;
            Debug.Log($"{gameObject.name}: 기본 총알이 설정되었습니다.");
        }
        else
        {
            Debug.LogError("기본 총알 프리팹을 찾을 수 없습니다! Resources/Prefabs/Bullet.prefab을 추가하세요.");
        }
    }

}
