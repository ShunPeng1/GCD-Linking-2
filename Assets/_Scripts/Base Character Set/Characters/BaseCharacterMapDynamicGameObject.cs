using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Cards.Card_UI;
using _Scripts.Input_and_Camera;
using _Scripts.Lights;
using _Scripts.Managers;
using Shun_Card_System;
using Shun_Grid_System;
using Shun_State_Machine;
using Shun_Utility;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class BaseCharacterMapDynamicGameObject : MapDynamicGameObject
{

    [Header("Components")] 
    public CharacterLight CharacterLight;
    public CharacterMovementVisual CharacterMovementVisual;
    public Collider2D Collider2D;
    
    public CharacterSet CharacterSet { get; protected set; }

    public float MoveSpeed => CharacterSet.CharacterInformation.MoveSpeed;
    private float MaxMoveCellCost => CharacterSet.CharacterInformation.MaxMoveCellCost;
    private LayerMask WallLayerMask => CharacterSet.CharacterInformation.WallLayerMask;
    
    
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

        Collider2D = GetComponent<Collider2D>();
    }

    public void InitializeCharacter(CharacterSet characterSet)
    {
        CharacterSet = characterSet;

        InitializePathfinding();
    }
    
    protected virtual void InitializePathfinding()
    {
        AdjacencyCellSelection = new NonCollisionTilemapAdjacencyCellSelection(this, WallLayerMask);
        PathfindingAlgorithm = new AStarPathFinding<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem>(Grid, AdjacencyCellSelection, PathFindingCostFunction.Manhattan);
        
        LastMovingCell = NextMovingCell = Grid.GetCell(transform.position);

        LastMovingCell?.Item.AddInCellGameObject(this);
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
        
        Grid.GetCell(transform.position).Item.RemoveInCellGameObject(this);
        
        CreateInitialPath(transform.position, CurrentMovementTask.GoalCellPosition);


        CheckArriveCell();
        ExtractNextCellInPath();
    }
    
    private void MoveToDestination(CharacterMovementState characterMovementState, object[] objects)
    {
        MoveAlongMovingPath();
        CheckArriveCell();
    }

    protected virtual void MoveAlongMovingPath()
    {
        CharacterMovementVisual.Move(LastMovingCell, NextMovingCell);
        CharacterLight.UpdateLight();
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
        
        MovingPath = PathfindingAlgorithm.FirstTimeFindPath(startCell, endCell, MaxMoveCellCost);

        if (MovingPath != null) return true; 
        
        // No destination was found
        

        return false;
    }
    
    #endregion

    #region MOVE_ABILITY

    private enum CellSelectionResult
    {
        InvalidSelection,
        ValidSelection,
        EndGameSelection
    }
    
    public virtual void MoveAbility(Action externalSuccessSelectionAction = null, Action externalFailSelectionAction = null)
    {
        ShowMovablePath();
        CellSelectMouseInput mouseInput = new CellSelectMouseInput(
            Grid,
            selectedCell =>
            {
                return CheckSelectedCellValid(selectedCell) != CellSelectionResult.InvalidSelection;
            },
            selectedCell =>
            {
                HideMovablePath();
                if (!FinishedSelectionCell(selectedCell, externalSuccessSelectionAction))
                    externalFailSelectionAction?.Invoke();
            });
        InputManager.Instance.ChangeMouseInput(mouseInput);
    }

    protected virtual bool FinishedSelectionCell(GridXYCell<MapCellItem> selectedCell, Action externalSuccessSelectionAction = null)
    {
        
        switch (CheckSelectedCellValid(selectedCell))
        {
            case CellSelectionResult.InvalidSelection:
                return false;
            case CellSelectionResult.ValidSelection:
                _canForceEnd = false;
            
                var validCellTask = new CharacterMovementTask(
                    CharacterMovementTask.StartPosition.NextCell,
                    Grid.GetWorldPositionOfNearestCell(selectedCell), 
                    () =>
                    {
                        StateMachine.SetToState(CharacterMovementState.Idling);
                        
                        MapManager.Instance.UpdateAllCharacterRecognition();
                        externalSuccessSelectionAction?.Invoke();
                        
                        Grid.GetCell(transform.position)?.Item.AddInCellGameObject(this);
                        
                        _canForceEnd = true;
                    });
            
                StateMachine.SetToState(CharacterMovementState.Moving, null, new object[]{validCellTask});

                return true;
            case CellSelectionResult.EndGameSelection:
                var endGameCellTask = new CharacterMovementTask(
                    CharacterMovementTask.StartPosition.NextCell,
                    Grid.GetWorldPositionOfNearestCell(selectedCell), 
                    () =>
                    {
                        StateMachine.SetToState(CharacterMovementState.Idling);
                        
                        MapManager.Instance.UpdateAllCharacterRecognition();
                        GameManager.Instance.RequestEndGame(this);

                        _canForceEnd = true;
                        
                    });
            
                StateMachine.SetToState(CharacterMovementState.Moving, null, new object[]{endGameCellTask});

                
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    private CellSelectionResult CheckSelectedCellValid(GridXYCell<MapCellItem> selectedCell)
    {
        if (selectedCell == Grid.GetCell(transform.position)) return CellSelectionResult.ValidSelection;

        var rolePlaying = GameManager.Instance.CurrentRolePlaying;
        if (selectedCell.Item.GetFirstInCellGameObject<ExitMapGameObject>() != null)
            return rolePlaying == PlayerRole.Imposter && GameManager.Instance.CheckImposterCanExit(this)
                ? CellSelectionResult.EndGameSelection
                : CellSelectionResult.InvalidSelection;
        
        var character = selectedCell.Item.GetFirstInCellGameObject<BaseCharacterMapDynamicGameObject>();
        if (character != null)
            return rolePlaying == PlayerRole.Detective
                   && character.CharacterSet.CharacterRecognition.Value != CharacterRecognitionState.Innocent
                ? CellSelectionResult.EndGameSelection
                : CellSelectionResult.InvalidSelection;
        
        return CellSelectionResult.ValidSelection;
    }
    
    
    protected void ShowMovablePath()
    {
        AllMovableCellAndCost = PathfindingAlgorithm.FindAllCellsSmallerThanCost(GetCell(), MaxMoveCellCost);

        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            if (CheckSelectedCellValid(cell) == CellSelectionResult.InvalidSelection) continue;
            
            var cellHighlighter = cell.Item.CellSelectHighlighter;
            cellHighlighter.EnableInteractable();
        }
    }

    protected void HideMovablePath()
    {
        foreach (var (cell, gCost) in AllMovableCellAndCost)
        {
            var cellHighlighter = cell.Item.CellSelectHighlighter;
            cellHighlighter.DisableInteractable();
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
