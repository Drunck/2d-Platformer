using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class scoreCounter : MonoBehaviour
{
    PlayerHealth playerHealth;
    PlayerCombatController playerDamage;
    public int counter = 0;
    public Text scoreDisplay;
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerDamage = FindObjectOfType<PlayerCombatController>();
    }

    public void Count()
    {
        counter += 1;
        print("score = " + counter);
        scoreDisplay.text = "Score : " + counter.ToString();
    }
    
}
