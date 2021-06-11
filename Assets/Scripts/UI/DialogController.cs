using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [SerializeField]
    private Text _name;

    public string Name
    {
        get
        {
            return _name.text;
        }
        set
        {
            _name.text = value;
        }
    }

    [SerializeField]
    private Text _health;

    public int Health
    {
        get
        {
            return int.Parse(_health.text);
        }
        set
        {
            _health.text = value.ToString();
        }
    }

    [SerializeField]
    private Text _attack;

    public int Attack
    {
        get
        {
            return int.Parse(_attack.text);
        }
        set
        {
            _attack.text = value.ToString();
        }
    }

    [SerializeField]
    private Text _attackDistance;

    public float AttackDistance
    {
        get
        {
            return float.Parse(_attackDistance.text);
        }
        set
        {
            _attackDistance.text = value.ToString("0.00");
        }
    }

    [SerializeField]
    private Text _moveSpeed;

    public float MoveSpeed
    {
        get
        {
            return float.Parse(_moveSpeed.text);
        }
        set
        {
            _moveSpeed.text = value.ToString("0.00");
        }
    }

    [SerializeField]
    private Text _attackInterval;

    public float AttackInterval
    {
        get
        {
            return float.Parse(_attackInterval.text);
        }
        set
        {
            _attackInterval.text = value.ToString("0.00");
        }
    }

    [SerializeField]
    private Text _cost;

    public int Cost
    {
        get
        {
            return int.Parse(_cost.text);
        }
        set
        {
            _cost.text = value.ToString();
        }
    }

    /// <summary>
    /// 设置属性
    /// </summary>
    /// <param name="name"></param>
    /// <param name="health"></param>
    /// <param name="attack"></param>
    /// <param name="attackDistance"></param>
    /// <param name="moveSpeed"></param>
    /// <param name="attackInterval"></param>
    /// <param name="cost"></param>
    public void Set(string name, int health, int attack, float attackDistance, float moveSpeed, float attackInterval,
        int cost)
    {
        Name = name;
        Health = health;
        Attack = attack;
        AttackDistance = attackDistance;
        MoveSpeed = moveSpeed;
        AttackInterval = attackInterval;
        Cost = cost;
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

}
