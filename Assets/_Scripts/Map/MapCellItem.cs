using Shun_Grid_System;

public class MapCellItem
{
    protected GridXY<MapCellItem> Grid;
    protected GridXYCell<MapCellItem> Cell;

    public MapCellGameObject MapCellGameObject;
    public CellHighlighter CellHighlighter;
    public MapCellItem(GridXY<MapCellItem> grid, GridXYCell<MapCellItem> cell)
    {
        Grid = grid;
        Cell = cell;
    }
}
    