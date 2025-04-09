using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Rotator1 : MonoBehaviour
{
    public static Rotator1 instance;
    public float rotationSpeed = 60f;

    [Header("Scene Setting")]
    public bool DirectionChange = false;
    public float changeInterval = 5f;

    private float direction = 1f;
    private float timer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //  씬 이동 후에도 유지

        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때 이벤트 추가
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; //  씬이 삭제될 때 이벤트 제거 (메모리 누수 방지)
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 회전 초기화
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (DirectionChange)
                Boss();
            else
                Rotate();
        }
    }

    void Rotate()
    {
        //Rotate(float xAngle, float yAngle, float zAngle)
        transform.Rotate(xAngle: 0f, yAngle: Time.deltaTime * rotationSpeed, zAngle: 0f);
    }

    void Boss()
    {
        // 객체를 y 축을 기준으로 회전
        transform.Rotate(0f, Time.deltaTime * rotationSpeed * direction, 0f);

        // 타이머 증가
        timer += Time.deltaTime;

        // 5초마다 방향 변경
        if (timer >= changeInterval)
        {
            direction *= -1; // 방향을 반대로 전환
            timer = 0f; // 타이머 초기화
        }
    }

}