using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AIAirEnemy : MonoBehaviour
{
    public float nextWaypointDistance = 3f, speed;

    public Transform Player;

    private Rigidbody2D rb;
    private Path path;
    private Seeker seeker;

    private int currentWaypoint;
    private bool reachedEndOfPath;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        seeker.StartPath(rb.position, Player.position, OnPathComplete);
    }

    void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;

        if (transform.position.x >= Player.position.x)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else if(transform.position.x <= Player.position.x)
            transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void UpdatePath()
    {
        if(seeker.IsDone())
            seeker.StartPath(rb.position, Player.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
