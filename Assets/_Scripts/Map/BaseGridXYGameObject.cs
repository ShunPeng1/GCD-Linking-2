using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class BaseGridXYGameObject : MonoBehaviour
{
    protected GridXY<BaseGridXYItemGameObject> Grid;

    void Start()
    {
        Grid = MapManager.Instance.WorldGrid;
        
    }

    public GridXYCell<BaseGridXYItemGameObject> GetCell()
    {
        return Grid.GetCell(transform.position);
    }
}
