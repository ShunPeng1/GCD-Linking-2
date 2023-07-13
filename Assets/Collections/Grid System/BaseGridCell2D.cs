using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGridCell2D<TItem>
{
    [Header("Base")]
    public List<BaseGridCell2D<TItem>> AdjacentCells = new();
    public TItem Item;
    public bool IsObstacle;

    [Header("A Star Pathfinding")] 
    public BaseGridCell2D<TItem> ParentXZCell2D = null; 
    public double FCost;
    public double HCost;
    public double GCost;


    protected BaseGridCell2D(TItem item = default)
    {
        Item = item;
    }

    public void SetAdjacency(BaseGridCell2D<TItem>[] adjacentRawCells)
    {
        foreach (var adjacentCell in adjacentRawCells)
        {
            SetAdjacency(adjacentCell);
        }
    }
    
    public void SetAdjacency(BaseGridCell2D<TItem> adjacentCell)
    {
        if (!AdjacentCells.Contains(adjacentCell)) AdjacentCells.Add(adjacentCell);
    }
    
    public void RemoveAdjacency(BaseGridCell2D<TItem>[] adjacentRawCells)
    {
        foreach (var adjacentCell in adjacentRawCells)
        {
            RemoveAdjacency(adjacentCell);
        }
    }
    
    public void RemoveAdjacency(BaseGridCell2D<TItem> adjacentCell)
    {
        if (AdjacentCells.Contains(adjacentCell)) AdjacentCells.Remove(adjacentCell);
    }

}
