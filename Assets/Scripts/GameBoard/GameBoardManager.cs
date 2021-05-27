using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoardManager : MonoBehaviour
{
    public enum GameState
    {
        Battle,
        Start,
        End
    };

    [SerializeField]
    private GameState state = GameState.Start;

    public GameState BoardState => state;

    [SerializeField]
    private List<PieceController> _piecesList;

    [NonSerialized]
    public MapEditor map;

    private IEnumerator<PieceController> currentPiece;

    public Action WinHandler { get; set; }

    public Action LossHandler { get; set; }

    void Awake()
    {

    }

    public void GameOver(int winner)
    {
        if (state == GameState.End)
        {
            state = GameState.Start;
            SetDraggable(true,0);
            if (winner == 0)
            {
                if (WinHandler != null)
                    WinHandler();
            }
            else
            {
                if (LossHandler != null)
                    LossHandler();
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

    /// <summary>
    /// start battle
    /// </summary>
    void StartBattle(){
        state = GameState.Battle;
        SetDraggable(false);
    }

    /// <summary>
    /// Reset game board
    /// </summary>
    void Reset(){
        if (map!=null){
            map.ResetPieces();
            _piecesList.Clear();
        }
    }

    /// <summary>
    /// set the piece draggable
    /// </summary>
    /// <param name="draggable"></param>
    /// <param name="team"></param>
    public void SetDraggable(bool draggable,int team=-1){
        foreach (var piece in _piecesList){
            if (team==-1)
                piece.draggable = draggable;
            else{
                if (piece.Team == team){
                    piece.draggable = draggable;
                }
            }
        }
    }

    void OnEnable()
    {
        map = GetComponentInChildren<Tilemap>(true).GetComponent<MapEditor>();
        map.InitTileMap();
        // SetDefault Current Piece;   
        if (_piecesList == null || _piecesList.Count == 0)
        {
            var pieces = GameObject.Find("Pieces");
            foreach (var piece in pieces.GetComponentsInChildren<PieceController>())
            {
                _piecesList.Add(piece);
            }
        }
        foreach (var piece in _piecesList)
        {
            map.PutPiece(Vector3Int.RoundToInt(piece.CurrentPosition - piece.offset), piece);
        }
        currentPiece = _piecesList.GetEnumerator();
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

    void Battle()
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
                        // update winner
                        state = GameState.End;
                        GameOver(piece.Team);
                    }
                    // If need to  move piece
                    if (piece.Target != null && piece.state != PieceController.PieceState.Move)
                    {
                        var dis = Vector3.Distance(piece.Target.TargetPos, piece.CurrentPosition);
                        Debug.Log($"{piece.gameObject.name} {dis}");
                        if (dis > piece.AttackDistance + 6e-6f) // float number equal 
                        {
                            // Whether the surrounding movable grid is shorter
                            var targetPath = map.FindPathToTarget(Vector3Int.RoundToInt(piece.CurrentPosition - piece.offset),
                                Vector3Int.RoundToInt(piece.Target.TargetPos - piece.offset));
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

    // Update is called once per frame
    void Update()
    {
        Battle();
    }
}
