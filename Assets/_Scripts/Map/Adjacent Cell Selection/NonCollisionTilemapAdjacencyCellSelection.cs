using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class NonCollisionTilemapAdjacencyCellSelection : TilemapAdjacencyCellSelection
{

    public NonCollisionTilemapAdjacencyCellSelection(GridXY<MapCellItem> grid, LayerMask wallLayerMask, float distance) : base(grid, wallLayerMask, distance)
    {
    }    
    
    public override bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        var fromPosition = Grid.GetWorldPositionOfNearestCell(from.XIndex, from.YIndex);
        var toPosition = Grid.GetWorldPositionOfNearestCell(to.XIndex, to.YIndex);
        var hit = Physics2D.Raycast(fromPosition , toPosition - fromPosition, Distance, WallLayerMask);

        return hit.transform == null;
    }

}