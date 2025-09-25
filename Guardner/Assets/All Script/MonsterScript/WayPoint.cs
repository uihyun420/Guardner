using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class WayPoint : MonoBehaviour
{    
    public Transform[] wayPoint;
    public Transform[] rightWayPoint;

    [SerializeField] private Transform[] runTimeWayPoints;
    public int wayPointCount = 0;

    public MonsterBehavior monster;
    private SpriteRenderer spriteRenderer;
    private bool isDefaultFlipX = false;

    public bool isMirrored = false; // ��������Ʈ �̷���
    public bool useRightWayPoints = false; // ������ ��������Ʈ ��� ����

    public void SetWayPointDirection(bool useRight)
    {
        useRightWayPoints = useRight;
        runTimeWayPoints = useRight ? rightWayPoint : wayPoint;
        wayPointCount = 0;
    }

    private void Start()
    {
        // wayPoint�� �������� ���� ��� �⺻������ leftWayPoints ���
        if (runTimeWayPoints == null || runTimeWayPoints.Length == 0)
        {
            runTimeWayPoints = wayPoint;
        }

        if (runTimeWayPoints != null && runTimeWayPoints.Length > 0)
        {
            transform.position = runTimeWayPoints[wayPointCount].transform.position;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (spriteRenderer != null)
        {
            isDefaultFlipX = spriteRenderer.flipX; // �������� �⺻ flipX �� ����
        }
    }

    private void Update()
    {
        if (!monster.isStunned)
        {            
            MovePath();
        }        
    }

    public void MovePath()
    {
        if (monster.isStunned) return;
        if(runTimeWayPoints == null || wayPointCount >= runTimeWayPoints.Length)
        {
            return;
        }

        Vector2 targetPos = runTimeWayPoints[wayPointCount].transform.position;  
        transform.position = Vector2.MoveTowards(transform.position, targetPos, monster.moveSpeed * Time.deltaTime);

        //���⿡ ���� flipx ����
        if(spriteRenderer != null)
        {
            bool movingLeft = targetPos.x < transform.position.x;

            bool shouldFlip = isMirrored ? !movingLeft : movingLeft;
            spriteRenderer.flipX = shouldFlip;
        }

        if (Vector2.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; // ��Ȯ�� �����ֱ�(�ɼ�)
            wayPointCount++;
        }
    }
}
