using Unity.VisualScripting;
using UnityEngine;

public class GuardnerBehavior : MonoBehaviour
{
    public readonly string attack = "ATTACK";
    public readonly string isDead = "isDead";
    public readonly string monster = "Monster"; // 몬스터 레이어 
    public GuardnerData guardnerData { get; private set; }
    public MonsterBehavior Monster;
    
    private CapsuleCollider2D collider;

    private float hp;
    private int attackPower;
    private int level;
    private float aps;
    private int attackRange;

    private Animator animator;

    private float attackTimer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        aps = 1f; // 테스트용
        attackRange = 3; // 테스트용 
        attackPower = 35; // 테스트용
    }

    public void Init(GuardnerData data)
    {
        // guardnerData = data; // 데이터 복사

        attackTimer = 0;
        //attackPower = guardnerData.AttackPower;
        //level = guardnerData.Level;
        //hp = guardnerData.HP;
        //aps = guardnerData.APS;        
        //attackRange = guardnerData.AttackRange; 

    }

    private void OnEnable()
    {
        Init(guardnerData);
    }


    private void Update()
    {
        collider.size = new Vector2(attackRange, attackRange); // 테스트 

        Monster = SearchMonster();

        attackTimer += Time.deltaTime;
        float attackInterval = aps > 0 ? 1f / aps : 1f;

        if (Monster != null)
        {
            if (attackTimer >= attackInterval)
            {
                Attack();
                Monster.Ondamage(attackPower, Monster.transform.position, Vector2.right);
                Debug.Log($"{attackInterval}초마다 공격");
                Debug.Log($"{attackPower}의 데미지");
                attackTimer = 0;
            }

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

    //public void Ondamage(int damage, Vector2 hitPoint, Vector2 hitNormal)
    //{
    //    hp -= damage;
    //    if (hp <= 0)
    //    {
    //        hp = 0;
    //        Die();
    //    }
    //}
    //public void Die()
    //{
    //    animator.SetTrigger(isDead);
    //    animator.speed = 1f;
    //    Destroy(gameObject, 2f);
    //}
}
