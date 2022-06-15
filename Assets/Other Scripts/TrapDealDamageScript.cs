using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDealDamageScript : MonoBehaviour
{
    public float damage = 2;
    private float[] attackDetails = new float[2];
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            attackDetails[0] = damage;
            attackDetails[1] = transform.position.x;
            other.SendMessage("PlayerTakeDamage", attackDetails);
        }
    }
}
