using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        if (monster.isStunned)
            return;
        MovePath();
    }

    public void MovePath()
    {
        if(wayPointCount >= wayPoint.Length)
        {
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, wayPoint[wayPointCount].transform.position, speed * Time.deltaTime);
        if(transform.position == wayPoint[wayPointCount].transform.position)
        {
            wayPointCount++;
        }
        //if(wayPointCount == wayPoint.Length)
        //{
        //    wayPointCount = 0;
        //}
    }

}
