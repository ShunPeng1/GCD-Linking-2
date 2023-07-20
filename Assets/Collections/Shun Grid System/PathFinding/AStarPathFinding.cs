using System;
using System.Collections.Generic;


namespace Shun_Grid_System
{
    public class AStarPathFinding<TGrid,TCell,TItem> : Pathfinding<TGrid, TCell, TItem> 
        where TGrid : BaseGrid2D<TCell,TItem> 
        where TCell : BaseGridCell2D<TItem>
    {
        private TCell _startCell, _endCell;
        private Dictionary<TCell, double> _hValues = new (); // rhsValues[x] = the current best estimate of the cost from x to the goal
        private Dictionary<TCell, double> _gValues = new (); // gValues[x] = the cost of the cheapest path from the start to x
        private IPathFindingDistanceCost _distanceCostFunction;
        private IPathFindingAdjacentCellSelection<TCell, TItem> _adjacentCellSelectionFunction;

        public AStarPathFinding(TGrid gridXZ, IPathFindingAdjacentCellSelection<TCell, TItem> adjacentCellSelectionFunction = null, PathFindingCostFunction costFunctionType = PathFindingCostFunction.Manhattan) : base(gridXZ)
        {
            _adjacentCellSelectionFunction = adjacentCellSelectionFunction ?? new PathFindingAllAdjacentCellAccept<TCell, TItem>();
            _distanceCostFunction = costFunctionType switch
            {
                PathFindingCostFunction.Manhattan => new ManhattanDistanceCost(),
                PathFindingCostFunction.Euclidean => new EuclideanDistanceCost(),
                PathFindingCostFunction.Octile => new OctileDistanceCost(),
                PathFindingCostFunction.Chebyshev => new ChebyshevDistanceCost(),
                _ => throw new ArgumentOutOfRangeException(nameof(costFunctionType), costFunctionType, null)
            };
        }

        public override LinkedList<TCell> FirstTimeFindPath(TCell startCell, TCell endCell)
        {
            _startCell = startCell;
            _endCell = endCell;
            return FindPath();
        }

        public override LinkedList<TCell> UpdatePathWithDynamicObstacle(TCell currentStartCell, List<TCell> foundDynamicObstacles)
        {
            return FindPath();
        }

        public override Dictionary<TCell, double> FindAllCellsSmallerThanCost(TCell currentStartCell, double cost)
        {
            Priority_Queue.SimplePriorityQueue<TCell, double> openSet = new Priority_Queue.SimplePriorityQueue<TCell, double>();
            HashSet<TCell> visitedSet = new HashSet<TCell>();

            openSet.Enqueue(currentStartCell, currentStartCell.FCost);

            Dictionary<TCell, double> reachableCells = new();

            reachableCells[currentStartCell] = 0;
            
            while (openSet.Count > 0)
            {
                TCell currentCell = openSet.Dequeue();
                visitedSet.Add(currentCell);

                if (currentCell.FCost > cost)
                    continue;

                foreach (TCell adjacentCell in currentCell.AdjacentCells)
                {
                    if (visitedSet.Contains(adjacentCell))
                        continue;

                    if (!_adjacentCellSelectionFunction.CheckMovableCell(currentCell, adjacentCell))
                        continue;

                    double newGCost = currentCell.GCost + GetDistanceCost(currentCell, adjacentCell);

                    if (newGCost < adjacentCell.GCost || !openSet.Contains(adjacentCell))
                    {
                        adjacentCell.GCost = newGCost;
                        adjacentCell.HCost = 0;
                        adjacentCell.FCost = newGCost + adjacentCell.HCost;
                        adjacentCell.ParentXZCell2D = currentCell;
                        
                        reachableCells[adjacentCell] = newGCost;
                        if (!openSet.Contains(adjacentCell))
                        {
                            openSet.Enqueue(adjacentCell, adjacentCell.FCost);
                        }
                    }
                }
            }

            return reachableCells;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns> the path between start and end</returns>
        private LinkedList<TCell> FindPath()
        {
            Priority_Queue.SimplePriorityQueue<TCell, double> openSet = new (); // to be travelled set
            HashSet<TCell> visitedSet = new(); // travelled set 
            
            openSet.Enqueue(_startCell, _startCell.FCost);
        
            while (openSet.Count > 0)
            {
                TCell currentMinFCostCell = openSet.Dequeue();
                visitedSet.Add(currentMinFCostCell);

                if (currentMinFCostCell == _endCell)
                {
                    return RetracePath(_startCell, _endCell);;
                }

                foreach (TCell adjacentCell in currentMinFCostCell.AdjacentCells)
                {
                    if (visitedSet.Contains(adjacentCell)) 
                        continue;  // skip for travelled cell
                    
                    if (!_adjacentCellSelectionFunction.CheckMovableCell(currentMinFCostCell, adjacentCell)) 
                        continue;
                    
                    double newGCostToNeighbour = currentMinFCostCell.GCost + GetDistanceCost(currentMinFCostCell, adjacentCell);
                    
                    if (newGCostToNeighbour < adjacentCell.GCost || !openSet.Contains(adjacentCell))
                    {
                        double hCost = GetDistanceCost(adjacentCell, _endCell);
                        
                        adjacentCell.GCost = newGCostToNeighbour;
                        adjacentCell.HCost = hCost;
                        adjacentCell.FCost = newGCostToNeighbour + hCost;
                        adjacentCell.ParentXZCell2D = currentMinFCostCell;

                        if (!openSet.Contains(adjacentCell)) // Not in open set
                        {
                            openSet.Enqueue(adjacentCell, adjacentCell.FCost);
                        }
                    }

                }
            }
            //Not found a path to the end
            return null;
        }

        /// <summary>
        /// Get a list of Cell that the pathfinding was found
        /// </summary>
        protected LinkedList<TCell> RetracePath(TCell start, TCell end)
        {
            LinkedList<TCell> path = new();
            TCell currentCell = end;
            while (currentCell != start && currentCell!= null) 
            {
                //Debug.Log("Path "+ currentCell.xIndex +" "+ currentCell.zIndex );
                path.AddFirst(currentCell);
                currentCell = (TCell)currentCell.ParentXZCell2D;
            }
            path.AddFirst(start);
            return path;
        }

        protected virtual double GetDistanceCost(TCell start, TCell end)
        {
            var indexDifferenceAbsolute = Grid.GetIndexDifferenceAbsolute(start,end);

            return _distanceCostFunction.GetDistanceCost(indexDifferenceAbsolute.x, indexDifferenceAbsolute.y) + start.GetAdditionalAdjacentCellCost(end);
        }
    
    }
}

