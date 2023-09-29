using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class MapCellItem
{
    protected GridXY<MapCellItem> Grid;
    protected GridXYCell<MapCellItem> Cell;

    private List<MonoBehaviour> _inCellMonoBehaviours = new();
    public CellSelectHighlighter CellSelectHighlighter;
    
    public MapCellItem(GridXY<MapCellItem> grid, GridXYCell<MapCellItem> cell)
    {
        Grid = grid;
        Cell = cell;
    }

    public TMonoBehaviour GetFirstInCellGameObject<TMonoBehaviour>() where TMonoBehaviour : MonoBehaviour
    {
        foreach (var inCellGameObject in _inCellMonoBehaviours)
        {
            if (inCellGameObject is TMonoBehaviour behaviour) return behaviour;
        }

        return default;
    }
    
    public void AddInCellGameObject(MonoBehaviour gameObject)
    {
        _inCellMonoBehaviours.Add(gameObject);
    }

    public void RemoveInCellGameObject(MonoBehaviour gameObject)
    {
        _inCellMonoBehaviours.Remove(gameObject);
    }
    
}
    