using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class CardSlotManager : MonoBehaviour
// IPointerClickHandler
{
    private Image card;
    // public Image[] cards;  

    public List<CardController> cardList;
    private int maxSize = 5; //卡槽容量

    // private bool isClick = false;  //标记是否点击卡牌了

    // //点击卡牌的坐标
    // private float x;
    // private float y;
    // private float z;
    //点击卡牌的id
    private int id = -1;

   
    void Start()
    {
      
        InitCardSlot();
  
    }

    //初始化卡槽
    void InitCardSlot() {
        for(int i = 0; i < maxSize; i++) {
            GameObject obj = new GameObject();
            obj.name = "bighero" + (i + 1);
            obj.transform.parent = this.transform;
            obj.AddComponent<CardController>();
            cardList.Add(obj.GetComponent<CardController>());
          
            card = obj.AddComponent<Image>();
            card.sprite = Resources.Load("heros/bighero" + (i + 1), typeof(Sprite)) as Sprite;
            card.transform.localScale = new Vector3(3f,3f,1f);
        }

    }




//     //点击事件
//     public void OnPointerClick(PointerEventData eventData)
//     {
//         GameObject obj = null;
//         PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
//         eventDataCurrentPosition.position = new Vector2
//         (
//             Input.mousePosition.x, Input.mousePosition.y
//         );
//         List<RaycastResult> results = new List<RaycastResult>();
//         EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
//         if(results.Count > 0) {
//             obj = results[0].gameObject;  //获取点击卡牌属性
//         }
  
//     }

   

    
    void Update()
    {
        
    }
}
