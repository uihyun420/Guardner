using UnityEngine;
using System;

public class DoorBehavior : MonoBehaviour, IDamageable
{
    public static event Action OnDoorDestroyed; // �̺�Ʈ�� �߻�

    public LayerMask layer;

    private int hp = 500;  // �׽�Ʈ 
    private BoxCollider2D boxCollider2D;
    public bool isBreak = false;


    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        isBreak = false;
    }

    private void OnEnable()
    {
    }

    public void Die()
    {
        isBreak = true;
        Destroy(gameObject, 1f);

        OnDoorDestroyed?.Invoke(); 
    }

    public void Ondamage(int damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            hp = 0;
            Die();
            Debug.Log("�й�");
        }
    }
}
