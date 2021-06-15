using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneManager : MonoSingleton<SceneManager>
{

    public void SwitchCheckpoint(string checkPointName)
    {
        if (checkPointName == null) return;
        var checkPointInt = int.Parse(checkPointName);
        if (checkPointInt > 1)
        {
            if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString($"{checkPointInt - 1}Checkpoint")))
            {
                return;
            }
        }

        DataManager.Instance.CurrentMap =
            DataManager.Instance.MapLists.FirstOrDefault(e => e.Name == checkPointName);
        if (DataManager.Instance.CurrentMap != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
        }
    }

    public void BackMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
