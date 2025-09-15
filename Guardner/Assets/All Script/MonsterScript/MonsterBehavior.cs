using UnityEngine;

public class MonsterBehavior : MonoBehaviour, IDamageable
{
    public readonly string isDead = "isDead";
    public readonly string attack = "ATTACK";
    public readonly string Door = "Door"; // �� ���̾��̸�
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
    public int attackPower;
    private string monsterName;
    private Type monsterType;
    private int skillId;
    private int overLapping;
    private string reference;


    public HpBar hpBar;


    private bool isStunned = false;
    private float stunTimer = 0f;

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

        hpBar.SetMaxHealth(hp);

        Debug.Log($"���� ����: {monsterName}, HP: {hp}, Speed: {moveSpeed}, Attack: {attackPower}");
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
                Debug.Log($"���� ���� ������{attackPower}");
                attackTimer = 0;
            }
        }
        else
        {
            //animator.SetBool(attack, false);
        }

        if(isStunned)
        {
            stunTimer -= Time.deltaTime;
            if(stunTimer <= 0f)
            {
                isStunned = false;
                stunTimer = 0f;
            }
            return;
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
        Destroy(gameObject, 0.25f);
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

    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }

    public void ReflectDamage(float amount)
    {
        Ondamage(Mathf.RoundToInt(amount));
    }


}
