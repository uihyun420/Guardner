using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private float speed = 1f;
    public Transform[] wayPoint;
    public int wayPointCount = 0;

    private MonsterBehavior monster;

    private void Start()
    {
        transform.position = wayPoint[wayPointCount].transform.position;
        monster = GetComponent<MonsterBehavior>();
    }

    private void Update()
    {
        if (!monster.isStunned)
        {
            MovePath();
        }
        else
        {
            if (monster.isStunned)
            {
                Debug.Log("È°¼ºÈ­µÊ");
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
    }
}
