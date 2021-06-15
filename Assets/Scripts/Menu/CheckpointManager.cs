using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointManager : MonoBehaviour
{
    public List<GameObject> lists;

    void OnEnable()
    {
        foreach (var stage in lists)
        {
            var checkpoint = int.Parse(stage.name.Replace("Stage", ""));
            var image = stage.transform.GetChild(0).gameObject;
            if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString($"{checkpoint}Checkpoint")))
            {
                image.SetActive(false);
            }
            else
            {
                image.SetActive(true);
            }
        }
    }
}
