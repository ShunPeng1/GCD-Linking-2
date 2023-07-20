using System;
using Shun_Grid_System;
using UnityEngine;


[Serializable]
public class NonCollisionTilemapCellSelection : IPathFindingAdjacentCellSelection<GridXYCell<MapCellItem>, MapCellItem>
{
    private GridXY<MapCellItem> _grid;
    private LayerMask _wallLayerMask;
    private float _distance;

    public NonCollisionTilemapCellSelection(GridXY<MapCellItem> grid, LayerMask wallLayerMask, float distance)
    {
        _grid = grid;
        _wallLayerMask = wallLayerMask;
        _distance = distance;
    }
    
    public bool CheckMovableCell(GridXYCell<MapCellItem> from, GridXYCell<MapCellItem> to)
    {
        var fromPosition = _grid.GetWorldPositionOfNearestCell(from.XIndex, from.YIndex);
        var toPosition = _grid.GetWorldPositionOfNearestCell(to.XIndex, to.YIndex);
        var hit = Physics2D.Raycast(fromPosition , toPosition - fromPosition, _distance, _wallLayerMask);

        return hit.transform == null;
    }
}