using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CardSlotManager : MonoBehaviour
{
    private Canvas canvas;
    private Object cardSlot;
    private Image card;
    private int[,] cards;  //卡牌数组   
    private int maxSize = 5; //卡槽容量
   
    void Start()
    {
        InitCardSlot();
    }

    //初始化卡槽
    void InitCardSlot() {
        for(int i = 1; i <= 5; i++) {
            GameObject obj = new GameObject();
            obj.transform.parent = this.transform;
            card = obj.AddComponent<Image>();
            card.sprite = Resources.Load("heros/bighero" + i, typeof(Sprite)) as Sprite;
            card.transform.localScale = new Vector3(3f,3f,1f);
        }
    
        



        // card = Resources.Load("card") as GameObject;
        // GameObject go = Instantiate(card) as GameObject;
        // go.transform.parent = this.transform;
    }

    


    void Update()
    {
        
    }
}
