using UnityEngine;

namespace Shun_Grid_System
{
    public interface IPathFindingAdjacentCellSelection<TCell, TItem> where TCell : BaseGridCell2D<TItem>
    {
        bool CheckMovableCell(TCell from, TCell to);
    }

}