using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemiesScriptableObjects : ScriptableObject
{
    public string enemyName;
    public int minDamage, maxDamage;
    public float health;
}
