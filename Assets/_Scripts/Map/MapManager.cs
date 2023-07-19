using System;
using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    public GridXY<MapCellItem> WorldGrid { get; private set;}
    public int GridWidth, GridHeight;
    public float WidthSize, HeightSize;
    
    private void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        WorldGrid = new GridXY<MapCellItem>(GridWidth, GridHeight, WidthSize, HeightSize , transform.position);

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                
                var cell = new GridXYCell<MapCellItem>(WorldGrid, x, y, null);
                var item = new MapCellItem(WorldGrid,cell);
                cell.Item = item;
                
                WorldGrid.SetCell(cell,x,y);   
            }
        }
    }
    
    private void Start()
    {
        
    }
}
