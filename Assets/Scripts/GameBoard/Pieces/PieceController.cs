using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Control single piece logic
/// </summary>
public class PieceController: MonoBehaviour
{
    public enum PieceState
    {
        Idle,
        Move,
        Die,
        Attack
    }

    private GameBoardManager gameBoardInstance;


    public GameBoardManager gameBoardManager
    {
        get
        {
            if (gameBoardInstance == null)
            {
                gameBoardInstance = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
            }
            return gameBoardInstance;
        }
    }

    [Header("Property")]

    public int maxHealth = 5;

    public string pieceName;


    /// <summary>
    /// Spend Cost
    /// </summary>
    public int cost;


    // CurrentMap health
    private int health;

    // GUI Responsive 
    public int Health
    {
        get
        {
            return health;
        }
        private set
        {
            int preValue = health;
            health = value;
            OnHealthUpdate?.Invoke(preValue, value);
            if (health <= 0 && preValue > 0)
            {
                health = 0;
                StartCoroutine(Die());
            }
        }
    }

    public event Action<int, int> OnHealthUpdate;

    public GameObject healthUI;

    public GameObject healthSystem;

    // if alive?
    public bool Alive => Health > 0;

    public int attack = 1;

    public int star = 1;

    public bool Clickable
    {
        get
        {
            return draggable || Placeable;
        }
        set
        {
            if (value == false)
            {
                draggable = false;
                Placeable = false;
            }
        }

    }


    // attack interval 1s
    [SerializeField]
    private float attackInterval = 1f;

    // attack distance
    [SerializeField]
    private float attackDistance = 1;
    public float AttackDistance => attackDistance;

    public bool IsRemoteAttack;

    [Header("Move")]
    // move
    [SerializeField]
    private float moveSpeed = 2f;

    // Offset to grid cell
    public Vector3 offset;

    // true if the game object is darggable, otherwise false
    public bool draggable;

    private bool _placeable;
    /// <summary>
    /// card place
    /// </summary>
    public bool Placeable
    {
        get
        {
            return _placeable;
        }
        set
        {
            _placeable = value;
            if (value)
            {
                healthUI?.SetActive(false);
            }
            else
            {
                healthUI?.SetActive(true);
            }
        }
    }

    [Header("Team")]

    public int Team = 0;

    private Vector3 targetPos;

    // move public
    public Vector3 TargetPos => targetPos;

    public PieceState state { private set; get; }

    // the position of origin placement
    private Vector3 originPos;

    public Vector3 OriginPos
    {
        get
        {
            return originPos;
        }
        set
        {
            originPos = value;
        }
    }

    public PieceController Target;

    public Vector3 CurrentPosition => transform.position;

    [NonSerialized]
    // public Animator AnimatorController;
    public CharacterAnimator AnimatorController;

    private DialogController dialogController;

    public AudioSource audio;

    void Awake()
    {
        AnimatorController = GetComponent<CharacterAnimator>();
        dialogController = FindObjectOfType<DialogController>(true);
        audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        SetOriginPosition();
        initHealthUI();
    }

