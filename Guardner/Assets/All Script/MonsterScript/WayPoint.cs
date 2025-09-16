using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private float speed = 1f;
    public Transform[] wayPoint;
    public int wayPointCount = 0;

    public MonsterBehavior monster;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        transform.position = wayPoint[wayPointCount].transform.position;
        //monster = GetComponent<MonsterBehavior>();
        spriteRenderer = GetComponent < SpriteRenderer>();
    }

    private void Update()
    {
        if (!monster.isStunned)
        {
            speed = 1f;
            MovePath();
        }
        else
        {
            if (monster.isStunned)
            {
                speed = 0;
            }
        }
    }

    public void MovePath()
    {
        if (monster.isStunned) return;
        if(wayPointCount >= wayPoint.Length)
        {
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, wayPoint[wayPointCount].transform.position, speed * Time.deltaTime);
        if (transform.position == wayPoint[wayPointCount].transform.position)
        {
            wayPointCount++;
        }
        if(wayPointCount == 2 || wayPointCount == 5 || wayPointCount ==7 || wayPointCount == 8)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
