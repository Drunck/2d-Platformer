using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeAttackScript : MonoBehaviour
{
    public float 
        speedCoefficient = 2,
        lineOfSite = 5;
    public Transform player;

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);
        if(distanceToPlayer < lineOfSite)
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speedCoefficient * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
    }
}
