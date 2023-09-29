using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class CollectorTilemapAdjacencyCellSelection : TilemapAdjacencyCellSelection
{

    public CollectorTilemapAdjacencyCellSelection(BaseCharacterMapDynamicGameObject characterMapDynamicGameObject,LayerMask wallLayerMask) : base(characterMapDynamicGameObject, wallLayerMask)
    {
    }    
    
    public override bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        VentMapGameObject fromVent = from.Item.GetFirstInCellGameObject<VentMapGameObject>();
        VentMapGameObject toVent = to.Item.GetFirstInCellGameObject<VentMapGameObject>();
        
        
        var fromPosition = Grid.GetWorldPositionOfNearestCell(from.XIndex, from.YIndex);
        var toPosition = Grid.GetWorldPositionOfNearestCell(to.XIndex, to.YIndex);
        
        
        Character.Collider2D.enabled = false;
        var hit = Physics2D.Linecast(fromPosition, toPosition, WallLayerMask);
        Character.Collider2D.enabled = true;
        
        
        ExitMapGameObject fromExit = from.Item.GetFirstInCellGameObject<ExitMapGameObject>();
        if (fromExit != null) return false;

        if (fromVent != null && toVent != null) 
        {
            return true;
        }
        else if (fromVent != null)
        {
            return hit.transform == null;
        }

        var fromCharacter = from.Item.GetFirstInCellGameObject<BaseCharacterMapDynamicGameObject>();
        if (fromCharacter != null && fromCharacter != Character) return false;
        
        
        return hit.transform == null;
    }

}