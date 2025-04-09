using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed; // 이동 속력

    public float baseSpeed = 8f;
    public Vector3 baseScale;

    public Gear gear;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //  씬 변경 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }

        baseScale = transform.localScale;
    }

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        gear = GetComponentInChildren<Gear>();  // 
    }


    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 수평과 수직 축 입력 값을 감지하여 저장
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // 실제 이동 속도를 입력 값과 이동 속력을 통해 결정
        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;

        // Vector3 속도를 (xSpeed, 0, zSpeed)으로 생성
        Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);
        // 리지드바디의 속도에 newVelocity를 할당
        playerRigidbody.velocity = newVelocity;
    }

    public void TakeDamage()
    {
        if (gear == null)  // gear가 null이면 할당 시도
        {
            gear = GetComponentInChildren<Gear>();
        }

        if (gear != null && gear.hasShield)
        {
            Debug.Log("Shield Absorbed the Damage!");
            gear.hasShield = false;

            Transform shieldTransform = transform.Find("ShieldEffect(Clone)"); // ShieldEffect(Clone) 이름으로 찾기

            if (shieldTransform != null)
            {
                Destroy(shieldTransform.gameObject); // 해당 이펙트를 삭제
            }
        }
        else
        {
            GameManager.instance.GameOver();
        }
    }


}