using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoardManager : MonoBehaviour
{
    private enum State
    {
        Battle,
        Start,
        End
    };

    [SerializeField]
    private State GameState = State.Start;

    [SerializeField]
    private List<PieceController> _piecesList;

    [NonSerialized]
    public MapEditor map;

    private IEnumerator<PieceController> currentPiece;

    // Start is called before the first frame update
    void Start()
    {
        // SetDefault Current Piece;
        map = GetComponentInChildren<Tilemap>(true).GetComponent<MapEditor>();
        foreach (var piece in _piecesList)
        {
            map.PutPiece(Vector3Int.FloorToInt(piece.CurrentPosition+Vector3.down), piece);
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
        if (GameState == State.Battle)
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
                    if (piece.Target == null || !piece.Target.Alive)
                    {
                        FindEnemy(piece);
                    }

                    if (piece.Target == null)
                    {
                        GameState = State.End;
                    }
                    // If need to  move piece
                    if (piece.Target != null && !piece.Moving)
                    {
                        var dis = Vector3.Distance(piece.Target.TargetPos, piece.CurrentPosition);
                        if (dis > piece.AttackDistance+6e-6f)
                        {
                            // Whether the surrounding movable grid is shorter
                            var targetPath = map.FindPathToTarget(Vector3Int.FloorToInt(piece.CurrentPosition+Vector3.down),
                                Vector3Int.FloorToInt(piece.Target.TargetPos+Vector3.down));
                            if (targetPath != null)
                            {
                                if (targetPath.Count > 0)
                                {
                                    var target = new Vector3(targetPath[0].x+.5f, targetPath[0].y, 0);
                                    if (map.CheckMoveable(Vector3Int.FloorToInt(target)))
                                    {
                                        map.PutPiece(Vector3Int.FloorToInt(piece.CurrentPosition+Vector3.down), null);
                                        map.PutPiece(Vector3Int.FloorToInt(target), piece);
                                        piece.Move(target + Vector3.up);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Attack the piece
                            if (piece.CanAttackDamage())
                            {
                                Debug.Log(piece.gameObject.name + " Attack!");
                                piece.Target.ApplyAttacked(piece.attack);
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
