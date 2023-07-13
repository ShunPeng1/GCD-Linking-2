using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Grid_System
{
    public class GridXYCell<TItem> : BaseGridCell2D<TItem>
    {
        [Header("Base")] private GridXY<TItem> _gridXY;
        public readonly int XIndex, YIndex;
        
        public GridXYCell(GridXY<TItem> grid, int x, int y, TItem item = default) : base(item)
        {
            _gridXY = grid;
            XIndex = x;
            YIndex = y;
        }
        
    }
}