using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class BaseGridXYItemGameObject : MonoBehaviour
{
    protected GridXY<BaseGridXYItemGameObject> Grid;
    protected GridXYCell<BaseGridXYItemGameObject> Cell;
    protected int XIndex, YIndex;
    
    
    void Start()
    {
        Grid = MapManager.Instance.WorldGrid;
        Cell = Grid.GetCell(transform.position);
        Cell.Item = this;
        XIndex = Cell.XIndex;
        YIndex = Cell.YIndex;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
