using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

static public class CsvLoader
{
    static public DataTable LoadCsv(string path)
    {
        //var lines = File.ReadLines(Path.Combine(Application.dataPath, path));
        Debug.Log(path);
        var lines = (Resources.Load(path) as TextAsset)?.text.Split('\n').ToList();
        Debug.Log(lines);
        var headers = lines.First().Split(',');
        var table = new DataTable();
        foreach (var header in headers){
            table.Columns.Add(header);
        }
        lines = (Resources.Load(path) as TextAsset)? .text.Split('\n').ToList();
        //lines = File.ReadLines(Path.Combine(Application.dataPath, path));
        foreach (var line in lines.Skip(1))
        {   
            var data = line.Split(',');
            if (data.Length>0)
                table.Rows.Add(data);
        }
        return table;
    }
}
