using System;
using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class MapCellGameObject : MonoBehaviour
{
    
    protected GridXY<MapCellItem> Grid;
    public GridXYCell<MapCellItem> Cell { get; private set; }
    protected int XIndex, YIndex;
    private bool _isInitializedGrid = false;

    private void Start()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        if (_isInitializedGrid) return;
        
        Grid = MapManager.Instance.WorldGrid;
        Cell = Grid.GetCell(transform.position);
        Cell.Item.AddInCellGameObject(this);
        _isInitializedGrid = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
