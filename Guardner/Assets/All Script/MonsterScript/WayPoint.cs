using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class WayPoint : MonoBehaviour
{    
    public Transform[] wayPoint;
    public int wayPointCount = 0;

    public MonsterBehavior monster;
    private SpriteRenderer spriteRenderer;
    private bool isDefaultFlipX = false;

    public bool isMirrored = false; // 스프라이트 미러링

    private void Start()
    {
        transform.position = wayPoint[wayPointCount].transform.position;
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
        if(wayPointCount >= wayPoint.Length)
        {
            return;
        }

        Vector2 targetPos = wayPoint[wayPointCount].transform.position;
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
