using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Cards.Card_UI;
using _Scripts.Input_and_Camera;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class BaseCharacterMapMovableGameObject : MapMovableGameObject
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
        AdjacencyCellSelection = new NonCollisionTilemapAdjacencyCellSelection(CharacterInformation.WallLayerMask);
        PathfindingAlgorithm = new AStarPathFinding<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem>(Grid, AdjacencyCellSelection, PathFindingCostFunction.Manhattan);

    }
    
    private void Update()
    {
        
    }

    private void Move(CellSelectHighlighter cellSelectHighlighter)
    {
        transform.position = cellSelectHighlighter.transform.position;
        //Debug.Log(GetCell().XIndex + " " + GetCell().YIndex);
    }


    public virtual void MoveAbility()
    {
        ShowMovablePath();
        CellHighlightSelectMouseInput mouseInput = new CellHighlightSelectMouseInput(Grid, FinishSelectCell);
        InputManager.Instance.ChangeMouseInput(mouseInput);
    }

    private void FinishSelectCell(CellSelectHighlighter cellSelectHighlighter)
    {
        if (cellSelectHighlighter != null) Move(cellSelectHighlighter);
        HideMovablePath();
    }
    
    public virtual  void SecondAbility()
    {
        
    }
    
    protected void ShowMovablePath()
    {
        AllMovableCellAndCost = PathfindingAlgorithm.FindAllCellsSmallerThanCost(GetCell(), CharacterInformation.MoveCellCost);

        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            var cellHighlighter = cell.Item.CellSelectHighlighter;
            cellHighlighter.EnableInteractable();
            cellHighlighter.StartHighlight();
        }
    }

    protected void HideMovablePath()
    {
        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            var cellHighlighter = cell.Item.CellSelectHighlighter;
            cellHighlighter.DisableInteractable();;
            cellHighlighter.EndHighlight();
        }
    }


}
