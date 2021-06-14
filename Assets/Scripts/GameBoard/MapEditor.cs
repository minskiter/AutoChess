using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Tracing;
using System.Linq;
using Assets.Scripts.Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditor : MonoBehaviour
{
    public Tilemap Map; // The GameMap

    public Tile cellTile; // the style of tile

    public GameObject PieceArea;

    // judge if this init
    private bool init = false;

    private int height = 5;
    private int width = 11;

    private RectInt mapRect => new RectInt(-width / 2, -height / 2, width, height);

    private bool[,] _map; // true if the cell exists, otherwise false

    private PieceController[,] _pieceLocates; // piece locate 

    public List<PieceController> Pieces
    {
        get
        {
            var list = new List<PieceController>();
            foreach (var pieceController in _pieceLocates)
            {
                if (pieceController != null)
                    list.Add(pieceController);
            }

            return list;
        }
    }

    private bool[,] _canPlacePiece; // the cell which player can place

    private bool editable = true; // editable

    private bool update = false; // is update

    private float curTimestamp = 0f;

    private int leftTileCount = 0;

    public int maxPieces = 5;

    private readonly List<Vector3Int> _forwards3 = new List<Vector3Int>
    {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    private readonly List<Vector2Int> _forwards2 = new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void OnEnable()
    {
        InitTileMap();
    }

    public void LoadMap(GameMap map)
    {
        if (height == map.Height && width == map.Width)
        {
            InitTileMap();
            // 清除所有队伍为1的棋子
            ClearPiece(1);
            for (var row = 0; row < map.Height; ++row)
            {
                for (var col = 0; col < map.Width; ++col)
                {
                    var cell = map.Map[row, col];
                    if (cell != null && !string.IsNullOrWhiteSpace(cell))
                    {
                        var piece = cell.Split('|');
                        var piecePrefab = DataManager.Instance.PiecePrefabs[int.Parse(piece[1])]
                            .FirstOrDefault(e =>
                                (e != null ? e.GetComponent<PieceController>().pieceName : null) == piece[0]);
                        if (piecePrefab != null)
                        {
                            var pieceInstance = Instantiate(piecePrefab, PieceArea.transform);
                            var pieceController = pieceInstance.GetComponent<PieceController>();
                            pieceController.OriginPos = new Vector3(col + mapRect.xMin, row + mapRect.yMin, 0) +
                                                        pieceController.offset;
                            pieceController.ChangeTeam(1);
                            pieceController.initHealthUI();
                            pieceController.Reset();
                            LayerTool.ChangeLayer(pieceController.transform, 3);
                            PutPiece(Vector3Int.RoundToInt(new Vector3(col + mapRect.xMin, row + mapRect.yMin, 0)),
                                pieceController);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("地图长宽错误");
        }
    }

    public int GetPieceCount(int Team = 0)
    {
        int cnt = 0;
        if (_pieceLocates != null)
            foreach (var piece in _pieceLocates)
            {
                if (piece != null)
                {
                    cnt += piece.Team == Team ? 1 : 0;
                }
            }
        return cnt;
    }

    public bool canPlace(Vector3Int position, bool add = false)
    {
        if (add)
        {
            int cnt = GetPieceCount(0);
            if (cnt + 1 > maxPieces) return false;
        }

        Vector2Int inner = new Vector2Int(position.x, position.y) - mapRect.min;
        return mapRect.Contains(new Vector2Int(position.x, position.y)) && _canPlacePiece[inner.x, inner.y] && _map[inner.x, inner.y] && _pieceLocates[inner.x, inner.y] == null;
    }

    /// <summary>
    /// Initialize Map
    /// </summary>
    public void InitTileMap(bool force = false)
    {
        if (!init || force)
        {
            _map = new bool[mapRect.width, mapRect.height];
            _pieceLocates = new PieceController[mapRect.width, mapRect.height];
            _canPlacePiece = new bool[mapRect.width, mapRect.height];
            foreach (var pos in mapRect.allPositionsWithin)
            {
                _map[pos.x - mapRect.xMin, pos.y - mapRect.yMin] = true;
                if (pos.x - mapRect.xMin <= mapRect.width / 2)
                {
                    _canPlacePiece[pos.x - mapRect.xMin, pos.y - mapRect.yMin] = true;
                }
            }
            ReloadMap();
            init = true;
        }
    }

    public void ClearPiece(int team = -1)
    {
        for (var row = 0; row < height; ++row)
        {
            for (var col = 0; col < width; ++col)
            {
                if (team == -1) _pieceLocates[col, row] = null;
                else if (_pieceLocates[col, row] != null && _pieceLocates[col, row].Team == team)
                {
                    _pieceLocates[col, row] = null;
                }
            }
        }
    }

    void ReloadMap()
    {
        if (_map != null)
        {
            foreach (var pos in mapRect.allPositionsWithin)
                if (_map[pos.x - mapRect.xMin, pos.y - mapRect.yMin])
                {
                    Map.SetTile(new Vector3Int(pos.x, pos.y, 0), cellTile);
                }
                else
                {
                    Map.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
                }
        }
    }

    public bool CheckMoveable(Vector3Int pos)
    {
        var cellPosition = new Vector2Int(pos.x, pos.y);
        return mapRect.Contains(cellPosition) && _map[cellPosition.x - mapRect.xMin, cellPosition.y - mapRect.yMin] &&
                _pieceLocates[cellPosition.x - mapRect.xMin, cellPosition.y - mapRect.yMin] == null;
    }

    public void ResetPieces()
    {
        for (var row = 0; row < _pieceLocates.GetLength(0); ++row)
        {
            for (var col = 0; col < _pieceLocates.GetLength(1); ++col)
            {
                _pieceLocates[row, col] = null;
            }
        }
    }

    /// <summary>
    /// Put piece in the cell
    /// </summary>
    /// <param name="pos">the position of cell</param>
    /// <param name="piece">the piece</param>
    /// <returns></returns>
    public bool PutPiece(Vector3Int pos, PieceController piece = null)
    {
        var cellPosition = new Vector2Int(pos.x, pos.y);
        if (mapRect.Contains(cellPosition) && _map[cellPosition.x - mapRect.xMin, cellPosition.y - mapRect.yMin])
        {
            _pieceLocates[cellPosition.x - mapRect.xMin, cellPosition.y - mapRect.yMin] = piece;
            return true;
        }
        return false;
    }

    /// <summary>
    ///  Find a path to target cell
    /// </summary>
    /// <param name="source">source cell</param>
    /// <param name="target">target cell</param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector3Int source, Vector3Int target, PieceController piece)
    {
        var path = new List<Vector2Int>();
        var target2 = new Vector2Int(target.x, target.y);
        var source2 = new Vector2Int(source.x, source.y);
        var parent = new Vector2Int?[mapRect.width, mapRect.height];
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(source2);
        while (queue.Count > 0)
        {
            var front = queue.Dequeue();
            foreach (var direction in _forwards2)
            {
                var next = front + direction;
                var nextInRect = next - mapRect.min;
                if (mapRect.Contains(next) && _map[nextInRect.x, nextInRect.y] && (_pieceLocates[nextInRect.x, nextInRect.y] == null) && parent[nextInRect.x, nextInRect.y] == null)
                {
                    parent[nextInRect.x, nextInRect.y] = front;
                    if ((piece.IsRemoteAttack && Vector2.Distance(target2, next) < piece.AttackDistance + 6e-6) || (!piece.IsRemoteAttack && Mathf.Abs(target2.x - next.x) < piece.AttackDistance + 6e-6 && target2.y == next.y))
                    {
                        while (parent[nextInRect.x, nextInRect.y].HasValue)
                        {
                            path.Add(nextInRect + mapRect.min);
                            if ((nextInRect + mapRect.min).Equals(source2)) break;
                            nextInRect = parent[nextInRect.x, nextInRect.y].Value - mapRect.min;
                        }
                        path.Reverse();
                        return path;
                    }
                    queue.Enqueue(next);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Remove Tile
    /// </summary>
    /// <param name="pos"></param>
    private void RemoveTile(Vector3Int pos)
    {
        if (!update)
        {
            update = true;
            int x = pos.x + width / 2;
            int y = pos.y + height / 2;
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                _map[y, x] = false;
                Map.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
            }

            update = false;
        }
    }

    private void PutTile(Vector3Int pos)
    {
        if (leftTileCount >= 0 && leftTileCount < 15)
        {
            int x = pos.x + width / 2;
            int y = pos.y + height / 2;
            if (x >= 0 && y >= 0 && x < width && y < height && !_map[y, x])
            {
                if (leftTileCount != 0)
                {
                    foreach (var forward in _forwards3)
                    {
                        var nxt = new Vector3Int(x, y, 0) + forward;
                        if (nxt.x >= 0 && nxt.y >= 0 && nxt.x < width && nxt.y < height && _map[nxt.y, nxt.x])
                        {
                            _map[y, x] = true;
                            Map.SetTile(new Vector3Int(pos.x, pos.y, 0), cellTile);
                            ++leftTileCount;
                            break;
                        }
                    }
                }
                else
                {
                    _map[y, x] = true;
                    Map.SetTile(new Vector3Int(pos.x, pos.y, 0), cellTile);
                    ++leftTileCount;
                }
            }
        }
    }

    /// <summary>
    /// Rotate 90 degree
    /// </summary>
    private void RotateRight90Degree()
    {
        int w = width / 2;
        int h = height;
        if (w == h)
        {
            // reverse x/y and reverse y equal rotate 90 degree
            for (int i = 0; i < h; ++i)
            {
                for (int j = i + 1; j < w; ++j)
                {
                    var tmp = _map[i, j];
                    _map[i, j] = _map[j, i];
                    _map[j, i] = tmp;
                }
            }

            for (int i = 0; i < h / 2; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    var tmp = _map[i, j];
                    _map[i, j] = _map[h - i - 1, j];
                    _map[h - i - 1, j] = tmp;
                }
            }
            ReloadMap();
        }
    }

    private void ClearLeftMap()
    {
        int w = width / 2;
        int h = height;
        if (w == h)
        {
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    _map[i, j] = false;
                }
            }
            ReloadMap();
            leftTileCount = 0;
        }
    }

    /// <summary>
    /// move left to right
    /// </summary>
    private void LeftMergeRight()
    {
        int emptyColumns = 0;
        for (int col = width / 2 - 1; col >= 0; --col)
        {
            bool empty = true;
            for (int row = 0; row < height; ++row)
            {
                if (_map[row, col])
                {
                    empty = false;
                    break;
                }
            }
            if (empty) emptyColumns++;
            else break;
        }

        if (emptyColumns < width / 2 && emptyColumns > 0)
        {
            int moveIndex = width / 2 - 1 - emptyColumns;
            for (int col = width / 2 - 1; col >= 0 && moveIndex >= 0; --col)
            {
                for (int row = 0; row < height; ++row)
                {
                    _map[row, col] = _map[row, moveIndex];
                    _map[row, moveIndex] = false;
                }

                --moveIndex;
            }
            ReloadMap();
        }
    }

}
