using System.Collections;
using System.Collections.Generic;
using Shun_Card_System;
using UnityEngine;
using UnityUtilities;

public class BaseCardMouseInput 
{
    protected Vector3 MouseWorldPosition;
    protected RaycastHit2D[] MouseCastHits;
    
    [Header("Hover Objects")]
    protected IMouseInteractable LastHoverMouseInteractable = null;
    public bool IsHoveringCard => LastHoverMouseInteractable != null;

    [Header("Drag Objects")]
    protected Vector3 CardOffset;
    protected BaseCardGameObject DraggingCard;
    protected BaseCardRegion LastCardRegion;
    protected BaseCardHolder LastCardHolder;
    protected BaseCardButton LastCardButton;

    public bool IsDraggingCard
    {
        get;
        private set;
    }

    
    protected virtual void Update()
    {
        UpdateMousePosition();
        if(!IsDraggingCard) UpdateHoverObject();
        
        if (Input.GetMouseButtonDown(0))
        {
            CastMouse();
            StartDragCard();
        }

        if (Input.GetMouseButton(0))
        {
            CastMouse();
            DragCard();
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastMouse();
            EndDragCard();
        }
    }

    protected void UpdateMousePosition()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseWorldPosition = new Vector3(worldMousePosition.x, worldMousePosition.y, 0);
    }

    #region Hover

    protected void UpdateHoverObject()
    {
        CastMouse();
        var hoveringMouseInteractable = GetMouseInteractableInCastMouse();
        if (hoveringMouseInteractable != LastHoverMouseInteractable)
        {
            LastHoverMouseInteractable?.EndHover();
            hoveringMouseInteractable?.StartHover();
            LastHoverMouseInteractable = hoveringMouseInteractable;
        }
    }
    
    
    protected virtual IMouseInteractable GetMouseInteractableInCastMouse()
    {
        foreach (var hit in MouseCastHits)
        {
            var characterCardButton = hit.transform.gameObject.GetComponent<BaseCardButton>();
            if (characterCardButton != null && characterCardButton.Interactable)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return characterCardButton;
            }
            
            var characterCardGameObject = hit.transform.gameObject.GetComponent<BaseCardGameObject>();
            if (characterCardGameObject != null && characterCardGameObject.Interactable)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return characterCardGameObject;
            }
        }

        return null;
    }

    #endregion
    
    
    protected void CastMouse()
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

    
    protected bool StartDragCard()
    {
        // Check for button first
        LastCardButton = FindFirstInMouseCast<BaseCardButton>();

        if (LastCardButton != null && LastCardButton.Interactable)
        {
            LastCardButton.Select();
            return true;
        } 

        // Check for card game object second
        DraggingCard = FindFirstInMouseCast<BaseCardGameObject>();

        if (DraggingCard == null || !DraggingCard.Interactable)
        {
            DraggingCard = null;
            return false;
        }
        
        CardOffset = DraggingCard.transform.position - MouseWorldPosition;
        IsDraggingCard = true;

        DraggingCard.Select();

        DetachCardToHolder();

        return true;
    }

    protected void DragCard()
    {
        if (!IsDraggingCard) return; 
        
        DraggingCard.transform.position = MouseWorldPosition + CardOffset;
        
    }

    protected void EndDragCard()
    {
        if (!IsDraggingCard) return;
        
        DraggingCard.Deselect();
        AttachCardToHolder();

        DraggingCard = null;
        LastCardHolder = null;
        LastCardRegion = null;
        IsDraggingCard = false;

    }
    
    protected void DetachCardToHolder()
    {
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

    protected void AttachCardToHolder()
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
