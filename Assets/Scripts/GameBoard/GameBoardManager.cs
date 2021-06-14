using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoardManager : MonoBehaviour
{
    public enum GameState
    {
        Battle,
        Start,
        End,
        Pause
    };

    [SerializeField]
    private GameState state = GameState.Start;

    public GameState BoardState => state;

    public List<PieceController> _piecesList;

    public MapEditor map;

    private IEnumerator<PieceController> currentPiece;

    public Action WinHandler { get; set; }

    public Action<int> LossHandler { get; set; }

    public PlayerController playerController;

    public DataManager dataManager = DataManager.Instance;

    public GameMap currentMap;

    public void GameOver(int winner)
    {
        if (state == GameState.End)
        {
            state = GameState.Start;
            SetDraggable(true, 0);
            if (winner == 0)
            {
                WinHandler?.Invoke();
            }
            else
            {
                if (LossHandler != null)
                {
                    var healthDamage = GetAllTeamHealth(1);
                    LossHandler(healthDamage);
                }
            }
            currentPiece.Reset();
            // clear old piece
            map.ResetPieces();
            // reset new piece
            foreach (var piece in _piecesList)
            {
                piece.Reset();
                map.PutPiece(Vector3Int.RoundToInt(piece.CurrentPosition - piece.offset), piece);
            }
        }
    }


    private bool _startBattleLock = false;

    private void UnlockBattle()
    {
        _startBattleLock = false;
    }
    /// <summary>
    /// start battle
    /// </summary>
    public void StartBattle()
    {
        if (_startBattleLock) return;
        _startBattleLock = true;
        switch (state)
        {
            case GameState.Battle:
                {
                    state = GameState.Pause;
                    break;
                }
            case GameState.Pause:
                {
                    state = GameState.Battle;
                    break;
                }
            case GameState.Start:
                {
                    state = GameState.Battle;
                    SetDraggable(false);
                    currentPiece = _piecesList.GetEnumerator();
                    break;
                }
        }

        Invoke("UnlockBattle", 1f); // TODO: 暂时如此，虽然很粗糙
    }

    /// <summary>
    /// Reset game board
    /// </summary>
    void Reset()
    {
        map.ResetPieces();
        _piecesList.Clear();
    }

    /// <summary>
    /// set the piece draggable
    /// </summary>
    /// <param name="draggable"></param>
    /// <param name="team"></param>
    public void SetDraggable(bool draggable, int team = -1)
    {
        foreach (var piece in _piecesList)
        {
            if (team == -1)
                piece.draggable = draggable;
            else
            {
                if (piece.Team == team)
                {
                    piece.draggable = draggable;
                }
            }
        }
    }

    public void LoadMap(GameMap map)
    {
        currentMap = map;
        this.map.InitTileMap(true);
        this.map.LoadMap(map);
        _piecesList = this.map.Pieces;
        SetDraggable(false, 1);
    }

    public void NextMap(GameMap map)
    {
        if (map != null)
        {
            currentMap = map;
            this.map.LoadMap(map);
            SetDraggable(false, 1);
        }
    }

    void Start()
    {
        _piecesList ??= new List<PieceController>();
        if (DataManager.Instance.CurrentMap != null)
        {
            LoadMap(DataManager.Instance.CurrentMap);
        }
        StartCoroutine(Battle());
    }


    public PieceController AddPiece(PieceController piece)
    {
        if (piece.cost <= playerController.Gold && state == GameState.Start)
        {
            if (playerController.SpendMoney(piece.cost))
            {
                PieceController _old;
                do
                {
                    _old = piece;
                    piece = UpgradePiece(piece);
                } while (_old != piece);

                _piecesList.Add(piece);
                return piece;
            }
        }
        return null;
    }


    public PieceController UpgradePiece(PieceController piece)
    {
        List<PieceController> pieces = new List<PieceController>
        {
        };
        foreach (var p in _piecesList)
        {
            if (p.pieceName == piece.pieceName && piece.star == p.star && piece.Team == p.Team)
            {
                pieces.Add(p);
            }
        }
        if (pieces.Count >= 2)
        {
            if (dataManager.PiecePrefabs.ContainsKey(piece.star + 1))
            {
                var list = dataManager.PiecePrefabs[piece.star + 1];
                var upgradePiece = list.Find(e => e.GetComponent<PieceController>().pieceName == piece.pieceName);
                if (upgradePiece != null)
                {
                    var pieceInstance = Instantiate(upgradePiece);
                    pieceInstance.transform.position = piece.transform.position;
                    var controller = pieceInstance.GetComponent<PieceController>();
                    LayerTool.ChangeLayer(pieceInstance.transform, 3);
                    foreach (var p in pieces)
                    {
                        map.PutPiece(Vector3Int.RoundToInt(p.CurrentPosition - p.offset), null);
                        _piecesList.Remove(p);
                        Destroy(p.gameObject);
                    }
                    Destroy(piece.gameObject);
                    return controller;
                }
            }
        }
        return piece;
    }

    public int GetAllTeamHealth(int team)
    {
        return _piecesList.Where(piece => piece.Team == team).Sum(piece => piece.Health);
    }


    private void FindEnemy(PieceController origin)
    {
        float minDistance = float.MaxValue;
        PieceController target = null;
        foreach (var piece in _piecesList)
        {
            // If target piece alive
            if (origin != piece && piece.Alive && piece.Team != origin.Team)
            {
                // Compare the distance of enemy
                var dis = Vector3.Distance(origin.CurrentPosition, piece.TargetPos);
                if (dis < minDistance)
                {
                    minDistance = dis;
                    target = piece;
                }
            }
        }
        origin.Target = target;
    }

    public IEnumerator Battle()
    {
        currentPiece = _piecesList.GetEnumerator();
        while (true)
        {
            if (state == GameState.Battle)
            {
                // If end of piece then  reset piece to start
                if (!currentPiece.MoveNext())
                {
                    currentPiece.Reset();
                }
                else
                {
                    var piece = currentPiece.Current;
                    // If piece alive then judge if it doesn't have the target enemy
                    if (piece.Alive)
                    {
                        if (piece.Target != null && !piece.Target.Alive)
                        {
                            piece.Target = null;
                        }
                        if (piece.Target == null)
                        {
                            FindEnemy(piece);
                        }
                        if (piece.Target == null)
                        {
                            yield return new WaitForSeconds(5f);
                            // update winner
                            state = GameState.End;
                            GameOver(piece.Team);
                        }
                        else
                        {
                            // If need to  move piece
                            if (piece.state != PieceController.PieceState.Move)
                            {

                                var dis = piece.IsRemoteAttack
                                    ? Vector3.Distance(piece.Target.TargetPos, piece.CurrentPosition)
                                    : Mathf.Abs(piece.Target.TargetPos.x - piece.CurrentPosition.x);
                                if ((dis > piece.AttackDistance + 6e-6f && piece.IsRemoteAttack) || (!piece.IsRemoteAttack && (piece.Target.TargetPos.y != piece.CurrentPosition.y || dis > piece.AttackDistance + 6e-6f))) // float number equal 
                                {
                                    // Whether the surrounding movable grid is shorter
                                    var targetPath = map.FindPathToTarget(Vector3Int.RoundToInt(piece.CurrentPosition - piece.offset),
                                        Vector3Int.RoundToInt(piece.Target.TargetPos - piece.offset), piece);
                                    if (targetPath != null)
                                    {
                                        if (targetPath.Count > 0)
                                        {
                                            var target = new Vector3(targetPath[0].x, targetPath[0].y, 0);
                                            if (map.CheckMoveable(Vector3Int.RoundToInt(target)))
                                            {
                                                map.PutPiece(Vector3Int.RoundToInt(target), piece); // place the piece to target first 
                                                if (piece.Move(target + piece.offset))
                                                {
                                                    map.PutPiece(Vector3Int.RoundToInt(piece.CurrentPosition - piece.offset), null);
                                                }
                                                else
                                                {
                                                    map.PutPiece(Vector3Int.RoundToInt(target), null);
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        piece.Target = null;
                                    }
                                }
                                else
                                {
                                    // Attack the piece
                                    if (piece.CanAttackDamage())
                                    {
                                        piece.Attack(piece.Target.ApplyAttacked);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            yield return null;
        }
    }

}
