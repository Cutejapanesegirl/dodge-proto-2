using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;
    public float timer;
    public float cooltime = 10f;
    public bool hasShield = false;
    private int cooltimeReductionCount = 0;

    public GameObject ShieldEffect;  // 쉴드 이펙트 프리팹 (Inspector에서 설정)
    public GameObject activeShield;
    public GameObject BulletPrefab;
    public GameObject bulletSpawnerPrefab;

    public PlayerController player;

    public Bullet bulletScript;
    public BulletSpawner bulletSpawner;

    public float originalRate; // 원래 적용된 수치 저장용 변수
    private bool isEffectRemoved = false;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        if (type == ItemData.ItemType.Item)  // 쉴드 아이템일 때만 타이머 동작
        {
            timer += Time.deltaTime;

            if (timer >= cooltime)
            {
                timer = 0f;
                Shield();
                Debug.Log("쉴드가 재 생성됨!");  // 재생성된 쉴드
            }
        }
    }


    public void Init(ItemData data)
    {
        // Basic
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        // Property
        type = data.itemType;
        rate = data.damages[0];

        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();

    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
            case ItemData.ItemType.Size:
                SizeDown();
                break;
            case ItemData.ItemType.Item:
                Shield();
                Debug.Log("처음 쉴드가 생성됨!");

                //  쿨타임을 최대 5번까지만 감소시키기
                if (cooltimeReductionCount < 5)
                {
                    cooltime = cooltime - rate;
                    cooltimeReductionCount++;  // 감소 횟수 증가
                    Debug.Log("쿨타임 감소 적용! 현재 횟수: " + cooltimeReductionCount);
                }

                timer = 0f;
                break;
        }
    }

    public void RemoveGearEffects()
    {
        if (isEffectRemoved) return; // 이미 효과가 제거된 상태라면 중복 실행 방지

        originalRate = rate; // 현재 Gear 효과 값을 저장
        isEffectRemoved = true;

        switch (type)
        {
            case ItemData.ItemType.Shoe:
                GameManager.instance.player.speed = GameManager.instance.player.baseSpeed;
                break;
            case ItemData.ItemType.Size:
                GameManager.instance.player.transform.localScale = GameManager.instance.player.baseScale;
                break;
            case ItemData.ItemType.Item:
                if (activeShield != null)
                    Destroy(activeShield);
                break;
        }

        Debug.Log("Gear 효과 제거 완료! 원래 rate: " + originalRate);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Boss8") // 특정 씬일 때 효과 제거
        {
            RemoveGearEffects();
            return;
        }

        if (isEffectRemoved) // 이전 씬에서 효과를 제거한 경우
        {
            isEffectRemoved = false; // 다시 활성화할 것이므로 초기화
            rate = originalRate; // 저장된 원래 값 복구
            ApplyGear(); // Gear 효과 다시 적용
            Debug.Log("Gear 효과 복구됨! rate: " + rate);
        }
    }

    void SpeedUp()
    {
        float speed = 8;
        GameManager.instance.player.speed = speed + speed * rate;
    }

    void SizeDown()
    {
        GameManager.instance.player.transform.localScale -= GameManager.instance.player.transform.localScale * rate;
    }

    void Shield()
    {
        player = GameManager.instance.player;

        if (player != null)
        {
            player.gear.hasShield = true;

            if (activeShield != null)
            {
                Destroy(activeShield);
            }

            // 쉴드 프리팹 생성
            activeShield = Instantiate(ShieldEffect, player.transform.position, Quaternion.identity, player.transform);
        }
    }
}





