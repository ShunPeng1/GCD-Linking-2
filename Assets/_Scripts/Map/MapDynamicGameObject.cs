using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class MapDynamicGameObject : MonoBehaviour
{
    protected GridXY<MapCellItem> Grid => MapManager.Instance.WorldGrid;
    

    public GridXYCell<MapCellItem> GetCell()
    {
        return Grid.GetCell(transform.position);
    }
}
