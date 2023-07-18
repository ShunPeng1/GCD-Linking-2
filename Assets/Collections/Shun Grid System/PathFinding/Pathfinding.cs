using System.Collections.Generic;

namespace Shun_Grid_System
{
    public enum PathFindingAlgorithmType
    {
        DStar,
        AStar
    }

    public interface IPathfindingAlgorithm<TGrid,TCell, TItem> 
        where TGrid : BaseGrid2D<TCell, TItem> 
        where TCell : BaseGridCell2D<TItem>
    {
        public LinkedList<TCell> FirstTimeFindPath(TCell startCell, TCell endCell);
        public LinkedList<TCell> UpdatePathWithDynamicObstacle(TCell currentStartNode, List<TCell> foundDynamicObstacles);
        public LinkedList<TCell> FindAllCellsSmallerThanCost(TCell currentStartNode, double cost);
    }

    public abstract class Pathfinding<TGrid, TCell, TItem> : IPathfindingAlgorithm<TGrid,TCell,TItem> 
        where TGrid : BaseGrid2D<TCell, TItem> 
        where TCell : BaseGridCell2D<TItem>
    {
        protected TGrid Grid;

        public Pathfinding(TGrid grid)
        {
            this.Grid = grid;
        }

        public abstract LinkedList<TCell> FirstTimeFindPath(TCell startCell, TCell endCell);

        public abstract LinkedList<TCell> UpdatePathWithDynamicObstacle(TCell currentStartNode, List<TCell> foundDynamicObstacles);
        public abstract LinkedList<TCell> FindAllCellsSmallerThanCost(TCell currentStartNode, double cost);
    }
}