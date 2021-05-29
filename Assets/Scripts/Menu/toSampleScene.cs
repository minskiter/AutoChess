using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class toSampleScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {

        SceneManager.LoadScene("Battle", LoadSceneMode.Single);

        var scene = SceneManager.GetSceneByName("Battle");
        SceneManager.sceneLoaded += (Scene sc, LoadSceneMode loadSceneMode) =>
        {
            SceneManager.SetActiveScene(scene);
        };
        //Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);
    }
}
