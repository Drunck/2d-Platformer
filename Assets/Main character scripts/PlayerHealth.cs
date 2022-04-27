using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead = false;
    public float MaxHealth;
    private float currentHealth;
    
    void Start()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        //Animation

        if (currentHealth <= 0.0f)
        {
            Die(); 
        }
    }

    private void Die()
    {
        isDead = true;
        //Animation
    }
    void Update()
    {
        
    }
}
