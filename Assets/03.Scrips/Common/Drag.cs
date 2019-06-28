using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Transform itemTr;
    Transform inventoryTr;
    Transform itemListTr;
    CanvasGroup canvasGroup;
    public static GameObject draggingItem = null;

    void Start()
    {
        itemTr = GetComponent<Transform>();
        inventoryTr = GameObject.Find("Inventory").GetComponent<Transform>();
        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();

        //Canvas Group 컴포넌트 
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //드래그 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        //드래그 이벤트가 발생하면 아이탬의 위치를 마우스 커서의 위치로 변경
        itemTr.position = Input.mousePosition;
    } 
 
    //드래그를 시작할 때 한번 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        //부모를 Inventory로 변경
        this.transform.SetParent(inventoryTr);
        //드래그가 시작되면 드래그 되는 아이템
        draggingItem = this.gameObject;
        //드래그가 시작 되면 다른UI이가 이벤트를 받지 않도록
        canvasGroup.blocksRaycasts = false;
    }
    
    //드래그가 종료 했을때 한번 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그가 종료되면 아이템 null
        draggingItem = null;
        //드래그가 종료되면 다른UI이벤트로 ItemList로 돌림
        canvasGroup.blocksRaycasts = true;
        //슬롯에 드래그하지 않았을때 원래대로 ItemList로 돌린다
        if (itemListTr.parent == inventoryTr)
        {
            itemTr.SetParent(itemListTr.transform);
        }
    }
}
