using System;
using System.Collections;
using System.Collections.Generic;
using _Script.PathFinding;
using _Scripts.Grid_System.PathFinding;
using Priority_Queue;
using UnityEngine;


/* <summary>
D* Lite is a pathfinding algorithm that is similar to A* but is designed to handle changing environments, such as a partially mapped or dynamic grid. The algorithm uses two values for each cell on the grid: the g-value and the rhs-value.

The g-value represents the cost of the optimal path from the start node to the current node. Initially, all g-values are set to infinity except for the start node, which is set to zero. As the algorithm explores the grid, it updates the g-values of cells that are closer to the start node with more accurate costs.

The rhs-value represents the cost of the second-best path from the start node to the current node. Initially, all rhs-values are set to infinity except for the start node, which is set to zero. As the algorithm explores the grid, it updates the rhs-values of cells that are farther from the start node with more accurate costs.

The algorithm starts by creating a priority queue of cells to be explored. The priority queue is sorted by the sum of the g-value and the heuristic value (H) of each cell. At each iteration, the algorithm checks the cell with the lowest priority in the priority queue. If the cell's g-value is greater than its rhs-value, the cell's g-value is updated to its rhs-value and the cell's predecessors are updated. If the cell's g-value is less than its rhs-value, the cell's rhs-value is updated to its g-value and the cell is added to the priority queue.

As the algorithm progresses, it updates the g-values and rhs-values of cells and adds new cells to the priority queue. When the goal cell is found, the algorithm returns the path from the start node to the goal node.

One of the key features of D* Lite is that it is designed to work efficiently in dynamic environments. When the environment changes, the algorithm can be re-run with the updated costs and a new path will be found that takes the changes into account. This makes it a useful algorithm for applications such as robotics, where the environment can change frequently.
*/
public class DStarLitePathFinding<TGrid,TCell,TItem> : Pathfinding<TGrid, TCell, TItem> 
    where TGrid : BaseGrid2D<TCell,TItem> 
    where TCell : BaseGridCell2D<TItem>
{
    private TCell _startNode, _endNode;
    private IPathFindingDistanceCost _distanceCostFunction;
    private Priority_Queue.SimplePriorityQueue<TCell, QueueKey > _openNodes = new ( new CompareFCostHCost()); // priority queue of open nodes
    private int _km = 0; // km = heuristic for estimating cost of travel along the last path
    private Dictionary<TCell, double> _rhsValues = new (); // rhsValues[x] = the current best estimate of the cost from x to the goal
    private Dictionary<TCell, double> _gValues = new (); // gValues[x] = the cost of the cheapest path from the start to x
    private Dictionary<TCell, TCell> _predecessors = new (); // predecessors[x] = the node that comes before x on the best path from the start to x
    private Dictionary<TCell, float> _dynamicObstacles = new(); // dynamicObstacle[x] = the cell that is found obstacle after find path and its found time
    
    public DStarLitePathFinding(TGrid gridXZ, PathFindingCostFunction costFunctionType) : base(gridXZ)
    {
        switch (costFunctionType)
        {
            case PathFindingCostFunction.Manhattan:
                _distanceCostFunction = new ManhattanDistanceCost();
                break;
            case PathFindingCostFunction.Euclidean:
                _distanceCostFunction = new EuclideanDistanceCost();
                break;
            case PathFindingCostFunction.Octile:
                _distanceCostFunction = new OctileDistanceCost();
                break;
            case PathFindingCostFunction.Chebyshev:
                _distanceCostFunction = new ChebyshevDistanceCost();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(costFunctionType), costFunctionType, null);
        }
    }
    
    public DStarLitePathFinding(TGrid gridXZ, IPathFindingDistanceCost pathFindingDistanceCost) : base(gridXZ)
    {
        _distanceCostFunction = pathFindingDistanceCost;
    }

    private struct QueueKey
    {
        public double FCost;
        public double HCost;

        public QueueKey(double fCost = 0, double hCost = 0)
        {
            FCost = fCost;
            HCost = hCost;
        }
    }
    private class CompareFCostHCost : IComparer<QueueKey>
    {
        public int Compare(QueueKey x, QueueKey y)
        {
            int compare = x.FCost.CompareTo(y.FCost);
            if (compare == 0)
            {
                compare = x.HCost.CompareTo(y.HCost);
            }
            return compare;
        }
    }

    public override LinkedList<TCell> FirstTimeFindPath(TCell startNode, TCell endNode)
    {
        ResetPathFinding(startNode, endNode);
        
        return FindPath(startNode, endNode);
    }

    public LinkedList<TCell> FindPath(TCell startNode, TCell endNode)
    {
        while (_openNodes.Count > 0 &&
               (GetRhsValue(startNode) > CalculateKey(startNode, startNode) ||
                _gValues[startNode] == double.PositiveInfinity))
        {
            if (!_openNodes.TryDequeue(out TCell currentNode)) return null; // there are no way to get to end
            
            //Debug.Log("DStar current Node" + currentNode.XIndex + " " + currentNode.ZIndex);

            if (GetGValue(currentNode) > _rhsValues[currentNode])
            {
                _gValues[currentNode] = _rhsValues[currentNode];

                foreach (TCell neighbor in currentNode.AdjacentCells)
                {
                    //if (neighbor != null && !neighbor.IsObstacle)
                    if (neighbor != null && !neighbor.IsObstacle && !_dynamicObstacles.ContainsKey(neighbor))
                    {
                        UpdateNode(neighbor, currentNode);
                    }
                }
            }
            else
            {
                _gValues[currentNode] = double.PositiveInfinity;
                UpdateNode(currentNode);

                foreach (TCell neighbor in currentNode.AdjacentCells)
                {
                    //if (neighbor != null && !neighbor.IsObstacle)
                    if (neighbor != null && !neighbor.IsObstacle && !_dynamicObstacles.ContainsKey(neighbor))
                    {
                        UpdateNode(neighbor, currentNode);
                    }
                }

            }
        }

        return RetracePath(startNode, endNode);
    }
    
    public override LinkedList<TCell> UpdatePathWithDynamicObstacle(TCell currentStartNode, List<TCell> foundDynamicObstacles)
    {
        ResetPathFinding(currentStartNode, _endNode);
        
        foreach (var obstacleCell in foundDynamicObstacles)
        {
            if (_dynamicObstacles.ContainsKey(obstacleCell)) continue;
            _dynamicObstacles[obstacleCell] = Time.time;
        }
        
        return FindPath(_startNode, _endNode);
    }
    
    private void UpdateNode(TCell updateNode, TCell itsPredecessorNode = null)
    {
        //Debug.Log("DStar UpdateNode " + node.XIndex + " " + node.ZIndex);
        if (updateNode != _endNode)
        {
            /*
             * Get the min rhs from Successors, then add it to the predecessors for traverse
             */
            double minRhs = double.PositiveInfinity;
            TCell minSucc = null;

            foreach (TCell successor in updateNode.AdjacentCells)
            {
                double rhs = _gValues.ContainsKey(successor)
                    ? GetGValue(successor) + GetDistanceCost(updateNode, successor)
                    : double.PositiveInfinity;
                if (rhs < minRhs && !successor.IsObstacle && !_dynamicObstacles.ContainsKey(successor) && !_dynamicObstacles.ContainsKey(successor))
                    // Is the min successor, if it the same, choose the one not its press 
                //if  ((rhs < minRhs||(rhs == minRhs && rhs !=double.PositiveInfinity  && successor != itsPredecessorNode)) && !successor.IsObstacle && !_dynamicObstacles.ContainsKey(successor))
                {
                    minRhs = rhs;
                    minSucc = successor;
                }
            }

            _rhsValues[updateNode] = minRhs;
            //_predecessors[upgradeNode] = minSucc ?? (_predecessors[upgradeNode] ?? null);
            _predecessors[updateNode] = minSucc;

        }

        if (_openNodes.Contains(updateNode)) // refresh the old node
        {
            _openNodes.TryRemove(updateNode);
        }

        if (GetGValue(updateNode) != GetRhsValue(updateNode)) // Mainly if both not equal double.PositiveInfinity, meaning it found a path that is shorter
        {
            //_gValues[updateNode] = GetRhsValue(updateNode);
            var gValue = (double.IsPositiveInfinity(GetGValue(updateNode)) ? 999999999 : _gValues[updateNode]);
            var hValue = GetDistanceCost(updateNode, _startNode);
            var fValue = (gValue + hValue + _km);
            updateNode.GCost = gValue;
            updateNode.HCost = hValue;
            updateNode.FCost = fValue;

            _openNodes.Enqueue(updateNode, new QueueKey(fValue, hValue)); // Enqueue the new node
        }
    }

    private LinkedList<TCell> RetracePath(TCell startXZCell, TCell endXZCell)
    {
        LinkedList<TCell> path = new LinkedList<TCell>();
        TCell currentCell = startXZCell;
        HashSet<TCell> visitedCells = new();
        
        while (currentCell != endXZCell)
        {
            path.AddLast(currentCell);
            visitedCells.Add(currentCell);
            
            TCell nextCell = null;
            double minGCost = Double.PositiveInfinity;
            foreach (TCell successor in currentCell.AdjacentCells)
            {
                double successorGCost = GetGValue(successor);
                if ( successorGCost <= minGCost && !successor.IsObstacle && !_dynamicObstacles.ContainsKey(successor) && !visitedCells.Contains(successor))
                {
                    nextCell = successor;
                    minGCost = successorGCost;
                }
                
            }

            if (nextCell != null) currentCell = nextCell;
            else return null;
        }

        path.AddLast(endXZCell);

        return path;
    }

    private void ResetPathFinding(TCell startNode, TCell endNode)
    {
        _openNodes = new ( new CompareFCostHCost()); // priority queue of open nodes
        _km = 0; // km = heuristic for estimating cost of travel along the last path
        _rhsValues = new (); // rhsValues[x] = the current best estimate of the cost from x to the goal
        _gValues = new (); // gValues[x] = the cost of the cheapest path from the start to x
        _predecessors = new (); // predecessors[x] = the node that comes before x on the best path from the start to x
        _dynamicObstacles = new(); // dynamicObstacle[x] = the cell that is found obstacle after find path and its found time

        this._startNode = startNode;
        this._endNode = endNode;
        
        _gValues[endNode] = double.PositiveInfinity;
        _rhsValues[endNode] = 0;

        _gValues[startNode] =  double.PositiveInfinity;
        _rhsValues[startNode] = double.PositiveInfinity;
        _predecessors[startNode] = null;

        _openNodes.Enqueue(endNode, new QueueKey((int) CalculateKey(endNode, startNode) , 0));

        
    }

    private double CalculateKey(TCell currNode, TCell startNode)
    {
        return Math.Min(GetRhsValue(currNode), GetGValue(currNode)) + GetDistanceCost(currNode, startNode) + _km;
    }

    private double GetRhsValue(TCell node)
    {
        return _rhsValues.TryGetValue(node, out double value) ? value : double.PositiveInfinity;
    }

    private double GetGValue(TCell node)
    {
        return _gValues.TryGetValue(node, out double value) ? value : double.PositiveInfinity;
    }

    protected virtual double GetDistanceCost(TCell start, TCell end)
    {
        var indexDifferenceAbsolute = Grid.GetIndexDifferenceAbsolute(start,end);

        return _distanceCostFunction.GetDistanceCost(indexDifferenceAbsolute.x, indexDifferenceAbsolute.y);
    }
    
}