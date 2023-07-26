using System;
using _Scripts.Managers;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class TilemapAdjacencyCellSelection : IPathFindingAdjacentCellSelection<GridXYCell<MapCellItem>, MapCellItem>
{
    protected GridXY<MapCellItem> Grid => MapManager.Instance.WorldGrid;
    protected BaseCharacterMapDynamicGameObject Character;
    protected LayerMask WallLayerMask;

    public TilemapAdjacencyCellSelection( BaseCharacterMapDynamicGameObject character, LayerMask wallLayerMask)
    {
        WallLayerMask = wallLayerMask;
        Character = character;
    }

    

    public virtual bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        return true;
    }
}
