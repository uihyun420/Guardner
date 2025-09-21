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

    private void Start()
    {
        transform.position = wayPoint[wayPointCount].transform.position;
        spriteRenderer = GetComponent < SpriteRenderer>();
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
        transform.position = Vector2.MoveTowards(transform.position, wayPoint[wayPointCount].transform.position, monster.moveSpeed * Time.deltaTime);
        if (transform.position == wayPoint[wayPointCount].transform.position)
        {
            wayPointCount++;
        }
        if(wayPointCount == 1 || wayPointCount == 4 || wayPointCount == 5 || wayPointCount ==7 || wayPointCount == 8)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
