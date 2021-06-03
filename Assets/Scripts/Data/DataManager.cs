using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Assets.Scripts.Data;
using UnityEngine;

public class DataManager
{
    public static DataManager Instance = new DataManager();

    public Dictionary<int, List<GameObject>> PiecePrefabs;

    public List<GameMap> MapLists;

    public GameMap CurrentMap;

    public void Init()
    {
        PiecePrefabs = new Dictionary<int, List<GameObject>>();
        MapLists = new List<GameMap>();
        var data = CsvLoader.LoadCsv("Data/pieces.csv");
        foreach (DataRow row in data.Rows)
        {
            var name = row["name"].ToString();
            var gameObject = Resources.Load<GameObject>(Path.Combine("Prefab/Pieces/", row["model"].ToString()));
            var pieceController = gameObject.GetComponent<PieceController>();
            var star = int.Parse(row["star"].ToString());
            pieceController.CloneProperties(name, int.Parse(row["maxHealth"].ToString()), int.Parse(row["attack"].ToString()), float.Parse(row["attackDistance"].ToString()), float.Parse(row["attackInterval"].ToString()), star, float.Parse(row["moveSpeed"].ToString()), int.Parse(row["cost"].ToString()));
            if (!PiecePrefabs.ContainsKey(star))
            {
                PiecePrefabs[star] = new List<GameObject>();
            }
            PiecePrefabs[star].Add(gameObject);
        }

        var mapNameList = MapLoader.LoadMapPathList("Data/mapList.csv");
        foreach (var mapPath in mapNameList)
        {
            Debug.Log($"load {mapPath}");
            MapLists.Add(MapLoader.LoadMap(mapPath));
        }
    }
}
