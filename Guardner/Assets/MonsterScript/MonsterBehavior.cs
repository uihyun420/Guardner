using UnityEngine;

public class MonsterBehavior : MonoBehaviour, IDamageable
{
    public readonly string isDead = "isDead";
    public readonly string attack = "ATTACK";
    public readonly string Door = "Door"; // 문 레이어이름
    public DoorBehavior door;

    private new CapsuleCollider2D collider;
    private Animator animator;
    public bool IsDead = false;
    public MonsterData monsterData;

    public LayerMask layer;

    private float attackInterval = 1f;
    private float attackTimer;
    public bool isTouchingDoor = false;

    private int hp;
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
        attackTimer += Time.deltaTime;

        if (isTouchingDoor && door != null && !door.isBreak)
        {
            if (attackTimer >= attackInterval)
            {
                //Attack();
                door.Ondamage(attackPower);
                Debug.Log($"문이 받은 데미지{attackPower}");
                attackTimer = 0;
            }
        }
        else
        {
            //animator.SetBool(attack, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DoorBehavior doorBehavior = collision.GetComponent<DoorBehavior>();
        if (doorBehavior != null && !doorBehavior.isBreak)
        {
            door = doorBehavior;
            isTouchingDoor = true;
        }
    }

    private void Attack()
    {
        animator.SetBool(attack, true);
    }

    public void Die()
    {
        IsDead = true;
        animator.SetTrigger(isDead);
        Destroy(gameObject, 0.5f);
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
