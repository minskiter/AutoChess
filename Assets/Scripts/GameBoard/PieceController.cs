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
    public enum PieceState{
        Idle,
        Move,
        Die,
        Attack
    }

    [Header("Property")]

    [SerializeField]
    private int maxHealth = 5;


    // current health
    private int health;

    // GUI Responsive 
    private int Health{
        get{
            return health;
        }
        set{

        }
    }

    // if alive?
    public bool Alive => health > 0;

    public int attack = 1;

    // attack interval 1s
    [SerializeField]
    private int attackInterval = 1;
    private float attackClock = 1;

    // attack distance
    [SerializeField]
    private int attackDistance = 1;
    public int AttackDistance => attackDistance;

    [Header("Move")]
    // move
    [SerializeField]
    private float moveSpeed = .1f;

    // Offset to grid cell
    public Vector3 offset;

    // true if the game object is darggable, otherwise false
    public bool draggable;

    [Header("Team")]

    public int Team = 0;

    private Vector3 targetPos;

    private float moveStart = 0;

    // move public
    public Vector3 TargetPos => targetPos;

    public PieceState state {private set;get;}

    // the position of origin placement
    private Vector3 originPos;

    public PieceController Target;

    public Vector3 CurrentPosition => transform.position;

    [NonSerialized]
    // public Animator AnimatorController;
    public CharacterAnimator AnimatorController;

    void Awake() {
        // AnimatorController = GetComponent<Animator>();
        AnimatorController = GetComponent<CharacterAnimator>();
        originPos = Vector3Int.RoundToInt(transform.position-offset)+offset;
    }

    /// <summary>
    /// death event
    /// </summary>
    void Die()
    {
        gameObject.SetActive(false);
        var grid = GetComponentInParent<Grid>().GetComponent<GameBoardManager>();
        if (grid != null)
        {
            state = PieceState.Die;
            AnimatorController.ChangeAnimation(PlayerAnimations.Death.ToString());
            Debug.Log($"{gameObject.name} died");
            grid.map.PutPiece(Vector3Int.RoundToInt(CurrentPosition - offset), null);
        }
    }

    void OnMouseDrag() {
        if (draggable){
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            curPosition.z = 0;
            transform.position = curPosition;    
        }
    }

    void OnMouseUp() {
        if (draggable){
            var grid = GetComponentInParent<Grid>().GetComponent<GameBoardManager>();
            var currentCellPosition = Vector3Int.RoundToInt(CurrentPosition-offset);
            if (grid.map.canPlace(currentCellPosition)){  
                grid.map.PutPiece(Vector3Int.RoundToInt(originPos-offset),null);
                grid.map.PutPiece(currentCellPosition,this);
                transform.position = currentCellPosition+offset;
                originPos = transform.position;
                targetPos = transform.position;
            }else{
                transform.position = originPos;
            }
        }
    }

    void OnEnable()
    {
        health = maxHealth; // Set Default health to Max Health
        attackClock = 0; // Can Attack
        // fixed origin position
        transform.position = originPos; // origin cell position and offset
        targetPos = originPos;
    }

    /// <summary>
    /// Calculate the attack
    /// </summary>
    /// <returns></returns>
    public bool CanAttackDamage()
    {
        if (attackClock <= 0)
        {
            attackClock = attackInterval;
            return true;
        }

        return false;
    }

    private Action<int> applyDamage=null;

    public void Attack(Action<int> applyDamage){
        if (state==PieceState.Idle){
            state = PieceState.Attack;
            // AnimatorController.SetBool("PieceAttack",true);
            AnimatorController.ChangeAnimation(PlayerAnimations.Attack1.ToString());
            this.applyDamage = applyDamage;
        }
    }

    public void ChangeTeam(int team){
        Team  = team;
        AnimatorController.SetFlipX(team==1);
    }

    /// <summary>
    /// Apply enemy
    /// </summary>
    /// <param name="damage">health damage</param>
    public void ApplyAttacked(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// reset piece state
    /// </summary>
    public void Reset()
    {
        state = PieceState.Idle;
        Target = null;
        gameObject.SetActive(true);
        health = maxHealth; // Set Default health to Max Health
        attackClock = 0; // Can Attack
        // fixed origin position
        transform.position = originPos; // origin cell position and offset
        targetPos = originPos;
        // reset animation
        // AnimatorController.Rebind();
        // AnimatorController.Update(0f);
        AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
    }


    public bool Move(Vector3 target)
    {
        if (state==PieceState.Idle){
            if (Vector3.Distance(CurrentPosition, target) <= 1.9f)
            {
                targetPos = target;
                // AnimatorController.SetBool("PieceRun",true);
                AnimatorController.ChangeAnimation(PlayerAnimations.Run.ToString());
                state = PieceState.Move;
                moveStart = 0;
            }
            else
            {
                targetPos = target;
            }
            return true;
        }else{
            return false;
        }
    }

    private void MoveStep()
    {
        if (state == PieceState.Move)
        {         
            moveStart += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPos, moveStart);
            if (moveStart >= 1f)
            {
                transform.position = targetPos;
                state = PieceState.Idle;
                // AnimatorController.SetBool("PieceRun",false);
                AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
                moveStart = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (attackClock>0)
            attackClock -= Time.deltaTime;
        else{
            if (applyDamage!=null){
                if (state==PieceState.Attack){
                    applyDamage(attack);
                    applyDamage=null;
                    state = PieceState.Idle;
                    AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
                }else if (state==PieceState.Die){
                    applyDamage = null;
                }
            }
            // AnimatorController.SetBool("PieceAttack",false);
            // AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
        }
        MoveStep();
    }
}
