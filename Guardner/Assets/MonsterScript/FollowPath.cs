using UnityEngine;
using Unity.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Transform target;
    private Transform[] wayPoints;
    private float waitTime; // wayPoint ���� �� ���ð� 
    private float unitPerSecond = 1; // 1�ʿ� �����̴� �Ÿ� 
    private bool isPlayOnAwake = true; // �ڵ� ���� ����
    private bool isLoop = true; // ������ ��ο��� ó�� ��η� �̾������� 


    private int wayPointCount; // �̵� ������ wayPoint ���� 
    private int currentIndex = 0; // ���� wayPoint �ε���


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


