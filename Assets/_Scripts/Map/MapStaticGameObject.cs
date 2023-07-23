using System;
using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class MapStaticGameObject : MonoBehaviour
{
    protected GridXY<MapCellItem> Grid => MapManager.Instance.WorldGrid;
    public GridXYCell<MapCellItem> Cell => Grid.GetCell(transform.position);
    protected int XIndex, YIndex;
    

    private void Start()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        Cell.Item.AddInCellGameObject(this);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
