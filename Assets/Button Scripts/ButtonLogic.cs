using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    PlayerHealth PH;
    PlayerCombatController PC;
    public void Start()
    {
        PH = GetComponent<PlayerHealth>();
        PC = GetComponent<PlayerCombatController>();
    }

    public void UpgradeHP()
    {
        PH.MaxHealth += 2;
        PH.currentHealth += 3;
    }

    public void UpgradeDamage()
    {
        
        int damage = Random.Range(1, 3);
        
        PC.attack_1Damage += damage;
    }
}
