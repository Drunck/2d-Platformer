using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyePatrolScript : MonoBehaviour
{
    public List<Transform> points;
    public float 
        speedCoefficient = 2,
        distanceToStartChase = 5,
        distanceToStopChasing = 3;
    private int 
        nextID = 0,
        idChangeValue = 1;

    public Transform player;
    private bool isMovingBackToPatrolPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Transform targetPoint = points[nextID];

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);
        float distanceToPatrolPoint = Vector2.Distance(transform.position, targetPoint.position);

        if (distanceToPatrolPoint > distanceToStopChasing && isMovingBackToPatrolPoint)
        {
            MoveToNextPoint();
        }
        else if (distanceToPlayer <= distanceToStartChase && !isMovingBackToPatrolPoint)
        {
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);

            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speedCoefficient * Time.deltaTime);
        }
    }

    void MoveToNextPoint()
    {
        Transform targetPoint = points[nextID];

        if (targetPoint.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speedCoefficient * Time.deltaTime);

        if(Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            if(nextID == points.Count - 1)
                idChangeValue = -1;
            if(nextID == 0)
                idChangeValue = 1;

            nextID += idChangeValue;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distanceToStartChase);
    }
}
