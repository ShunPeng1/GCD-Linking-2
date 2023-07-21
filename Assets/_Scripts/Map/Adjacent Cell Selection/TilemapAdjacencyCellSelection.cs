using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class TilemapAdjacencyCellSelection : IPathFindingAdjacentCellSelection<GridXYCell<MapCellItem>, MapCellItem>
{
    protected GridXY<MapCellItem> Grid;
    protected LayerMask WallLayerMask;
    

    public TilemapAdjacencyCellSelection(GridXY<MapCellItem> grid, LayerMask wallLayerMask)
    {
        Grid = grid;
        WallLayerMask = wallLayerMask;
    }

    public virtual bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        return true;
    }
}
