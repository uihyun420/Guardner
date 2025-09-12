using UnityEngine;

public class MonsterBehavior : MonoBehaviour, IDamageable
{
    public readonly string isDead = "isDead";

    private CapsuleCollider2D collider;
    private Animator animator;
    public bool IsDead = false;
    public MonsterData monsterData;

    public LayerMask layer;

    private int hp = 350; // 테스트

    private void Init(MonsterData data)
    {
        data = monsterData;
        hp = monsterData.HP;
    }


    private void Awake()
    {
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    public void Die()
    {
        IsDead = true;
        animator.SetTrigger(isDead);
        Debug.Log("몬스터 죽음");        
        Destroy(gameObject, 2f);
    }

    public void Ondamage(int damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            hp = 0;
            Die();
        }
    }
}
