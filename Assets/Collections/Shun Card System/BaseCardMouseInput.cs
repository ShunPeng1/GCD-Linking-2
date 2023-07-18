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
    protected BaseCardRegion LastCardRegion;
    protected BaseCardHolder LastCardHolder;
    protected BaseCardButton LastCardButton;

    protected RaycastHit2D[] MouseCastHits;

        protected void Update()
    {
        UpdateMousePosition();
        
        if (Input.GetMouseButtonDown(0))
        {
            CastMouse();
            StartDragMouse();
        }

        if (Input.GetMouseButton(0))
        {
            CastMouse();
            DragMouse();
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastMouse();
            EndDragMouse();
        }
    }

    protected void UpdateMousePosition()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseWorldPosition = new Vector3(worldMousePosition.x, worldMousePosition.y, 0);
    }

    
    private void CastMouse()
    {
        MouseCastHits = Physics2D.RaycastAll(MouseWorldPosition, Vector2.zero);
    }
    
    protected TResult FindFirstInMouseCast<TResult>()
    {
        foreach (var hit in MouseCastHits)
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
        // Check for button first
        LastCardButton = FindFirstInMouseCast<BaseCardButton>();

        if (LastCardButton != null && LastCardButton.Interactable)
        {
            LastCardButton.Execute();
            return;
        } 

        // Check for card game object second
        DraggingCard = FindFirstInMouseCast<BaseCardGameObject>();

        if (DraggingCard == null || !DraggingCard.Interactable)
        {
            DraggingCard = null;
            return;
        }
        
        CardOffset = DraggingCard.transform.position - MouseWorldPosition;
        IsDragging = true;

        DraggingCard.Select();
        
        // Check the card region base on card game object or card holder, to TakeOutTemporary
        LastCardRegion = FindFirstInMouseCast<BaseCardRegion>();
        if (LastCardRegion == null)
        {
            LastCardHolder = FindFirstInMouseCast<BaseCardHolder>();
            if (LastCardHolder == null)
            {
                return;
            }

            LastCardRegion = LastCardHolder.CardRegion;
        }
        else
        {
            LastCardHolder = LastCardRegion.FindCardPlaceHolder(DraggingCard);
        }

        // Having got the region and holder, take the card out temporary
        if (LastCardRegion.TakeOutTemporary(DraggingCard, LastCardHolder)) return;
        
        LastCardHolder = null;
        LastCardRegion = null;

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
        LastCardHolder = null;
        LastCardRegion = null;
        IsDragging = false;

    }

    protected void AddCardToHolder()
    {
        
        var dropRegion = FindFirstInMouseCast<BaseCardRegion>();
        var dropHolder = FindFirstInMouseCast<BaseCardHolder>();
        
        if (dropHolder == null)
        {
            if (dropRegion != null && dropRegion != LastCardRegion 
                                   && dropRegion.AddCard(DraggingCard, dropHolder)) // Successfully add to the drop region
            {
                if (LastCardHolder != null) // remove the temporary in last region
                {
                    LastCardRegion.RemoveTemporary(DraggingCard);
                    return;
                }
            }
            
            if (LastCardRegion != null) // Unsuccessfully add to drop region or it is the same region
                LastCardRegion.ReAddTemporary(DraggingCard);
        }
        else
        {
            if (dropRegion == null) dropRegion = dropHolder.CardRegion;
            
            if (!dropRegion.AddCard(DraggingCard, dropHolder))
            {
                if(LastCardRegion != null) LastCardRegion.ReAddTemporary(DraggingCard);
            }
            
            if (LastCardHolder != null)
            {
                LastCardRegion.RemoveTemporary(DraggingCard);
            }

        }

    }
    
}
