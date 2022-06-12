using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead = false;
    public float MaxHealth;
    public float heal;
    public float healPoint;
    private bool isHurt;
    public float currentHealth;
    private Animator anim;
    public HealthBar healthbar;

    void Start()
    {
        currentHealth = MaxHealth;
        healthbar.SetMaxHealth(MaxHealth);
        anim = GetComponent<Animator>();
    }

   

    public void PLayerTakeDamage(float damage)
    {
        isHurt = true;
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
        if (currentHealth <= 0.0f)
        {
           //anim.SetBool("Hurt", false);
          //anim.SetBool("Die", true);
        }
    }


/*    public void TakeDamageOff()
    {
        anim.SetBool("Hurt", false);
    }*/

    public void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    public void Update()
    {
        if (isHurt)
        {
            //anim.SetBool("Hurt", true);
            isHurt = !isHurt;
        }
    }

    private void FixedUpdate()
    {
        if (currentHealth <= healPoint)
        {
            currentHealth += Time.deltaTime * heal;
            healthbar.SetHealth(currentHealth);
        }
    }
}
