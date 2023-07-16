using System;
using System.Collections.Generic;


namespace Shun_Grid_System
{
    public class AStarPathFinding<TGrid,TCell,TItem> : Pathfinding<TGrid, TCell, TItem> 
        where TGrid : BaseGrid2D<TCell,TItem> 
        where TCell : BaseGridCell2D<TItem>
    {
        private TCell _startNode, _endNode;
        private Dictionary<TCell, double> _hValues = new (); // rhsValues[x] = the current best estimate of the cost from x to the goal
        private Dictionary<TCell, double> _gValues = new (); // gValues[x] = the cost of the cheapest path from the start to x
        private IPathFindingDistanceCost _distanceCostFunction;
        private IPathFindingAdjacentCellSelection<TCell, TItem> _adjacentCellSelectionFunction;

        public AStarPathFinding(TGrid gridXZ, IPathFindingAdjacentCellSelection<TCell, TItem> adjacentCellSelectionFunction, PathFindingCostFunction costFunctionType) : base(gridXZ)
        {
            _adjacentCellSelectionFunction = adjacentCellSelectionFunction;
            _distanceCostFunction = costFunctionType switch
            {
                PathFindingCostFunction.Manhattan => new ManhattanDistanceCost(),
                PathFindingCostFunction.Euclidean => new EuclideanDistanceCost(),
                PathFindingCostFunction.Octile => new OctileDistanceCost(),
                PathFindingCostFunction.Chebyshev => new ChebyshevDistanceCost(),
                _ => throw new ArgumentOutOfRangeException(nameof(costFunctionType), costFunctionType, null)
            };
        }
    
        public AStarPathFinding(TGrid gridXZ, IPathFindingDistanceCost pathFindingDistanceCost) : base(gridXZ)
        {
            _distanceCostFunction = pathFindingDistanceCost;
        }
        
        public override LinkedList<TCell> FirstTimeFindPath(TCell startNode, TCell endNode)
        {
            _startNode = startNode;
            _endNode = endNode;
            return FindPath(startNode, endNode);
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns> the path between start and end</returns>
        public LinkedList<TCell> FindPath(TCell startXZCell, TCell endXZCell)
        {
            Priority_Queue.SimplePriorityQueue<TCell, double> openSet = new (); // to be travelled set
            HashSet<TCell> closeSet = new(); // travelled set 
            openSet.Enqueue(startXZCell, startXZCell.FCost);
        
            while (openSet.Count > 0)
            {
                var currentMinFCostCell = openSet.Dequeue();
                closeSet.Add(currentMinFCostCell);

                if (currentMinFCostCell == endXZCell)
                {
                    return RetracePath(startXZCell, endXZCell);;
                }

                foreach (TCell adjacentCell in currentMinFCostCell.AdjacentCells)
                {
                    if (closeSet.Contains(adjacentCell)) continue;  // skip for travelled cell
                    if (!_adjacentCellSelectionFunction.CheckMovableCell(currentMinFCostCell, adjacentCell)) continue;
                    

                    double newGCostToNeighbour = currentMinFCostCell.GCost + GetDistanceCost(currentMinFCostCell, adjacentCell);
                    if (newGCostToNeighbour < adjacentCell.GCost || !openSet.Contains(adjacentCell))
                    {
                        double hCost = GetDistanceCost(adjacentCell, endXZCell);
                        
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
            TCell currentNode = end;
            while (currentNode != start && currentNode!= null) 
            {
                //Debug.Log("Path "+ currentNode.xIndex +" "+ currentNode.zIndex );
                path.AddFirst(currentNode);
                currentNode = (TCell)currentNode.ParentXZCell2D;
            }
            path.AddFirst(start);
            return path;
        }

        protected virtual double GetDistanceCost(TCell start, TCell end)
        {
            var indexDifferenceAbsolute = Grid.GetIndexDifferenceAbsolute(start,end);

            return _distanceCostFunction.GetDistanceCost(indexDifferenceAbsolute.x, indexDifferenceAbsolute.y) + start.GetAdjacentCellCost(end);
        }
    
    }
}

