using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneManager : MonoBehaviour
{
    public void SwitchCheckpoint(string checkPointName)
    {
        if (checkPointName != null)
        {
            DataManager.Instance.CurrentMap = DataManager.Instance.MapLists.FirstOrDefault(e => e.Name == checkPointName);
            if (DataManager.Instance.CurrentMap != null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
            }
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
