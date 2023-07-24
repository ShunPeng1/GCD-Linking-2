using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Cards.Card_UI;
using _Scripts.Input_and_Camera;
using Shun_Card_System;
using Shun_Grid_System;
using Shun_State_Machine;
using Shun_Utility;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class BaseCharacterMapDynamicGameObject : MapDynamicGameObject
{

    [Header("Components")] [SerializeField]
    protected CharacterMovementVisual CharacterMovementVisual;
    public CharacterInformation CharacterInformation { get; protected set; }
    protected BaseCharacterCardGameObject CharacterCardGameObject;

    public float MoveSpeed => CharacterInformation.MoveSpeed;

    
    [Header("Grid")]
    public GridXYCell<MapCellItem> NextMovingCell;
    public GridXYCell<MapCellItem> LastMovingCell;
    public bool IsBetween2Cells = true;
    protected LinkedList<GridXYCell<MapCellItem>> MovingPath;
    protected CharacterMovementTask CurrentMovementTask;
    
    
    [Header("Pathfinding")] 
    protected TilemapAdjacencyCellSelection AdjacencyCellSelection;
    protected Dictionary<GridXYCell<MapCellItem>, double> AllMovableCellAndCost;
    protected IPathfindingAlgorithm<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem> PathfindingAlgorithm;

    
    [Header("Movement State")]
    protected BaseStateMachine<CharacterMovementState> StateMachine = new();
    public CharacterMovementState CurrentMovementState => StateMachine.CurrentBaseState.MyStateEnum;
    
    public enum CharacterMovementState
    {
        Idling,
        Moving
    }

    
    [Header("Ability")] 
    private bool _canForceEnd = true;
    
    #region INITIALIZE
    
    protected void Start()
    {
        InitializeState();
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
        
        LastMovingCell = NextMovingCell = Grid.GetCell(transform.position);

    }
    
    private void InitializeState()
    {
        BaseState<CharacterMovementState> idlingState = new(CharacterMovementState.Idling, null, null, null);
            
        BaseState<CharacterMovementState> movingState = new(CharacterMovementState.Moving, MoveToDestination, null, AssignTask);
        
        
        StateMachine.AddState(idlingState);
        StateMachine.AddState(movingState);
        
        StateMachine.SetToState(CharacterMovementState.Idling);
    }
    

    private void FixedUpdate()
    {
        StateMachine.ExecuteState();
    }

    #endregion

    #region MOVEMENT

    private void AssignTask(CharacterMovementState characterMovementState, object[] enterParameters)
    {
        CurrentMovementTask = enterParameters[0] as CharacterMovementTask;
        if (CurrentMovementTask == null) return;
        
        CreateInitialPath(transform.position, CurrentMovementTask.GoalCellPosition);

        CheckArriveCell();
    }
    
    private void MoveToDestination(CharacterMovementState characterMovementState, object[] objects)
    {
        MoveAlongMovingPath();
        CheckArriveCell();
    }

    protected virtual void MoveAlongMovingPath()
    {
        CharacterMovementVisual.Move(LastMovingCell, NextMovingCell);
        IsBetween2Cells = true;
    }

    private bool CheckArriveCell()
    {
        Vector3 nextCellPosition = Grid.GetWorldPositionOfNearestCell(NextMovingCell);

        if (Vector3.Distance(transform.position, nextCellPosition) != 0) return true;
            
        IsBetween2Cells = false;
        if (CurrentMovementTask != null && transform.position == CurrentMovementTask.GoalCellPosition)
        {
            CurrentMovementTask.GoalArrivalAction?.Invoke();
            ExtractNextCellInPath();
            return false;
        }
                
        ExtractNextCellInPath();
        return true;
    }

    private void ExtractNextCellInPath()
    {
        if (MovingPath == null || MovingPath.Count == 0)
        {
            LastMovingCell = NextMovingCell;
            return;
        }
        
        var nextNextMovingCell = MovingPath.First.Value;
        MovingPath.RemoveFirst(); // the next standing node

        Vector3 nextNextCellPosition = Grid.GetWorldPositionOfNearestCell(nextNextMovingCell.XIndex, nextNextMovingCell.YIndex) ;

        LastMovingCell = NextMovingCell;
        NextMovingCell = nextNextMovingCell;
        //Debug.Log(gameObject.name + " Get Next Cell " + NextCellPosition);
    }
    
    protected bool CreateInitialPath(Vector3 startPosition, Vector3 endPosition)
    {
        var startCell = Grid.GetCell(startPosition);
        var endCell = Grid.GetCell(endPosition);
        
        MovingPath = PathfindingAlgorithm.FirstTimeFindPath(startCell, endCell, CharacterInformation.MaxMoveCellCost);

        if (MovingPath != null) return true; 
        
        // No destination was found
        

        return false;
    }
    
    #endregion

    #region MOVE_ABILITY

    public virtual void MoveAbility(Action externalSuccessSelectionAction = null, Action externalFailSelectionAction = null)
    {
        ShowMovablePath();
        CellHighlightSelectMouseInput mouseInput = new CellHighlightSelectMouseInput(Grid, 
            (CellSelectHighlighter cellSelectHighlighter) =>
        {
            if (CheckSelectedCellValid(cellSelectHighlighter))
            {
                HideMovablePath();
            
                _canForceEnd = false;
            
                var newTask = new CharacterMovementTask(
                    CharacterMovementTask.StartPosition.NextCell,
                    cellSelectHighlighter.transform.position, 
                    () =>
                    {
                        StateMachine.SetToState(CharacterMovementState.Idling);
                        externalSuccessSelectionAction?.Invoke();   
                        _canForceEnd = true;
                    });
            
                StateMachine.SetToState(CharacterMovementState.Moving, null, new object[]{newTask});

                
            }
            else
            {
                HideMovablePath();
                externalFailSelectionAction?.Invoke();
            }
        });
        InputManager.Instance.ChangeMouseInput(mouseInput);
    }

    private bool CheckSelectedCellValid(CellSelectHighlighter cellSelectHighlighter)
    {
        return cellSelectHighlighter != null;
    }
    
    protected void ShowMovablePath()
    {
        AllMovableCellAndCost = PathfindingAlgorithm.FindAllCellsSmallerThanCost(GetCell(), CharacterInformation.MaxMoveCellCost);

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
    
    
    public virtual bool ForceEndMoveAbility(Action externalSuccessForceEndAction = null, Action externalFailForceEndAction = null)
    {
        if (_canForceEnd)
        {
            InputManager.Instance.RemoveCurrentMouseInput();
            HideMovablePath();
            externalSuccessForceEndAction?.Invoke();
            return true;
        }
        else
        {
            externalFailForceEndAction?.Invoke();
            return false;
        }
    }

    
    #endregion



    #region SECOND_ABILITY
    
    public virtual void SecondAbility(Action externalSuccessSelectionAction = null, Action externalFailSelectionAction = null)
    {
        
    }
    
    public virtual bool ForceEndSecondAbility(Action externalSuccessForceEndAction = null, Action externalFailForceEndAction = null)
    {
        if (_canForceEnd)
        {
            InputManager.Instance.RemoveCurrentMouseInput();
            externalSuccessForceEndAction?.Invoke();
            return true;
        }
        else
        {
            externalFailForceEndAction?.Invoke();
            return false;
        }
    }
    
    #endregion
    
}
