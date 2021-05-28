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
        var lines = File.ReadLines(Path.Combine(Application.dataPath, path));
        var headers = lines.First().Split('\t');
        var table = new DataTable();
        foreach (var header in headers){
            table.Columns.Add(header);
        }
        lines = File.ReadLines(Path.Combine(Application.dataPath, path));
        foreach (var line in lines.Skip(1))
        {   
            var data = line.Split('\t');
            if (data.Length>0)
                table.Rows.Add(data);
        }
        return table;
    }
}
