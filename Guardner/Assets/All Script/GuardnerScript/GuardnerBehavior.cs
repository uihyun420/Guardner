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

    public string name;
    public string egName;
    public int id;
    public int idDivide;
    public int level;
    public GuardnerTypes role;
    public int attackPower;
    public float aps;
    public float dps;
    public int gateHp;
    public int attackRange;
    public int summonGold;
    public int sellingGold;
    public GuardnerGrade rarity;
    public int maxLevel;
    public int skillId;
    public int lvStatId;
    public string reference; 

    private Animator animator;

    private float attackTimer;
    public float duration;
    public int coolTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();   
        attackTimer = 0;
    }
    public void Init(GuardnerData data)
    {
        guardnerData = data;

        name = guardnerData.Name;
        egName = guardnerData.EGName;
        id = guardnerData.Id;
        idDivide = guardnerData.IdDivide;
        level = guardnerData.Level;
        role = guardnerData.Role;
        attackPower = guardnerData.AttackPower;
        aps = guardnerData.APS;
        dps = guardnerData.DPS;
        gateHp = guardnerData.GateHP;
        attackRange = guardnerData.AttackRange;
        summonGold = guardnerData.SummonGold;
        sellingGold = guardnerData.SellingGold;
        rarity = guardnerData.Rarity;
        maxLevel = guardnerData.MaxUPLevel;
        skillId = guardnerData.SkillID;
        lvStatId = guardnerData.LvStatId;
        reference = guardnerData.Reference;
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
                Monster.Ondamage(attackPower);
                //Debug.Log($"{attackInterval}초마다 공격");
                //Debug.Log($"{attackPower}의 데미지");
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
        foreach(var mon in hits)
        {
            var monster = mon.GetComponent<MonsterBehavior>();
            if(monster !=null)
            {
                return monster;
            }
        }
        return null;
    }

    public void AttackPowerBoost(float amount, float duration)
    {
        StartCoroutine(CoAttackPowerBoost(amount, duration));
    }
    private IEnumerator CoAttackPowerBoost(float amount, float duration)
    {
        int att = attackPower;
        attackPower += Mathf.RoundToInt(att * amount);
        yield return new WaitForSeconds(duration);
        attackPower = att;
    }

    public IEnumerator CoAttackSpeedBoost(float amount, float duration)
    {
        float originalAps = aps;
        aps += aps * amount;
        yield return new WaitForSeconds(duration);
        aps = originalAps;
    }
    public void AttackSpeedBoost(float amount, float duration)
    {
        StartCoroutine(CoAttackSpeedBoost(amount, duration));
    }
}
