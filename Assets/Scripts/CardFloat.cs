using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardFloat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject myPrefab;
    //最初位置
    private float x;
    private float y;
    private bool flag = false;  //标记是否点击卡牌了
    private float aimX;
    private float aimY;
    Vector3 mousePositionOnScreen;//获取到点击屏幕的屏幕坐标
    
    //鼠标移入
    public void OnPointerEnter(PointerEventData eventData)
    {
        // x = transform.position.x;
        // y = transform.position.y;
        // transform.localScale = new Vector2(3.2f, 3.2f);
        // transform.position = new Vector2(x, y + 220f);
    }
    //鼠标移出
    public void OnPointerExit(PointerEventData eventData)
    {
        // transform.localScale = new Vector2(1f, 1f);
        // transform.position = new Vector2(x, y);
    }
  
    public void OnPointerClick(PointerEventData eventData)
    {
       
        if(!flag) {
            x = transform.position.x;
            y = transform.position.y;
            transform.localScale = new Vector2(1.3f, 1.3f);
            transform.position = new Vector2(x, y + 45f);
            flag = true;
        }
        else {
            transform.localScale = new Vector2(1f, 1f);
            transform.position = new Vector2(x, y);
            flag = false;
        }
        
    }

    

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButton(0)) {
        //     mousePositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);;
        //     if(flag) {
        //         GameObject sunFlower = Instantiate(myPrefab);
        //         sunFlower.transform.position = new Vector2(0, 0);
             
        //         transform.localScale = new Vector2(1f, 1f);
        //         transform.position = new Vector2(x, y);
        //         flag = false;
        //     }
        // }
    }
}
