using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Rotator : MonoBehaviour
{

    public float rotationSpeed = 60f;

    [Header("Scene Setting")]
    public bool DirectionChange = false;
    public float changeInterval = 5f;
    public int gametimer;

    private float direction = 1f;
    private float timer;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때 이벤트 추가
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; //  씬이 삭제될 때 이벤트 제거 (메모리 누수 방지)
    }

    // 특정 씬이 로드될 때 DirectionChange 설정
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) // 특정 씬(예: 1번 씬)에서 활성화
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }

        Debug.Log($"씬 변경됨: {scene.name} (Index: {scene.buildIndex})");
    }

    void Update()
    {
        if (DirectionChange)
        {
            Boss();
        }
        else
        {
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
