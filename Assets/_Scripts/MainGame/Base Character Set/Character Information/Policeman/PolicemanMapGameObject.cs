
using Shun_Grid_System;

public class PolicemanMapGameObject : BaseCharacterMapDynamicGameObject
{
    protected override void InitializePathfinding()
    {
        AdjacencyCellSelection = new PolicemanTilemapAdjacencyCellSelection(this, WallLayerMask);
        PathfindingAlgorithm = new AStarPathFinding<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem>(Grid, AdjacencyCellSelection, PathFindingCostFunction.Manhattan);
        
        LastMovingCell = NextMovingCell = Grid.GetCell(transform.position);

        LastMovingCell?.Item.AddInCellGameObject(this);
    }
}