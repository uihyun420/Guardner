using UnityEngine;

public class MonsterBehavior : MonoBehaviour, IDamageable
{
    public readonly string isDead = "isDead";

    private new CapsuleCollider2D collider;
    private Animator animator;
    public bool IsDead = false;
    public MonsterData monsterData;

    public LayerMask layer;

    private int hp = 350; // 테스트
    private float moveSpeed;
    private int attackPower;
    private string monsterName;
    private Type monsterType;
    private int skillId;
    private int overLapping;
    private string reference;

    public void Init(MonsterData data)
    {
        monsterData = data;
        hp = monsterData.HP;
        moveSpeed = monsterData.BaseMoveSpeed;
        attackPower = monsterData.BaseAttackPower;
        monsterName = monsterData.Name;
        monsterType = monsterData.Type;
        skillId = monsterData.SkillId;
        overLapping = monsterData.OverLapping;
        reference = monsterData.Reference;

        Debug.Log($"몬스터 생성: {monsterName}, HP: {hp}, Speed: {moveSpeed}, Attack: {attackPower}");

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
        Destroy(gameObject, 1f);
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
