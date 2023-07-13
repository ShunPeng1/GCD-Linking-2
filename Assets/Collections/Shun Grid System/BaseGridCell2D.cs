using System.Collections.Generic;
using UnityEngine;

namespace Shun_Grid_System
{
    public abstract class BaseGridCell2D<TItem>
    {
        [Header("Base")]
        public List<BaseGridCell2D<TItem>> AdjacentCells = new();
        private Dictionary<BaseGridCell2D<TItem> ,double> _adjacentCellCosts = new();
        public TItem Item;
        public bool IsObstacle;

        [Header("A Star Pathfinding")] 
        public BaseGridCell2D<TItem> ParentXZCell2D = null; 
        public double FCost;
        public double HCost;
        public double GCost;


        protected BaseGridCell2D(TItem item = default)
        {
            Item = item;
        }

        public void SetAdjacency(BaseGridCell2D<TItem>[] adjacentRawCells, double [] adjacentCellCost = null)
        {
            foreach (var adjacentCell in adjacentRawCells)
            {
                SetAdjacency(adjacentCell);
            }
        }
    
        public void SetAdjacency(BaseGridCell2D<TItem> adjacentCell, double adjacentCellCost = 0)
        {
            if (!AdjacentCells.Contains(adjacentCell))
            {
                AdjacentCells.Add(adjacentCell);
                _adjacentCellCosts[adjacentCell] = adjacentCellCost;
            }
        }
        
        public void RemoveAdjacency(BaseGridCell2D<TItem>[] adjacentRawCells)
        {
            foreach (var adjacentCell in adjacentRawCells)
            {
                RemoveAdjacency(adjacentCell);
            }
        }
    
        public void RemoveAdjacency(BaseGridCell2D<TItem> adjacentCell)
        {
            if (!AdjacentCells.Contains(adjacentCell)) return;
            
            AdjacentCells.Remove(adjacentCell);
            _adjacentCellCosts.Remove(adjacentCell);
        }

        public double GetAdjacentCellCost(BaseGridCell2D<TItem> adjacentCell)
        {
            return AdjacentCells.Contains(adjacentCell)? _adjacentCellCosts[adjacentCell] : 0;
        }
    }
}
