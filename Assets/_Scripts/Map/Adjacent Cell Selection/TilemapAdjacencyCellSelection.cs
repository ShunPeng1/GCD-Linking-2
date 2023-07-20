using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class TilemapAdjacencyCellSelection : IPathFindingAdjacentCellSelection<GridXYCell<MapCellItem>, MapCellItem>
{
    protected GridXY<MapCellItem> Grid;
    protected LayerMask WallLayerMask;
    protected float Distance;

    public TilemapAdjacencyCellSelection(GridXY<MapCellItem> grid, LayerMask wallLayerMask, float distance)
    {
        Grid = grid;
        WallLayerMask = wallLayerMask;
        Distance = distance;
    }

    public virtual bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        return true;
    }
}
