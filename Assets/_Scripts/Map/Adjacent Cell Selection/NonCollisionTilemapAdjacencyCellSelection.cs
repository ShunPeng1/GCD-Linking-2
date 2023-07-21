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
        VentMapGameObject fromVent = from.Item.GetFirstInCellGameObject<VentMapGameObject>();
        VentMapGameObject toVent = to.Item.GetFirstInCellGameObject<VentMapGameObject>();
        if (fromVent != null && toVent != null)
        {
            return fromVent.IsOpen && toVent.IsOpen;
        } 
        
        var fromPosition = Grid.GetWorldPositionOfNearestCell(from.XIndex, from.YIndex);
        var toPosition = Grid.GetWorldPositionOfNearestCell(to.XIndex, to.YIndex);
        var hit = Physics2D.Raycast(fromPosition , toPosition - fromPosition, Distance, WallLayerMask);

        return hit.transform == null;
    }

}