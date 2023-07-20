using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class BaseCharacterMapGameObject : MapGameObject
{
    protected CharacterInformation CharacterInformation;
    protected BaseCharacterCardGameObject CharacterCardGameObject;
    
    protected Rigidbody2D Rb;

    [Header("Pathfinding")] 
    protected TilemapAdjacencyCellSelection AdjacencyCellSelection;
    protected Dictionary<GridXYCell<MapCellItem>, double> AllMovableCellAndCost;
    protected IPathfindingAlgorithm<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem> PathfindingAlgorithm;

    
    
    
    protected void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        
    }

    public void InitializeCharacter(CharacterInformation characterInformation, BaseCharacterCardGameObject characterCardGameObject )
    {
        CharacterInformation = characterInformation;
        CharacterCardGameObject = characterCardGameObject;

        InitializePathfinding();
    }

    protected virtual void InitializePathfinding()
    {
        AdjacencyCellSelection = new NonCollisionTilemapAdjacencyCellSelection(Grid, CharacterInformation.WallLayerMask, Grid.GetCellWorldSize().x);
        PathfindingAlgorithm = new AStarPathFinding<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem>(Grid, AdjacencyCellSelection, PathFindingCostFunction.Manhattan);

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


    public virtual void MoveAbility()
    {
        ShowMovablePath();
    }

    public virtual  void SecondAbility()
    {
        
    }
    
    protected void ShowMovablePath()
    {
        AllMovableCellAndCost = PathfindingAlgorithm.FindAllCellsSmallerThanCost(GetCell(), CharacterInformation.MoveCellCost);

        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            var cellHighlighter = cell.Item.CellHighlighter;
            cellHighlighter.Interactable = true;
            cellHighlighter.StartHighlight();
        }
    }

    protected void HideMovablePath()
    {
        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            var cellHighlighter = cell.Item.CellHighlighter;
            cellHighlighter.Interactable = false;
            cellHighlighter.EndHighlight();
        }
    }


}
