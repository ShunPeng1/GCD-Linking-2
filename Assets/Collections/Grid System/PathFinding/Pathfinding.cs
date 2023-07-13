using System.Collections.Generic;

namespace _Scripts.Grid_System.PathFinding
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
        public LinkedList<TCell> FirstTimeFindPath(TCell start, TCell end);
        public LinkedList<TCell> UpdatePathWithDynamicObstacle(TCell currentStartNode, List<TCell> foundDynamicObstacles);
    }

    public class Pathfinding<TGrid, TCell, TItem> : IPathfindingAlgorithm<TGrid,TCell,TItem> 
        where TGrid : BaseGrid2D<TCell, TItem> 
        where TCell : BaseGridCell2D<TItem>
    {
        protected TGrid Grid;

        public Pathfinding(TGrid grid)
        {
            this.Grid = grid;
        }
    
        public virtual LinkedList<TCell> FirstTimeFindPath(TCell start, TCell end)
        {
            return null;
        }

        public virtual LinkedList<TCell> UpdatePathWithDynamicObstacle(TCell currentStartNode, List<TCell> foundDynamicObstacles)
        {
            return null;
        }
    }
}