

namespace Shun_Grid_System
{
    /// <summary>
    /// This is a template of a grid item, it can be inherit with different data save
    /// </summary>
    public class BaseGridXYItem
    {
        protected GridXY<BaseGridXYItem> Grid;
        protected GridXYCell<BaseGridXYItem> Cell;

        protected BaseGridXYItem(GridXY<BaseGridXYItem> grid, GridXYCell<BaseGridXYItem> cell)
        {
            Grid = grid;
            Cell = cell;
        }

    }
}