    /// <summary>
    /// enable move
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(ResetProperty());
    }

    IEnumerator ResetProperty()
    {
        yield return null;
        AnimatorController.SetFlipX(Team == 1);
        Health = maxHealth; // Set Default health to Max Health
        // fixed origin position
        transform.position = originPos; // origin cell position and offset
        targetPos = originPos;
        healthUI.SetActive(true);
        yield break;
    }

    public void initHealthUI()
    {
        if (healthSystem == null)
        {
            healthSystem = GameObject.Find("HealthSystem");
        }

        if (healthUI != null)
        {
            Destroy(healthUI);
        }

        var prefab = Resources.Load<GameObject>(Team == 1 ? "Prefab/Health/RedHealth" : "Prefab/Health/BlueHealth");
        healthUI = Instantiate<GameObject>(prefab, healthSystem.transform);
        healthUI.GetComponent<HealthController>().Init(this);
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
    public void CloneProperties(string name, int maxHealth, int attack, float attackDistance, float attackInterval, int star, float moveSpeed, int cost)
    {
        this.pieceName = name;
        this.maxHealth = maxHealth;
        this.attack = attack;
        this.attackDistance = attackDistance;
        this.attackInterval = attackInterval;
        this.star = star;
        this.moveSpeed = moveSpeed;
        this.cost = cost;
    }

    public event Action OnDie;

    /// <summary>
    /// death event
    /// </summary>
    public IEnumerator Die()
    {
        state = PieceState.Die;
        AnimatorController.ChangeAnimation(PlayerAnimations.Death.ToString());
        yield return new WaitForSeconds(1f);
        gameBoardManager.map.PutPiece(Vector3Int.RoundToInt(CurrentPosition - offset), null);
        gameObject.SetActive(false);
        OnDie?.Invoke();
        yield break;
    }

    private bool _doubleclick = false;

    void OnMouseDown()
    {
        if (!Clickable) return;
        if (!_doubleclick)
        {
            _doubleclick = true;
            Invoke("CancelClick", .3f);
        }
        else
        {
            dialogController.Set($"{pieceName}", maxHealth, attack, AttackDistance, moveSpeed, attackInterval, cost,star);
            _doubleclick = false;
            CancelInvoke("CancelClick");
            if (!dialogController.gameObject.activeSelf)
                dialogController.gameObject.SetActive(true);
        }
    }

    void CancelClick()
    {
        Debug.Log("CancelInvoke");
        _doubleclick = false;
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
        if (Placeable)
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
            var currentCellPosition = Vector3Int.RoundToInt(CurrentPosition - offset);
            if (gameBoardManager.map.canPlace(currentCellPosition))
            {
                gameBoardManager.map.PutPiece(Vector3Int.RoundToInt(originPos - offset), null);
                gameBoardManager.map.PutPiece(currentCellPosition, this);
                transform.position = currentCellPosition + offset;
                originPos = transform.position;
                targetPos = transform.position;
            }
            else
            {
                transform.position = originPos;
            }
        }
        if (Placeable)
        {
            var currentCellPosition = Vector3Int.RoundToInt(CurrentPosition - offset);
            if (gameBoardManager.map.canPlace(currentCellPosition, true))
            {
                var add = gameBoardManager.AddPiece(this);
                if (add != null)
                {
                    var card = transform.parent.gameObject;
                    if (card != null)
                    {
                        var ui = card.GetComponent<CardBaseController>().ui;
                        if (ui != null)
                        {
                            ui.GetComponent<Animator>().SetTrigger("FrontToBack");
                        }
                    }
                    add.transform.parent = GameObject.Find("Pieces").transform;
                    add.transform.position = currentCellPosition + offset;
                    add.transform.position -= new Vector3(0, 0, add.transform.position.z);
                    add.transform.localScale = new Vector3(.3f, .3f, .3f);
                    add.originPos = add.transform.position;
                    add.targetPos = add.transform.position;
                    add.Placeable = false;
                    add.draggable = true;
                    gameBoardManager.map.PutPiece(currentCellPosition, add);
                }
                else
                {
                    transform.position = originPos;
                }

            }
            else
            {
                transform.position = originPos;
            }
        }
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
            this.applyDamage = applyDamage;
            StartCoroutine("AttackAction");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            var controller = collision.gameObject.GetComponent<BallController>();
            if (controller.Team != Team)
            {
                if (controller.Attack.HasValue)
                {
                    audio.clip = DataManager.Instance.AttackSource["Arrow-damage"];
                    audio.Play();
                    ApplyAttacked(controller.Attack.Value);
                }

                controller.MaxAttackEnemy--;
                if (controller.MaxAttackEnemy <= 0)
                {
                    //collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    IEnumerator AttackAction()
    {
        AnimatorController.ChangeAnimation(PlayerAnimations.Attack2.ToString());
        AnimatorController.SetFlipX(Target.transform.position.x < transform.position.x);
        if (AnimatorController.MyJob == Jobs.Warrior)
        {
            audio.clip = DataManager.Instance.AttackSource["warrior"];
            audio.Play();
        }
        else if (AnimatorController.MyJob == Jobs.Duelist)
        {
            audio.clip = DataManager.Instance.AttackSource["duelist"];
            audio.Play();
        }else if (AnimatorController.MyJob == Jobs.Archer)
        {
            audio.clip = DataManager.Instance.AttackSource["Arrow-start"];
            audio.Play();
        }
        yield return new WaitForSeconds(attackInterval);
        if (!IsRemoteAttack)
        {
            if (state == PieceState.Attack && this.applyDamage != null)
            {
                state = PieceState.Idle;
                this.applyDamage(attack);
                AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
            }

        }
        else
        {
            if (state == PieceState.Attack)
            {
                state = PieceState.Idle;
                AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
            }
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
        Health -= damage;
    }

    /// <summary>
    /// reset piece state
    /// </summary>
    public void Reset()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        state = PieceState.Idle;
        Target = null;
        Health = maxHealth; // Set Default health to Max Health
        // fixed origin position
        transform.position = originPos; // origin cell position and offset
        targetPos = originPos;
        lastMove = originPos;
        // reset animation
        // AnimatorController.Rebind();
        // AnimatorController.Update(0f);
        AnimatorController.ChangeAnimation(PlayerAnimations.Idle.ToString());
        AnimatorController.SetFlipX(Team == 1);
    }

    private void OnDestroy()
    {
        Destroy(healthUI);
    }

    public Vector3 lastMove;

    public bool Move(Vector3 target)
    {
        if (state == PieceState.Idle)
        {
            targetPos = target;
            lastMove = TargetPos;
            // AnimatorController.SetBool("PieceRun",true);
            AnimatorController.ChangeAnimation(PlayerAnimations.Run.ToString());
            state = PieceState.Move;
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
        var percent = 0f;
        AnimatorController.SetFlipX(Target.transform.position.x < transform.position.x);
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
