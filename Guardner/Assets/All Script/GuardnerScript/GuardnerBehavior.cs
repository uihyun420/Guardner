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

    private Animator animator;

    private float attackTimer;
    public float duration;
    public int coolTime;

    private LineRenderer lineRenderer;
    private bool isRangeVisible = false;

    public System.Action<GuardnerBehavior> OnGuardnerClicked; // 클릭했을때 이벤트 전달
    private Camera mainCamera;

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

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        mainCamera = Camera.main;
        if(mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        SetupLinRenderer();
    }
    private void SetupLinRenderer()
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red; 
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        //lineRenderer.useWorldSpace = false;
        lineRenderer.useWorldSpace = true; // 월드 좌표 사용

        lineRenderer.enabled = false;
        lineRenderer.sortingOrder = 10; // UI 위에 표시
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

        HandleTouchInput();

        if(isRangeVisible)
        {
            DrawAttackRange();
        }
    }

    private void HandleTouchInput()
    {
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                CheckTouch(touch.position);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            CheckTouch(Input.mousePosition);

        }
    }

    private void CheckTouch(Vector2 screenPosition)
    {
        if (mainCamera == null) return;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;

        if(IsPointInGuardner(worldPosition))
        {
            OnGuardnerTouched();
        }
    }

    private bool IsPointInGuardner(Vector3 worldPosition)
    {
        // CapsuleCollider2D의 범위 내에 포인트가 있는지 확인
        Vector2 center = (Vector2)transform.position + collider.offset;
        Vector2 size = collider.size;

        // 간단한 사각형 범위 검사 (실제로는 더 정확한 캡슐 검사가 필요할 수 있음)
        Bounds bounds = new Bounds(center, size);
        return bounds.Contains(worldPosition);
    }

    private void OnGuardnerTouched()
    {
        Debug.Log($"가드너 터치됨: {guardnerData.Name}");

        // 이벤트 발생
        OnGuardnerClicked?.Invoke(this);

        // 범위 표시 토글
        ToggleRangeDisplay();
    }

    public void ToggleRangeDisplay()
    {
        isRangeVisible = !isRangeVisible;
        ShowAttackRange(isRangeVisible);
    }

    public void ShowAttackRange(bool show)
    {
        isRangeVisible = show;
        lineRenderer.enabled = show;
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

        Vector3 center = transform.position; // 월드 좌표 기준

        for (int i = 0; i <= segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius + center.x;
            float y = Mathf.Sin(angle) * radius + center.y;
            lineRenderer.SetPosition(i, new Vector3(x, y, center.z));
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
