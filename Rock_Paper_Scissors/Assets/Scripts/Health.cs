using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    private int health;

    private void Awake() 
    {
        health = maxHealth;
    }

    public void Damage(int damageAmount)
    {
        Debug.Log("Damage!");
        health -= damageAmount;
        CheckForDeath();
    }

    public void CheckForDeath()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
