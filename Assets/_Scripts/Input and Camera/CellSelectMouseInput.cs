
using System;
using System.Collections.Generic;
using Shun_Card_System;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityEngine.EventSystems;


public class CellSelectMouseInput : BaseCardMouseInput
{

    protected GridXY<MapCellItem> Grid;
    protected Action<GridXYCell<MapCellItem>> FinishedSelectionFunc;
    protected Func<GridXYCell<MapCellItem>, bool> CellSelectionFunc;
    protected CellSelectHighlighter LastHoverCellSelectHighlighter;
    public CellSelectMouseInput(GridXY<MapCellItem> grid, Func<GridXYCell<MapCellItem>, bool> cellSelectionFunc,Action<GridXYCell<MapCellItem>> finishedSelectionFunc)
    {
        Grid = grid;
        CellSelectionFunc = cellSelectionFunc;
        FinishedSelectionFunc = finishedSelectionFunc;
    }

    public virtual void AddFinishedSelection(Action<GridXYCell<MapCellItem>> finishedSelection)
    {
        FinishedSelectionFunc += finishedSelection;
    }
    
    public virtual void AddFinishedSelection(Action finishedSelection)
    {
        FinishedSelectionFunc += (_) => finishedSelection.Invoke(); 
    }

    protected virtual void InvokeFinishedSelection(GridXYCell<MapCellItem> cellSelectHighlighter)
    {
        FinishedSelectionFunc.Invoke(cellSelectHighlighter);
    }
    
    public override void UpdateMouseInput()
    {
        UpdateMousePosition();
        CastMouse();
        UpdateHoverObject();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (LastHoverCellSelectHighlighter != null) LastHoverCellSelectHighlighter.EndHover();
        
            var foundCell = FindFirstGridCellInMouseCast();
            if (foundCell != null) InvokeFinishedSelection(foundCell);
        }

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

    protected GridXYCell<MapCellItem> FindFirstGridCellInMouseCast()
    {
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

            GridXYCell<MapCellItem> gridXYCell = Grid.GetCell(MouseWorldPosition);
            CellSelectHighlighter cellSelectHighlighter = gridXYCell?.Item?.CellSelectHighlighter;
            
            if (cellSelectHighlighter != null  && cellSelectHighlighter.Interactable)
            {
                if (CellSelectionFunc.Invoke(gridXYCell)) return gridXYCell;
            }
        }

        return null;
    }


}
