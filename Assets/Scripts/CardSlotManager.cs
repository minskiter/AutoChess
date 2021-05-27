using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class CardSlotManager : MonoBehaviour, IPointerClickHandler
{
    private Image card;
    public Image[] cards;  
    private int maxSize = 5; //卡槽容量

    private bool isClick = false;  //标记是否点击卡牌了

    //点击卡牌的坐标
    private float x;
    private float y;
    private float z;
    //点击卡牌的id
    private int id = -1;

   
    void Start()
    {
        cards = new Image[maxSize]; 
        InitCardSlot();
    }

    //初始化卡槽
    void InitCardSlot() {
        for(int i = 0; i < maxSize; i++) {
            GameObject obj = new GameObject();
            obj.name = "bighero" + (i + 1);
            obj.transform.parent = this.transform;
            card = obj.AddComponent<Image>();
            card.sprite = Resources.Load("heros/bighero" + (i + 1), typeof(Sprite)) as Sprite;
            card.transform.localScale = new Vector3(3f,3f,1f);
            cards[i] = card;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject obj = null;
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2
        (
            Input.mousePosition.x, Input.mousePosition.y
        );
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if(results.Count > 0) {
            obj = results[0].gameObject;  //获取点击卡牌属性
        }
  
        //点击突出逻辑
        if(id == -1 || id == (int)obj.name[obj.name.Length - 1]) {
            id = (int)obj.name[obj.name.Length - 1];
            if(isClick == false) {
                x = obj.transform.position.x;
                y = obj.transform.position.y;
                z = obj.transform.position.z;
                obj.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
    
                obj.transform.position = new Vector3(x, y + 10f, z);
                isClick = true;
            }
            else {
                obj.transform.localScale = new Vector3(3f, 3f, 1f);
                obj.transform.position = new Vector3(x, y, z);
                isClick = false;
                id = -1;
            }
        }
       
        
    }

    
    void Update()
    {
        
    }
}
