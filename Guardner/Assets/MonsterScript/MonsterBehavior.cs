using UnityEngine;

public class MonsterBehavior : MonoBehaviour, IDamageable
{
    private readonly string isDead = "Die";

    private CapsuleCollider2D collider;
    private Animator animator;
    public MonsterData monsterData;

    public LayerMask layer;

    private float hp;

    private void Init()
    {
        hp = monsterData.HP;
    }


    private void Awake()
    {
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    public void Die()
    {
        animator.SetTrigger(isDead);
        Destroy(gameObject, 2f);
    }

    public void Ondamage(int damage, Vector2 hitPoint, Vector2 hitNormal)
    {
        hp -= damage;
        if(hp <= 0)
        {
            hp = 0;
            Die();
        }
    }
}
