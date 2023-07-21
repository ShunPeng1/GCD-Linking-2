﻿using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class NonCollisionTilemapAdjacencyCellSelection : TilemapAdjacencyCellSelection
{

    public NonCollisionTilemapAdjacencyCellSelection(GridXY<MapCellItem> grid, LayerMask wallLayerMask) : base(grid, wallLayerMask)
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
        var hit = Physics2D.Linecast(fromPosition, toPosition, WallLayerMask);
        return hit.transform == null;
    }

}