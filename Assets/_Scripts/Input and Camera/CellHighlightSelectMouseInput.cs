
using System;
using System.Collections.Generic;
using Shun_Card_System;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;


public class CellHighlightSelectMouseInput : BaseCardMouseInput
{

    protected GridXY<MapCellItem> Grid;
    protected Action<CellSelectHighlighter> FinishedSelection;

    protected CellSelectHighlighter LastHoverCellSelectHighlighter;
    public CellHighlightSelectMouseInput(GridXY<MapCellItem> grid,Action<CellSelectHighlighter> finishedSelection)
    {
        Grid = grid;
        FinishedSelection = finishedSelection;
    }

    public virtual void AddFinishedSelection(Action<CellSelectHighlighter> finishedSelection)
    {
        FinishedSelection += finishedSelection;
    }
    
    public virtual void AddFinishedSelection(Action finishedSelection)
    {
        FinishedSelection += (_) => finishedSelection.Invoke(); 
    }

    protected virtual void InvokeFinishedSelection(CellSelectHighlighter cellSelectHighlighter)
    {
        FinishedSelection.Invoke(cellSelectHighlighter);
    }
    
    public override void UpdateMouseInput()
    {
        UpdateMousePosition();
        CastMouse();
        UpdateHoverObject();

        if (!Input.GetMouseButtonDown(0)) return;
        
        if (LastHoverCellSelectHighlighter != null) LastHoverCellSelectHighlighter.EndHover();
        InvokeFinishedSelection(FindFirstCellSelectInMouseCast());

    }

    protected override void UpdateHoverObject()
    {
        var hoveringMouseInteractableGameObject = FindFirstCellSelectInMouseCast();

        if (hoveringMouseInteractableGameObject == LastHoverCellSelectHighlighter) return;
        
        if (LastHoverCellSelectHighlighter != null) LastHoverCellSelectHighlighter.EndHover();
        if (hoveringMouseInteractableGameObject != null) hoveringMouseInteractableGameObject.StartHover();

        LastHoverCellSelectHighlighter = hoveringMouseInteractableGameObject;
    }

    protected virtual CellSelectHighlighter FindFirstCellSelectInMouseCast()
    {
        CellSelectHighlighter result = null;
        
        foreach (var hit in MouseCastHits)
        {
            var characterCardButton = hit.transform.gameObject.GetComponent<BaseCardButton>();
            if (characterCardButton != null && characterCardButton.Interactable)
            {
                return null;
            }

            var characterCardGameObject = hit.transform.gameObject.GetComponent<BaseCardGameObject>();
            if (characterCardGameObject != null && characterCardGameObject.Interactable)
            {
                return null;
            }

            MapCellItem mapCellItem = Grid.GetCell(MouseWorldPosition)?.Item;
            CellSelectHighlighter cellSelectHighlighter = mapCellItem?.CellSelectHighlighter;
            
            if (cellSelectHighlighter != null  && cellSelectHighlighter.Interactable)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                result = cellSelectHighlighter;
            }
        }

        return result;
    }

    
    
}
