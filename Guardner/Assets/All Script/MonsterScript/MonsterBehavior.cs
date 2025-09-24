using UnityEngine;
using System.Collections;

public class MonsterBehavior : MonoBehaviour, IDamageable
{
    public readonly string isDead = "isDead";
    public readonly string attack = "ATTACK";
    public readonly string Door = "Door"; // �� ���̾��̸�
    public DoorBehavior door;
    private BattleUi battleUi;
    private Rigidbody2D rb;
    private CapsuleCollider2D collider;
    private Animator animator;
    public bool IsDead = false;
    public MonsterData monsterData;

    public LayerMask layer;

    public float attackInterval = 1f;
    private float attackTimer;
    public bool isTouchingDoor = false;

    private int hp;
    public float moveSpeed;
    public int attackPower;
    private string monsterName;
    private Type monsterType;
    private int skillId;
    private int overLapping;
    private string reference;

    public HpBar hpBar;
    public bool isStunned { get; set; } = false;
    private float stunTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public void SetBattleUi(BattleUi ui)
    {
        battleUi = ui;
    }

    public void Init(MonsterData data)
    {
        monsterData = data;
        hp = monsterData.HP;
        // moveSpeed = monsterData.BaseMoveSpeed;
        moveSpeed = monsterData.BaseMoveSpeed <= 0 ? 1.0f : monsterData.BaseMoveSpeed; // �׽�Ʈ
        attackPower = monsterData.BaseAttackPower;
        monsterName = monsterData.Name;
        monsterType = monsterData.Type;
        skillId = monsterData.SkillId;
        overLapping = monsterData.OverLapping;
        reference = monsterData.Reference;

        hpBar.SetMaxHealth(hp);

        //Debug.Log($"���� ����: {monsterName}, HP: {hp}, Speed: {moveSpeed}, Attack: {attackPower}");
    }
    private void Awake()
    {
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // �ڽĿ��� SpriteRenderer�� ã�Ƽ� �Ҵ�
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (isTouchingDoor && door != null && !door.isBreak)
        {
            if (attackTimer >= attackInterval)
            {
                Attack();
                door.Ondamage(attackPower);
                attackTimer = 0;
            }
        }
        else
        {
           animator.SetBool(attack, false);
        }

        if(isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer < 0f)
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
        StageManager stageManager = FindObjectOfType<StageManager>();
        MonsterSpawner monsterSpawner = FindObjectOfType<MonsterSpawner>();
        if (stageManager != null)
        {
            stageManager.enemiesRemaining--;
            Debug.Log($"���� ��� - ���� ��: {stageManager.enemiesRemaining}");
        }

        if(monsterSpawner != null)
        {
            monsterSpawner.RemoveMonster(this);
        }

        Destroy(gameObject, 0.25f);
    }

    public void Ondamage(int damage)
    {
        hp -= damage;
        StartCoroutine(CoHitEffect());
        hpBar.SetHealth(hp);
        if(hp <= 0)
        {
            hp = 0;
            Die();
            battleUi.AddGold(monsterData.RewardGold);
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


    private IEnumerator CoHitEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    public void SetSortingOrder(int order)
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }



    // ���� ���ǵ� ����� 
    public void SpeedDebuff(float moveSpeedPercent, float attackSpeedPercent, float duration)
    {
        StopCoroutine("CoSpeedDebuff");
        StartCoroutine(CoSpeedDebuff(moveSpeedPercent, attackSpeedPercent, duration));
    }

    public IEnumerator CoSpeedDebuff(float moveSpeedPercent, float attackSpeedPercent, float duration)
    {
        float originalMoveSpeed = moveSpeed;
        float originalAttackInterval = attackInterval;

        // �̵��ӵ� ����
        moveSpeed *= (1f - moveSpeedPercent);
        // ���ݼӵ� ����(���ݰ��� ����)
        attackInterval *= (1f + attackSpeedPercent);

        yield return new WaitForSeconds(duration);

        // ���󺹱� (���� ������ ���� ����)
        moveSpeed = originalMoveSpeed;
        attackInterval = originalAttackInterval;
    }
}
