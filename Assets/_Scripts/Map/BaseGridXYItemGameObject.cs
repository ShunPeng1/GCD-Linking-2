using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class BaseGridXYItemGameObject : MonoBehaviour
{
    protected GridXY<MapCellItem> Grid;
    protected GridXYCell<MapCellItem> Cell;
    protected int XIndex, YIndex;
    
    
    protected virtual void Start()
    {
        Grid = MapManager.Instance.WorldGrid;
        Cell = Grid.GetCell(transform.position);
        
        XIndex = Cell.XIndex;
        YIndex = Cell.YIndex;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
