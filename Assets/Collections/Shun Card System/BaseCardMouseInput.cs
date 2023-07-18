using System.Collections;
using System.Collections.Generic;
using Shun_Card_System;
using UnityEngine;
using UnityUtilities;

public class BaseCardMouseInput : MonoBehaviour
{
    protected Vector3 MouseWorldPosition;
    
    [Header("Drag Objects")]
    protected bool IsDragging = false;
    protected Vector3 CardOffset;
    protected BaseCardGameObject DraggingCard;
    protected BaseCardPlaceRegion LastCardPlaceRegion;
    protected BaseCardPlaceHolder LastCardPlaceHolder;
    
    protected void Update()
    {
        UpdateMousePosition();
        
        if (Input.GetMouseButtonDown(0))
        {
           StartDragMouse();
        }

        if (Input.GetMouseButton(0))
        {
            DragMouse();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDragMouse();
        }
    }

    protected void UpdateMousePosition()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseWorldPosition = new Vector3(worldMousePosition.x, worldMousePosition.y, 0);
    }

    protected TResult CastMouseFindFirst<TResult>()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPosition, Vector2.zero);

        foreach (var hit in hits)
        {
            var result = hit.transform.gameObject.GetComponent<TResult>();
            if (result != null)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return result;
            }
        }

        //Debug.Log("Mouse cannot find "+ typeof(TResult));
        return default;
    }

    protected void StartDragMouse()
    {
        DraggingCard = CastMouseFindFirst<BaseCardGameObject>();
        LastCardPlaceHolder = CastMouseFindFirst<BaseCardPlaceHolder>();
        LastCardPlaceRegion = CastMouseFindFirst<BaseCardPlaceRegion>();

        if (DraggingCard == null) return;
        
        CardOffset = DraggingCard.transform.position - MouseWorldPosition;
        IsDragging = true;

        DraggingCard.Select();
        
        if (LastCardPlaceHolder == null)
        {
            LastCardPlaceRegion = null;
            return;
        }
        if (!LastCardPlaceRegion.TakeOutTemporary(DraggingCard, LastCardPlaceHolder))
        {
            LastCardPlaceHolder = null;
            LastCardPlaceRegion = null;
        }

    }

    protected void DragMouse()
    {
        if (!IsDragging) return; 
        
        DraggingCard.transform.position = MouseWorldPosition + CardOffset;
        
    }

    protected void EndDragMouse()
    {
        if (!IsDragging) return;
        
        DraggingCard.Deselect();
        AddCardToHolder();

        DraggingCard = null;
        LastCardPlaceHolder = null;
        LastCardPlaceRegion = null;
        IsDragging = false;

    }

    protected void AddCardToHolder()
    {
        
        var placeRegion = CastMouseFindFirst<BaseCardPlaceRegion>();
        var placeHolder = CastMouseFindFirst<BaseCardPlaceHolder>();
        
        if (placeHolder == null)
        {
            if (placeRegion != null && placeRegion != LastCardPlaceRegion && placeRegion.AddCard(DraggingCard, placeHolder))
            {
                if (LastCardPlaceHolder != null)
                {
                    LastCardPlaceRegion.RemoveTemporary(DraggingCard);
                    return;
                }
            }
            
            if (LastCardPlaceRegion != null) 
                LastCardPlaceRegion.ReAddTemporary(DraggingCard);
        }
        else
        {
            if (!placeRegion.AddCard(DraggingCard, placeHolder))
            {
                if(LastCardPlaceRegion != null) LastCardPlaceRegion.ReAddTemporary(DraggingCard);
            }
            
            if (LastCardPlaceHolder != null)
            {
                LastCardPlaceRegion.RemoveTemporary(DraggingCard);
            }

        }

    }
    
}
