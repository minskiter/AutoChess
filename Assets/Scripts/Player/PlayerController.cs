using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Property")]
    public int maxHealth;

    /// <summary>
    /// the health of player;
    /// </summary>
    private int health;

    /// <summary>
    /// gold count
    /// </summary>
    public int gold;

    [Header("TeamInfo")]

    public int team = 0;

    public bool Alive => health>0;

    private HealthSystem HealthSystem;

    void Awake() {
        HealthSystem = GameObject.Find("TinyHealthSystem").GetComponent<HealthSystem>();
        HealthSystem.SetMaxHealth(maxHealth);
        HealthSystem.HealDamage(maxHealth);
        health = maxHealth;
    }

    /// <summary>
    /// apply the damage to player
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage){
        health-=damage;
        HealthSystem.TakeDamage(damage);
        if (health<=0){
            Die();
        }
    }

    /// <summary>
    /// lose of game
    /// </summary>
    void Die(){
        // handler loss here
    }
}
