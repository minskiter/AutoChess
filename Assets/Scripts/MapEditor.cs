using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditor : MonoBehaviour
{
    public Tilemap Map; // The Map

    public Tile cellTile; // the style of tile

    private int height = 5;
    private int width = 11;

    private RectInt mapRect => new RectInt(-width / 2, -height/2, width, height);

    private bool[,] _map; // true if the cell exists, otherwise false

    private PieceController[,] _pieceLocates; // piece locate 

    private bool editable = true; // editable

    private bool update = false; // is update

    private int updateTimestamp = 5;
    private float curTimestamp = 0f;

    private int leftTileCount = 0;

    private int maxLeftTileCount = 15;

    private List<Vector3Int> forwards3 = new List<Vector3Int>
    {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    private List<Vector2Int> forwards2 = new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    // Start is called before the first frame update
    void Awake()
    {
        InitTileMap();
    }

    /// <summary>
    /// Initialize map
    /// </summary>
    void InitTileMap()
    {
        _map = new bool[mapRect.width, mapRect.height];
        _pieceLocates = new PieceController[mapRect.width, mapRect.height];
        Map = GetComponent<Tilemap>();
        cellTile = Resources.Load("Tiles/center") as Tile;
        foreach (var pos in mapRect.allPositionsWithin)
        {
            _map[pos.x - mapRect.xMin, pos.y - mapRect.yMin] = true;
        }
        ReloadMap();
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
    public List<Vector2Int> FindPathToTarget(Vector3Int source, Vector3Int target)
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
            foreach (var direction in forwards2)
            {
                var next = front + direction;
                var nextInRect = next - mapRect.min;
                if (mapRect.Contains(next) && _map[nextInRect.x, nextInRect.y] && (!_pieceLocates[nextInRect.x, nextInRect.y] || target2.Equals(next))  && parent[nextInRect.x, nextInRect.y] == null)
                {
                    parent[nextInRect.x, nextInRect.y] = front;
                    if (target2.Equals(next))
                    {
                        Debug.Log(nextInRect);
                        while (parent[nextInRect.x, nextInRect.y].HasValue)
                        {
                            path.Add(nextInRect + mapRect.min);
                            if ((nextInRect + mapRect.min).Equals(source2)) break;
                            nextInRect = parent[nextInRect.x, nextInRect.y].Value - mapRect.min;
                            Debug.Log(nextInRect);
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
    /// 移除某个格子
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
                    foreach (var forward in forwards3)
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
    /// 顺时针旋转90°
    /// </summary>
    private void RotateRight90Degree()
    {
        int w = width / 2;
        int h = height;
        if (w == h)
        {
            // 先对角线翻转，再上下翻转就可以达到旋转90°的效果
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
    /// 将地图左半部分移动到与右边合并
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


    // Update is called once per frame
    void Update()
    {
    }
}
