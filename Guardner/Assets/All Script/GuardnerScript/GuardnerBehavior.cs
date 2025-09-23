using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GuardnerBehavior : MonoBehaviour
{
    public readonly string attack = "ATTACK";
    public readonly string isDead = "isDead";
    public readonly string monster = "Monster"; // 몬스터 레이어 

    public GuardnerData guardnerData;
    public MonsterBehavior Monster;

    private Rigidbody2D rb;
    private CapsuleCollider2D collider;

    public bool hasCriticalBuff = false; // 크리티컬 공격 여부 
    public float buffCriticalChance = 0f; // 크리티컬 확률
    public float buffCriticalDamage = 0f; // 크리티컬 데미지

    private string name;
    private string egName;
    public int id;
    private int idDivide;
    public GuardnerTypes role;
    public int attackPower;
    public float aps;
    private float dps;
    private int gateHp;
    public int attackRange;
    private int summonGold;
    private int sellingGold;
    public GuardnerGrade rarity;
    private int maxLevel;
    public int skillId;
    private string reference;
    private int guardnerDrawId;

    //private int level;

    private Animator animator;

    private float attackTimer;
    public float duration;
    public int coolTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        collider.isTrigger = true;
        attackTimer = 0;

        if(rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
    public void Init(GuardnerData data)
    {
        guardnerData = data;

        name = guardnerData.Name;
        egName = guardnerData.EGName;
        id = guardnerData.Id;
        idDivide = guardnerData.IdDivide;
        role = guardnerData.Role;
        attackPower = guardnerData.AttackPower;
        aps = guardnerData.APS;
        dps = guardnerData.DPS;
        attackRange = guardnerData.AttackRange;
        summonGold = guardnerData.SummonGold;
        sellingGold = guardnerData.SellingGold;
        rarity = guardnerData.Rarity;
        maxLevel = guardnerData.MaxUPLevel;
        skillId = guardnerData.SkillID;
        reference = guardnerData.Reference;
        guardnerDrawId = guardnerData.GuardenerDrawId;
    }

    private void Update()
    {
        collider.size = new Vector2(attackRange, attackRange);

        Monster = SearchMonster();

        attackTimer += Time.deltaTime;
        float attackInterval = aps > 0 ? 1f / aps : 1f;

        if (Monster != null && !Monster.IsDead)
        {
            if (attackTimer >= attackInterval)
            {
                Attack();
                attackTimer = 0;
            }
        }
        else
        {
            animator.SetBool(attack, false);
        }
    }

    private void Attack()
    {
        animator.SetBool(attack, true);
        animator.speed = aps;

        // 일반 공격
        Monster.Ondamage(attackPower);

        // 크리티컬 버프가 있으면 추가 데미지 체크
        if (hasCriticalBuff && Random.value < buffCriticalChance)
        {
            int criticalDamage = Mathf.RoundToInt(attackPower * buffCriticalDamage);
            Monster.Ondamage(criticalDamage);
            Debug.Log($"크리티컬! 추가 데미지: {criticalDamage}");
        }
    }

    private MonsterBehavior SearchMonster()
    {
        Vector2 center = (Vector2)collider.transform.position + collider.offset;
        float width = collider.size.x;
        float height = collider.size.y;
        Vector2 size = new Vector2(width, height);

        CapsuleDirection2D direction = collider.direction;
        float angle = collider.transform.eulerAngles.z; // 회전 각도 
        var layer = LayerMask.GetMask(monster);

        Collider2D[] hits = Physics2D.OverlapCapsuleAll(center, size, direction, angle, layer);

        MonsterBehavior closestMonster = null;
        float minDist = float.MaxValue;

        foreach (var mon in hits)
        {
            var monster = mon.GetComponent<MonsterBehavior>();
            if (monster != null && !monster.IsDead)
            {
                float dist = Vector2.Distance(transform.position, monster.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestMonster = monster;
                }
            }
        }
        return closestMonster;
    }

    public void AttackPowerBoost(float amount, float duration)
    {
        StopCoroutine("CoAttackPowerBoost");
        StartCoroutine(CoAttackPowerBoost(amount, duration));
    }
    private IEnumerator CoAttackPowerBoost(float amount, float duration)
    {
        attackPower += (int)amount;
        yield return new WaitForSeconds(duration);
        attackPower -= (int)amount;
    }

    public void AttackSpeedBoost(float amount, float duration)
    {
        StopCoroutine("CoAttackSpeedBoost"); // 적용시간 지나면 다시 돌아오게 
        StartCoroutine(CoAttackSpeedBoost(amount, duration));
    }
    public IEnumerator CoAttackSpeedBoost(float amount, float duration) 
    {
        aps += amount;
        yield return new WaitForSeconds(duration);
        aps -= amount; // 적용시간 지나면 다시 돌아오게 
    }

    //public IEnumerator CoCleanDebuff()

    //public void CleanDebuff(float duration)
    //{
    //    StartCoroutine()
    //}
}
