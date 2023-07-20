
using System;
using _Scripts.Cards.Card_UI;
using _Scripts.Managers;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;

public class CellHighlightAndCardMouseInput : BaseCardMouseInput
{
    private Action<CellHighlighter> _finishedSelection;
    protected void InitializeEnd(Action<CellHighlighter> finishedSelection)
    {
        _finishedSelection = finishedSelection;
    }

    
    
    protected override void Update()
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

    protected bool CheckCellHighlight()
    {
        var cellHighlighter = FindFirstInMouseCast<CellHighlighter>();

        if (cellHighlighter == null || !cellHighlighter.Interactable) return false;
        
        cellHighlighter.Select();
        return true;

    }
    
    protected override IMouseInteractable GetMouseInteractableInCastMouse()
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
            
            var cellHighlighter = hit.transform.gameObject.GetComponent<CellHighlighter>();
            if (cellHighlighter != null  && cellHighlighter.Interactable)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return cellHighlighter;
            }
        }

        return null;
    }

    
}
