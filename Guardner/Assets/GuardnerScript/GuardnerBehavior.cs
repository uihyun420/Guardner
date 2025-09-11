using Unity.VisualScripting;
using UnityEngine;

public class GuardnerBehavior : MonoBehaviour, IDamageable
{
    public readonly string attack = "ATTACK";
    public readonly string isDead = "isDead";
    public GuardnerData guardnerData { get; private set; }

    private float hp;
    private int level;
    private float aps;
    private int attackRange;

    private Animator animator;

    private float attackTimer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        aps = 1f; // 테스트용
    }

    public void Init(GuardnerData data)
    {
        guardnerData = data; // 복사

        attackTimer = 0;
        level = guardnerData.Level;
        hp = guardnerData.HP;
        //aps = guardnerData.APS;        
        attackRange = guardnerData.AttackRange;
    }

    private void OnEnable()
    {
        Init(guardnerData);
    }


    private void Update()
    {
        attackTimer += Time.deltaTime;
        float attackInterval = aps > 0 ? 1f / aps : 1f;
        if (attackTimer >= attackInterval)
        {
            Attack();
            attackTimer = 0;
            Debug.Log($"{attackInterval}초마다 공격");
        }
    }

    private void Attack()
    {
        animator.SetBool(attack, true);
        animator.speed = aps;
    }

    public void Ondamage(float damage, Vector2 hitPoint, Vector2 hitNormal)
    {
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }
    public void Die()
    {
        animator.SetTrigger(isDead);
        animator.speed = 1f;
        Destroy(gameObject, 2f);
    }
}
