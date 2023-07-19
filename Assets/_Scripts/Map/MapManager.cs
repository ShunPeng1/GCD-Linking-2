using System;
using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    public GridXY<BaseGridXYItemGameObject> WorldGrid { get; private set;}
    public int GridWidth, GridHeight;
    public float WidthSize, HeightSize;
    
    private void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        WorldGrid = new GridXY<BaseGridXYItemGameObject>(GridWidth, GridHeight, WidthSize, HeightSize , transform.position);

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var testItem = Instantiate(ResourceManager.Instance.TestItem, WorldGrid.GetWorldPositionOfNearestCell(x, y),
                    Quaternion.identity, transform);
                var cell = new GridXYCell<BaseGridXYItemGameObject>(WorldGrid, x, y, testItem);
                
                WorldGrid.SetCell(cell,x,y);   
            }
        }
    }
    
    private void Start()
    {
        
    }
}
