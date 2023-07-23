using System;
using _Scripts.Managers;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class TilemapAdjacencyCellSelection : IPathFindingAdjacentCellSelection<GridXYCell<MapCellItem>, MapCellItem>
{
    protected GridXY<MapCellItem> Grid => MapManager.Instance.WorldGrid;
    protected PlayerRole PlayerRole => GameManager.Instance.CurrentRolePlaying;
    protected LayerMask WallLayerMask;

    public TilemapAdjacencyCellSelection(LayerMask wallLayerMask)
    {
        WallLayerMask = wallLayerMask;
    }

    

    public virtual bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        return true;
    }
}
