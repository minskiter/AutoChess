using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;

public class DataManager : GlobalSingleton<DataManager>
{
    public Dictionary<int, List<GameObject>> PiecePrefabs;

    public List<GameMap> MapLists;

    public GameMap CurrentMap;

    public Dictionary<string, AudioClip> AttackSource = new Dictionary<string, AudioClip>();

    private static bool _init;

    public void Init()
    {
        if (_init) return;
        _init = true;
        PiecePrefabs = new Dictionary<int, List<GameObject>>();
        MapLists = new List<GameMap>();
        var data = CsvLoader.LoadCsv("Data/pieces");
        foreach (DataRow row in data.Rows)
        {
            var name = row["name"].ToString();
            var gameObject = Resources.Load<GameObject>(Path.Combine("Prefab/Pieces/", row["model"].ToString()));
            var pieceController = gameObject.GetComponent<PieceController>();
            var star = int.Parse(row["star"].ToString());
            Debug.Log(row["model"].ToString());
            pieceController.CloneProperties(name, int.Parse(row["maxHealth"].ToString()), int.Parse(row["attack"].ToString()), float.Parse(row["attackDistance"].ToString()), float.Parse(row["attackInterval"].ToString()), star, float.Parse(row["moveSpeed"].ToString()), int.Parse(row["cost"].ToString()));
            if (!PiecePrefabs.ContainsKey(star))
            {
                PiecePrefabs[star] = new List<GameObject>();
            }
            PiecePrefabs[star].Add(gameObject);
        }

        var mapNameList = MapLoader.LoadMapPathList("Data/mapList");
        foreach (var mapPath in mapNameList)
        {
            MapLists.Add(MapLoader.LoadMap(mapPath));
        }
        // 暴力加载资源
        AttackSource.Add("warrior", Resources.Load<AudioClip>("Audio/Warrior"));
        AttackSource.Add("Arrow-damage", Resources.Load<AudioClip>("Audio/Arrow-damage"));
        AttackSource.Add("Arrow-start", Resources.Load<AudioClip>("Audio/Arrow-start"));
        AttackSource.Add("duelist", Resources.Load<AudioClip>("Audio/duelist"));
        AttackSource.Add("duelistL", Resources.Load<AudioClip>("Audio/duelistL"));
        AttackSource.Add("boom", Resources.Load<AudioClip>("Audio/boom"));
    }
}
