using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class PolicemanTilemapAdjacencyCellSelection : TilemapAdjacencyCellSelection
{

    public PolicemanTilemapAdjacencyCellSelection(BaseCharacterMapDynamicGameObject characterMapDynamicGameObject,LayerMask wallLayerMask) : base(characterMapDynamicGameObject, wallLayerMask)
    {
    }    
    
    public override bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {

        ExitMapGameObject fromExit = from.Item.GetFirstInCellGameObject<ExitMapGameObject>();
        if (fromExit != null) return false;

        var fromCharacter = from.Item.GetFirstInCellGameObject<BaseCharacterMapDynamicGameObject>();
        if (fromCharacter != null && fromCharacter != Character) return false;
        
        var fromPosition = Grid.GetWorldPositionOfNearestCell(from.XIndex, from.YIndex);
        var toPosition = Grid.GetWorldPositionOfNearestCell(to.XIndex, to.YIndex);

        Character.Collider2D.enabled = false;
        var hit = Physics2D.Linecast(fromPosition, toPosition, WallLayerMask);
        Character.Collider2D.enabled = true;
        
        return hit.transform == null;
    }

}