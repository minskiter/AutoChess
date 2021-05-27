using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;

using UnityEngine.EventSystems;


public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDrag = false;
    //偏移量
    private Vector3 offset = Vector3.zero;

    public CardSlotManager cardSlotManager = new CardSlotManager();

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = false;
        SetDragObjPostion(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
        SetDragObjPostion(eventData); 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetDragObjPostion(eventData);
    }



    void SetDragObjPostion(PointerEventData eventData)
    {
        // RectTransform rect = this.GetComponent<RectTransform>();
        // Vector3 mouseWorldPosition;

        //判断是否点到UI图片上的时候
        // if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out mouseWorldPosition))
        // {
        //     if (isDrag)
        //     {
        //         rect.position = mouseWorldPosition + offset;
        //     }
        //     else
        //     { 
	    //        //计算偏移量
        //         offset = rect.position - mouseWorldPosition;
        //     }

        //     // 直接赋予position点到的时候回跳动
        //     // rect.position = mouseWorldPosition;
        // }
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        curPosition.z = 0;
        transform.position = curPosition;    
    }

}
