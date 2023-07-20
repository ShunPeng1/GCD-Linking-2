using System;
using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class BaseWorldCharacter : MapGameObject
{
    public CharacterInformation CharacterInformation;
    
    
    protected Rigidbody2D Rb;

    [Header("Pathfinding")] 
    protected TilemapAdjacencyCellSelection AdjacencyCellSelection;
    protected Dictionary<GridXYCell<MapCellItem>, double> AllMovableCellAndCost;
    protected IPathfindingAlgorithm<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem> PathfindingAlgorithm;


    protected override void Start()
    {
        base.Start();
        Rb = GetComponent<Rigidbody2D>();
        
        InitializePathfinding();
    }

    private void Update()
    {
        
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        movement.Normalize();

        Rb.AddForce(movement * CharacterInformation.MoveSpeed);
        
        //Debug.Log(GetCell().XIndex + " " + GetCell().YIndex);
    }

    protected virtual void InitializePathfinding()
    {
        AdjacencyCellSelection = new NonCollisionTilemapAdjacencyCellSelection(Grid, CharacterInformation.WallLayerMask, Grid.GetCellWorldSize().x);
        PathfindingAlgorithm = new AStarPathFinding<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem>(Grid, AdjacencyCellSelection, PathFindingCostFunction.Manhattan);

    }
    
    private void ShowMovablePath()
    {
        AllMovableCellAndCost = CharacterInformation.PathfindingAlgorithm.FindAllCellsSmallerThanCost(GetCell(), CharacterInformation.MoveCellCost);

        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            cell.Item.CellHighlighter.StartHighlight();
        }
    }

    private void HideMovablePath()
    {
        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            cell.Item.CellHighlighter.EndHighlight();
        }
    }


}
