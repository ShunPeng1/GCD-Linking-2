
using System;

using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;


public class CellHighlightSelectMouseInput
{
    protected Vector3 MouseWorldPosition;
    protected RaycastHit2D[] MouseCastHits;

    
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
    
    public virtual void UpdateMouseInput()
    {
        UpdateMousePosition();
        CastMouse();
        UpdateHoverObject();

        if (!Input.GetMouseButtonDown(0)) return;
        
        if (LastHoverCellSelectHighlighter != null) LastHoverCellSelectHighlighter.EndHover();
        InvokeFinishedSelection(FindFirstIMouseInteractableInMouseCast());

    }
    
    protected void UpdateHoverObject()
    {
        var hoveringMouseInteractable = FindFirstIMouseInteractableInMouseCast();
        if (hoveringMouseInteractable != LastHoverCellSelectHighlighter)
        {
            if (LastHoverCellSelectHighlighter != null) LastHoverCellSelectHighlighter.EndHover();
            if (hoveringMouseInteractable != null) hoveringMouseInteractable.StartHover();
            LastHoverCellSelectHighlighter = hoveringMouseInteractable;
        }
    }
    
    protected virtual CellSelectHighlighter FindFirstIMouseInteractableInMouseCast()
    {
        foreach (var hit in MouseCastHits)
        {
            var characterCardButton = hit.transform.gameObject.GetComponent<BaseCardButton>();
            if (characterCardButton != null && characterCardButton.Interactable)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return null;
            }

            var characterCardGameObject = hit.transform.gameObject.GetComponent<BaseCardGameObject>();
            if (characterCardGameObject != null && characterCardGameObject.Interactable)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return null;
            }

            MapCellItem mapCellItem = Grid.GetCell(MouseWorldPosition).Item;
            CellSelectHighlighter cellSelectHighlighter = mapCellItem.CellSelectHighlighter;
            
            if (cellSelectHighlighter != null  && cellSelectHighlighter.Interactable)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return cellSelectHighlighter;
            }
        }

        return null;
    }

    #region CAST

    protected void UpdateMousePosition()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseWorldPosition = new Vector3(worldMousePosition.x, worldMousePosition.y, 0);
    }

    protected void CastMouse()
    {
        MouseCastHits = Physics2D.RaycastAll(MouseWorldPosition, Vector2.zero);
    }

    #endregion
    
}
