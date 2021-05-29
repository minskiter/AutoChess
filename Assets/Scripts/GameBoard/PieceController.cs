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
    public enum PieceState
    {
        Idle,
        Move,
        Die,
        Attack
    }

    [Header("Property")]

    public int maxHealth = 5;


    // current health
    private int health;

    // GUI Responsive 
    public int Health
    {
        get
        {
            return health;
        }
    }

    public GameObject healthUI;

    public GameObject healthSystem;

    // if alive?
    public bool Alive => health > 0;

    public int attack = 1;

    public int star = 1;


    // attack interval 1s
    [SerializeField]
    private float attackInterval = 1f;

    // attack distance
    [SerializeField]
    private float attackDistance = 1;
    public float AttackDistance => attackDistance;

    [Header("Move")]
    // move
    [SerializeField]
    private float moveSpeed = 2f;

    // Offset to grid cell
    public Vector3 offset;

    // true if the game object is darggable, otherwise false
    public bool draggable;

    /// <summary>
    /// card place
    /// </summary>
    public bool placeable;

    [Header("Team")]

    public int Team = 0;

    private Vector3 targetPos;

    // move public
    public Vector3 TargetPos => targetPos;

    public PieceState state { private set; get; }

    // the position of origin placement
    private Vector3 originPos;

    public Vector3 OriginPos => originPos;

    public PieceController Target;

    public Vector3 CurrentPosition => transform.position;

    [NonSerialized]
    // public Animator AnimatorController;
    public CharacterAnimator AnimatorController;

    void Awake()
    {
        // AnimatorController = GetComponent<Animator>();
        AnimatorController = GetComponent<CharacterAnimator>();
        SetOriginPosition();
        initHealthUI();
    }

    void initHealthUI()
    {
        if (healthSystem == null)
        {
            healthSystem = GameObject.Find("HealthSystem");
        }
        var prefab = Resources.Load<GameObject>("Prefab/Health/Health");
        healthUI = Instantiate<GameObject>(prefab, healthSystem.transform);
        healthUI.GetComponent<HealthController>().target = this;
    }

    public void SetOriginPosition()
    {
        originPos = Vector3Int.RoundToInt(transform.position - offset) + offset;
    }

    /// <summary>
    /// Clone piece Property
    /// </summary>
    /// <param name="maxHealth">max Health</param>
    /// <param name="attack">attack damage</param>
    /// <param name="attackDistance">attack distance</param>
    /// <param name="attackInterval">attack interval</param>
    /// <param name="star">stars</param>
    /// <param name="moveSpeed">move speed</param>
    public void CloneProperties(int maxHealth, int attack, float attackDistance, float attackInterval, int star, float moveSpeed)
    {
        this.maxHealth = maxHealth;
        this.attack = attack;
        this.attackDistance = attackDistance;
        this.attackInterval = attackInterval;
        this.star = star;
        this.moveSpeed = moveSpeed;
    }

    /// <summary>
    /// death event
    /// </summary>
    void Die()
    {
        var grid = GetComponentInParent<Grid>().GetComponent<GameBoardManager>();
        if (grid != null)
        {
            state = PieceState.Die;
            AnimatorController.ChangeAnimation(PlayerAnimations.Death.ToString());
            grid.map.PutPiece(Vector3Int.RoundToInt(CurrentPosition - offset), null);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// move mouse
    /// </summary>
    void OnMouseDrag()
    {
        if (draggable)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            curPosition.z = 0;
            transform.position = curPosition;
        }
        if (placeable)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            curPosition.z = 3f;
            transform.position = curPosition;
        }
    }

    /// <summary>
    /// mouse up
    /// </summary>
    void OnMouseUp()
    {
        if (draggable)
        {
            var grid = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
            var currentCellPosition = Vector3Int.RoundToInt(CurrentPosition - offset);
            if (grid.map.canPlace(currentCellPosition))
            {
                grid.map.PutPiece(Vector3Int.RoundToInt(originPos - offset), null);
                grid.map.PutPiece(currentCellPosition, this);
                transform.position = currentCellPosition + offset;
                originPos = transform.position;
                targetPos = transform.position;
            }
            else
            {
                transform.position = originPos;
            }
        }
        if (placeable)
        {
            var grid = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
            var currentCellPosition = Vector3Int.RoundToInt(CurrentPosition - offset);
            if (grid.map.canPlace(currentCellPosition, true))
            {
                transform.parent = GameObject.Find("Pieces").transform;
                transform.position = currentCellPosition + offset;
                transform.localScale = new Vector3(.3f, .3f, .3f);
                grid.map.PutPiece(currentCellPosition, this);
                originPos = transform.position;
                targetPos = transform.position;
                placeable = false;
                draggable = true;
                grid.AddPiece(this);
            }
            else
            {
                transform.position = originPos;
            }
        }
    }

    /// <summary>
    /// enable move
    /// </summary>
    void OnEnable()
    {
        health = maxHealth; // Set Default health to Max Health
        // fixed origin position
        transform.position = originPos; // origin cell position and offset
        targetPos = originPos;
        // healthUI.SetActive(true);
    }

    /// <summary>
    /// Calculate the attack
    /// </summary>
    /// <returns></returns>
    public bool CanAttackDamage()
    {
        return state == PieceState.Idle;
    }

    private Action<int> applyDamage = null;

    public void Attack(Action<int> applyDamage)
    {
        if (state == PieceState.Idle)
        {
            state = PieceState.Attack;
            // AnimatorController.SetBool("PieceAttack",true);
            AnimatorController.ChangeAnimation(PlayerAnimations.Attack1.ToString());
            this.applyDamage = applyDamage;
            StartCoroutine("AttackAction");
        }
    }

    IEnumerator AttackAction()
    {
        AnimatorController.ChangeAnimation(PlayerAnimations.Attack2.ToString());
        yield return new WaitForSeconds(attackInterval);
        if (state == PieceState.Attack && this.applyDamage != null)
        {
            state = PieceState.Idle;
            this.applyDamage(attack);
            AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
        }
        this.applyDamage = null;
        yield break;
    }

    public void ChangeTeam(int team)
    {
        Team = team;
        AnimatorController.SetFlipX(team == 1);
    }

    /// <summary>
    /// Apply enemy
    /// </summary>
    /// <param name="damage">health damage</param>
    public void ApplyAttacked(int damage)
    {
        Debug.Log($"{gameObject.name}: {health}");
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
        health = maxHealth; // Set Default health to Max Health
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
        if (state == PieceState.Idle)
        {
            targetPos = target;
            // AnimatorController.SetBool("PieceRun",true);
            AnimatorController.ChangeAnimation(PlayerAnimations.Run.ToString());
            state = PieceState.Move;
            Debug.Log("move", this.gameObject);
            StartCoroutine("MoveStep");
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator MoveStep()
    {
        Debug.Log(state);
        var percent = 0f;
        while (state == PieceState.Move)
        {
            percent += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPos, percent);
            if (percent >= 1f)
            {
                transform.position = targetPos;
                state = PieceState.Idle;
                AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        yield break;
    }

}
