using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    DataTable data;
    public Dictionary<int,List<GameObject>> piecePrefabs = new Dictionary<int, List<GameObject>>();

    private void Awake()
    {
        Debug.Log("Load data.",gameObject);
        data = CsvLoader.LoadCsv("Data/pieces.csv");
        LoadCard();
    }

    private void LoadCard()
    {
        foreach (DataRow row in data.Rows)
        {
            string name = row["name"].ToString();
            var gameObject = Resources.Load<GameObject>(Path.Combine("Prefab/Pieces/", row["model"].ToString()));
            Debug.Log(gameObject);
            Debug.Log(Path.Combine("Prefab/Pieces/", row["model"].ToString()));
            var pieceController = gameObject.GetComponent<PieceController>();
            var star = int.Parse(row["star"].ToString());
            pieceController.CloneProperties(int.Parse(row["maxHealth"].ToString()), int.Parse(row["attack"].ToString()), float.Parse(row["attackDistance"].ToString()), float.Parse(row["attackInterval"].ToString()), int.Parse(row["star"].ToString()), float.Parse(row["moveSpeed"].ToString()));
            if (!piecePrefabs.ContainsKey(star)){
                piecePrefabs[star] = new List<GameObject>();
            }
            piecePrefabs[star].Add(gameObject);
        }
    }
}
