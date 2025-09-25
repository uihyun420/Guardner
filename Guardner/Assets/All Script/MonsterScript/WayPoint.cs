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

    public bool isMirrored = false; // 스프라이트 미러링
    public bool useRightWayPoints = false; // 오른쪽 웨이포인트 사용 여부

    public void SetWayPointDirection(bool useRight)
    {
        useRightWayPoints = useRight;
        runTimeWayPoints = useRight ? rightWayPoint : wayPoint;
        wayPointCount = 0;
    }

    private void Start()
    {
        // wayPoint가 설정되지 않은 경우 기본값으로 leftWayPoints 사용
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
            isDefaultFlipX = spriteRenderer.flipX; // 프리팹의 기본 flipX 값 저장
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

        //방향에 따라 flipx 설정
        if(spriteRenderer != null)
        {
            bool movingLeft = targetPos.x < transform.position.x;

            bool shouldFlip = isMirrored ? !movingLeft : movingLeft;
            spriteRenderer.flipX = shouldFlip;
        }

        if (Vector2.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos; // 정확히 맞춰주기(옵션)
            wayPointCount++;
        }
    }
}
