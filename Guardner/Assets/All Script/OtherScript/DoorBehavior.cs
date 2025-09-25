using UnityEngine;
using System;

public class DoorBehavior : MonoBehaviour, IDamageable
{
    public static event Action OnDoorDestroyed; // 이벤트가 발생

    public LayerMask layer;
    public HpBar hpBar;

    private int hp = 100;  // 테스트 
    private BoxCollider2D boxCollider2D;
    public bool isBreak = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        hpBar.SetMaxHealth(hp);
        isBreak = false;
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
        hpBar.SetHealth(hp);
        if(hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    public void Init()
    {
        hp = 100;
        hpBar.SetHealth(hp);
        hpBar.SetMaxHealth(hp);
    }
}
