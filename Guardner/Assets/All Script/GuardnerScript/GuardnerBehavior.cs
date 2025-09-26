using System.Collections;
using UnityEngine;

public class GuardnerBehavior : MonoBehaviour
{
    public readonly string attack = "ATTACK";
    public readonly string isDead = "isDead";
    public readonly string monster = "Monster"; // ���� ���̾� 

    public GuardnerData guardnerData;
    public MonsterBehavior Monster;

    private Rigidbody2D rb;
    private CapsuleCollider2D collider;

    public bool hasCriticalBuff = false; // ũ��Ƽ�� ���� ���� 
    public float buffCriticalChance = 0f; // ũ��Ƽ�� Ȯ��
    public float buffCriticalDamage = 0f; // ũ��Ƽ�� ������

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

    private Animator animator;

    private float attackTimer;
    public float duration;
    public int coolTime;

    private LineRenderer lineRenderer;
    private bool isRangeVisible = false;

    private Camera mainCamera;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        CapsuleCollider2D touchCollider = gameObject.AddComponent<CapsuleCollider2D>();
        touchCollider.isTrigger = false; // ��ġ ������
        touchCollider.size = new Vector2(0.5f, 0.5f); // ��ġ ���� ũ��

        collider.isTrigger = true; // ���� ������
        attackTimer = 0;

        if(rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // ������Ʈ ���� Ȱ��ȭ
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.enabled = true;

        mainCamera = Camera.main;
        if(mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        SetupLinRenderer();
    }
    private void SetupLinRenderer()
    {
        var mat = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = mat;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f; // �� �β���
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = 100; // �� ���� ��
        lineRenderer.sortingLayerName = "Default";
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

       // HandleTouchInput();

        if(isRangeVisible)
        {
            DrawAttackRange();
        }
    }
    public void ToggleRangeDisplay()
    {
        isRangeVisible = !isRangeVisible;
        Debug.Log($"���� ���: {isRangeVisible}, attackRange: {attackRange}");
        ShowAttackRange(isRangeVisible);
    }

    public void ShowAttackRange(bool show)
    {
        isRangeVisible = show;
        lineRenderer.enabled = show;
        Debug.Log($"LineRenderer Ȱ��ȭ: {lineRenderer.enabled}"); // ����� �α� �߰�
        if (show)
        {
            DrawAttackRange();
        }
    }

    private void DrawAttackRange()
    {
        int segments = 64;
        float radius = attackRange * 0.5f;
        lineRenderer.positionCount = segments + 1;

        Vector3 center = transform.position; // ���� ��ǥ ����

        for (int i = 0; i <= segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius + center.x;
            float y = Mathf.Sin(angle) * radius + center.y;
            //lineRenderer.SetPosition(i, new Vector3(x, y, center.z));
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }


    private void Attack()
    {
        animator.SetBool(attack, true);
        animator.speed = aps;

        // �Ϲ� ����
        Monster.Ondamage(attackPower);

        // ũ��Ƽ�� ������ ������ �߰� ������ üũ
        if (hasCriticalBuff && Random.value < buffCriticalChance)
        {
            int criticalDamage = Mathf.RoundToInt(attackPower * buffCriticalDamage);
            Monster.Ondamage(criticalDamage);
            Debug.Log($"ũ��Ƽ��! �߰� ������: {criticalDamage}");
        }
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
        StopCoroutine("CoAttackSpeedBoost"); // ����ð� ������ �ٽ� ���ƿ��� 
        StartCoroutine(CoAttackSpeedBoost(amount, duration));
    }
    public IEnumerator CoAttackSpeedBoost(float amount, float duration) 
    {
        aps += amount;
        yield return new WaitForSeconds(duration);
        aps -= amount; // ����ð� ������ �ٽ� ���ƿ��� 
    }

    //public IEnumerator CoCleanDebuff()

    //public void CleanDebuff(float duration)
    //{
    //    StartCoroutine()
    //}
}
