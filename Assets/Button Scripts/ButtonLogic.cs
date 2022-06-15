using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerCombatController playerDamage;
    private scoreCounter score;
    public void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerDamage = FindObjectOfType<PlayerCombatController>();
        score = FindObjectOfType<scoreCounter>();
    }

    public void UpgradeHP()
    {
        if (score.counter > 0)
        {
            playerHealth.MaxHealth += 2;
            playerHealth.currentHealth += 3;
            score.counter--;
        }
    }

    public void UpgradeDamage()
    {
        int damage = Random.Range(1, 3);
        if (score.counter > 0)
        {
            playerDamage.attack_1Damage += damage;
            score.counter--;
        }
        
        //PC.attack_1Damage += damage;
    }
}
