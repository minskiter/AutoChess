using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class DataManager
{
    public static DataManager Instance = new DataManager();

    public Dictionary<int, List<GameObject>> piecePrefabs = new Dictionary<int, List<GameObject>>();

    public void Init()
    {
        DataTable data = CsvLoader.LoadCsv("Data/pieces.csv");
        foreach (DataRow row in data.Rows)
        {
            string name = row["name"].ToString();
            var gameObject = Resources.Load<GameObject>(Path.Combine("Prefab/Pieces/", row["model"].ToString()));
            var pieceController = gameObject.GetComponent<PieceController>();
            var star = int.Parse(row["star"].ToString());
            pieceController.CloneProperties(name, int.Parse(row["maxHealth"].ToString()), int.Parse(row["attack"].ToString()), float.Parse(row["attackDistance"].ToString()), float.Parse(row["attackInterval"].ToString()), star, float.Parse(row["moveSpeed"].ToString()),int.Parse(row["cost"].ToString()));
            if (!piecePrefabs.ContainsKey(star))
            {
                piecePrefabs[star] = new List<GameObject>();
            }
            piecePrefabs[star].Add(gameObject);
        }
    }
}
