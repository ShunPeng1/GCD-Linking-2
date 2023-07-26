using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class NonCollisionTilemapAdjacencyCellSelection : TilemapAdjacencyCellSelection
{

    public NonCollisionTilemapAdjacencyCellSelection(BaseCharacterMapDynamicGameObject characterMapDynamicGameObject,LayerMask wallLayerMask) : base(characterMapDynamicGameObject, wallLayerMask)
    {
    }    
    
    public override bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        VentMapGameObject fromVent = from.Item.GetFirstInCellGameObject<VentMapGameObject>();
        VentMapGameObject toVent = to.Item.GetFirstInCellGameObject<VentMapGameObject>();
        
        if (fromVent != null && toVent != null)
        {
            return to.Item.GetFirstInCellGameObject<BaseCharacterMapDynamicGameObject>() == null;
        }
        
        ExitMapGameObject fromExit = from.Item.GetFirstInCellGameObject<ExitMapGameObject>();
        if (fromExit != null) return false;


        var fromPosition = Grid.GetWorldPositionOfNearestCell(from.XIndex, from.YIndex);
        var toPosition = Grid.GetWorldPositionOfNearestCell(to.XIndex, to.YIndex);

        Character.Collider2D.enabled = false;
        var hit = Physics2D.Linecast(fromPosition, toPosition, WallLayerMask);
        Character.Collider2D.enabled = true;
        
        return hit.transform == null;
    }

}