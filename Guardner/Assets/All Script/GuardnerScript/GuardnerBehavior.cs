using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GuardnerBehavior : MonoBehaviour
{
    public readonly string attack = "ATTACK";
    public readonly string isDead = "isDead";
    public readonly string monster = "Monster"; // ���� ���̾� 

    public GuardnerData guardnerData { get; private set; }
    public MonsterBehavior Monster;

    private Rigidbody2D rb;
    private CapsuleCollider2D collider;

    public int attackPower;
    public float aps;
    private int attackRange;
    private Animator animator;
    private float attackTimer;


    public float duration;
    public int coolTime;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();   

        aps = 1f; // �׽�Ʈ��
        attackRange = 2; // �׽�Ʈ�� 
        attackPower = 35; // �׽�Ʈ��
        attackTimer = 0;
        duration = 2; // �׽�Ʈ
        coolTime = 5;
    }


    private void OnEnable()
    {
    }


    private void Update()
    {
        collider.size = new Vector2(attackRange, attackRange); // �׽�Ʈ 

        Monster = SearchMonster();

        attackTimer += Time.deltaTime;
        float attackInterval = aps > 0 ? 1f / aps : 1f;

        if (Monster != null && !Monster.IsDead)
        {
            if (attackTimer >= attackInterval)
            {
                Attack();
                Monster.Ondamage(attackPower);
                //Debug.Log($"{attackInterval}�ʸ��� ����");
                //Debug.Log($"{attackPower}�� ������");
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
        float angle = collider.transform.eulerAngles.z; // ȸ�� ���� 
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
