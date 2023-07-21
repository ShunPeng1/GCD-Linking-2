using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class VentMapGameObject : MapCellGameObject
{
    private GridXY<MapCellItem> _grid;
    public GridXYCell<MapCellItem> Cell;

    [SerializeField] private bool _isOpen = true;
    public bool IsOpen { 
        get => _isOpen;
        set => _isOpen = value;
    }
    
    [SerializeField] private Animator _animator;
    
    // Start is called before the first frame update
    void Start()
    {
        _grid = MapManager.Instance.WorldGrid;
        Cell = _grid.GetCell(transform.position);
        Cell.Item.AddInCellGameObject(this);
    }

    public void CloseVent()
    {
        _isOpen = false;
        
    }
    
    public void OpenVent()
    {
        _isOpen = true;
        
    }

}
