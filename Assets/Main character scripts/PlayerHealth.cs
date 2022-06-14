using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead;
    public float MaxHealth;
    public float heal;
    public float healPoint;
    public float currentHealth;
    private Animator anim;
    public HealthBar healthbar;
    public GameObject menu;
    void Start()
    {
        currentHealth = MaxHealth;
        healthbar.SetMaxHealth(MaxHealth);
        anim = GetComponent<Animator>();
        isDead = false;
    }




    public void TakeDamage(float damage)
    {
        
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
        if (currentHealth <= 0.0f)
        {
            anim.SetBool("isDead", true);
            isDead = true;
            menu.SetActive(true);
            Time.timeScale = 0;
        }
    }




    public void Die()
    {

        isDead = true;
        //Destroy(gameObject);
        menu.SetActive(true);
        Time.timeScale = 0;
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
