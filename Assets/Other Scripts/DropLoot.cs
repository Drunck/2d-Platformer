using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
    public GameObject item;
    [Range(0.0f, 100.0f)]
    public float minDropChance;
    [Range(0.0f, 100.0f)]
    public float maxDropChance;
}

public class DropLoot : MonoBehaviour
{
    public Loot[] loots;
    public Transform enemyPos;

    public void DropItem()
    {
        float dropChance = Random.Range(0.0f, 101.0f);
        foreach (Loot loot in loots)
        {
            if (dropChance > loot.minDropChance && dropChance < loot.maxDropChance)
            {
                GameObject drop = Instantiate(loot.item, enemyPos.position, Quaternion.identity);
                drop.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.Range(-2.0f, 3.0f), Random.Range(1f, 5f)), ForceMode2D.Impulse);
            }
        }
    }
}
