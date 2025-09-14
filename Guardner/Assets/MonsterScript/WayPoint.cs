using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WayPoint : MonoBehaviour
{
    //public MonoBehaviour monster;
    private float speed = 10f;
    public Transform[] wayPoint;
    public int wayPointCount = 0;

    private void Start()
    {
        transform.position = wayPoint[wayPointCount].transform.position;
    }

    private void Update()
    {
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
