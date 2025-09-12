using UnityEngine;
using Unity.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Transform target;
    private Transform[] wayPoints;
    private float waitTime; // wayPoint 도착 후 대기시간 
    private float unitPerSecond = 1; // 1초에 움직이는 거리 
    private bool isPlayOnAwake = true; // 자동 시작 여부
    private bool isLoop = true; // 마지막 경로에서 처음 경로로 이어지는지 


    private int wayPointCount; // 이동 가능한 wayPoint 개수 
    private int currentIndex = 0; // 현재 wayPoint 인덱스


    private void Awake()
    {
        wayPointCount = wayPoints.Length;

        if(target == null)
        {
            target = transform;
        }
        if(isPlayOnAwake == true)
        {
            Play();
        }
    }

    private void Play()
    {
        StartCoroutine
    }
}


