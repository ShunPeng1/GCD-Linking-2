using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class BaseGridXYGameObject : MonoBehaviour
{
    [SerializeField] protected GridXY<MapCellItem> Grid;

    protected virtual void Start()
    {
        Grid = MapManager.Instance.WorldGrid;
        
    }

    public GridXYCell<MapCellItem> GetCell()
    {
        return Grid.GetCell(transform.position);
    }
}
