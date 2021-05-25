using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Control single piece logic
/// </summary>
public class PieceController : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 5;

    // current health
    [SerializeField]
    private int health = 5;

    // if alive?
    public bool Alive => health >= 0;
    
    public int attack = 1;

    // attack interval 1s
    [SerializeField]
    private int attackInterval = 1;
    private float attackClock = 1;

    // attack distance
    [SerializeField]
    private int attackDistance = 1;
    public int AttackDistance => attackDistance;

    

    // move
    [SerializeField]
    private float moveSpeed = .1f;
    private Vector3 targetPos;

    private float moveStart = 0;

    // move public
    public Vector3 TargetPos => targetPos;

    public bool Moving { private set; get; }

    // the position of origin placement
    private Vector3 originPos;

    public PieceController Target;

    public Vector3 CurrentPosition => transform.position;

    public int Team = 0;

    /// <summary>
    /// death event
    /// </summary>
    void Death()
    {
        gameObject.SetActive(false);
        var grid = GetComponentInParent<Grid>().GetComponent<GameBoardManager>();
        if (grid != null)
        {
            Debug.Log($"{gameObject.name} is deactivate");
            grid.map.PutPiece(Vector3Int.FloorToInt(CurrentPosition + Vector3.down), null);
        }
    }

    /// <summary>
    /// Calculate the attack
    /// </summary>
    /// <returns></returns>
    public bool CanAttackDamage()
    {
        if (attackClock<=0)
        {
            attackClock = attackInterval;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Apply enemy
    /// </summary>
    /// <param name="damage">health damage</param>
    public void ApplyAttacked(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} health:{health}");
        if (health <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// reset piece state
    /// </summary>
    public void Reset()
    {
        health = maxHealth;
        attackClock = attackInterval;
        transform.position = originPos;
    }

    public void Move(Vector3 target)
    {
        if (Vector3.Distance(CurrentPosition, target) <= 1.9f)
        {
            targetPos = Vector3Int.FloorToInt(target) + new Vector3(.5f, 0);
            Moving = true;
            moveStart = 0;
        }
        else
        {
            targetPos = Vector3Int.FloorToInt(transform.position)+new Vector3(.5f,0,0);
        }
    }

    private void MoveStep()
    {
        if (Moving)
        {
            moveStart += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPos, moveStart);
            if (moveStart > 1 * moveSpeed)
            {
                Moving = false;
                moveStart = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        originPos = Vector3Int.FloorToInt(transform.position) + new Vector3(.5f, 0, 0);
        targetPos = originPos;
        transform.position = originPos;
    }

    // Update is called once per frame
    void Update()
    {
        attackClock -= Time.deltaTime;
        MoveStep();
    }
}
