using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Property")]
    public int maxHealth;

    public Text HealthUI;

    public Text MoneyUI;

    /// <summary>
    /// the health of player;
    /// </summary>
    private int health;

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value > maxHealth)
            {
                health = maxHealth;
            }
            else if (value < 0)
            {
                health = 0;
                Die();
            }
            else
                health = value;
            HealthUI.text = health.ToString();
        }
    }

    /// <summary>
    /// gold count
    /// </summary>
    private int gold;

    public int initGold = 5;

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
            MoneyUI.text = gold.ToString();
        }
    }

    [Header("TeamInfo")]

    public int team = 0;

    public bool Alive => health > 0;

    void Awake()
    {
        Health = maxHealth;
        Gold = initGold;
    }

    /// <summary>
    /// apply the damage to player
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    /// <summary>
    /// Spend MoneyUI
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public bool SpendMoney(int money)
    {
        if (Gold >= money)
        {
            Gold -= money;
            return true;
        }
        return false;
    }

    /// <summary>
    /// lose of game
    /// </summary>
    void Die()
    {
        // handler loss here
    }
}
