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
    protected float MoveSpeed => CharacterInformation.MoveSpeed;
    
    protected CharacterInformation CharacterInformation;
    protected BaseCharacterCardGameObject CharacterCardGameObject;
    
    
    [Header("Grid")]
    public Vector3 NextCellPosition;
    public Vector3 LastCellPosition;
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

    [Header("Components")]
    protected Animator Animator;
    protected bool IsTweenAnimation = false;
    private static readonly int XDirection = Animator.StringToHash("XDirection");
    private static readonly int YDirection = Animator.StringToHash("YDirection");

    #region INITIALIZE
    
    protected void Start()
    {
        Animator = GetComponent<Animator>();
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
        
        LastCellPosition = NextCellPosition = Grid.GetWorldPositionOfNearestCell(transform.position);

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

    protected void MoveAlongMovingPath()
    {
        if (IsTweenAnimation) return;
        
        // Move
        transform.position = Vector3.MoveTowards(transform.position, NextCellPosition, MoveSpeed * Time.fixedDeltaTime);
        IsBetween2Cells = true;
        
        //Animation
        Animator.SetFloat(XDirection, ShunMath.GetSignOrZero(NextCellPosition.x - transform.position.x));
        Animator.SetFloat(YDirection, ShunMath.GetSignOrZero(NextCellPosition.y - transform.position.y));
        
    }

    protected bool CheckArriveCell()
    {
        if (Vector3.Distance(transform.position, NextCellPosition) != 0) return true;
            
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
    
    protected void ExtractNextCellInPath()
    {
        if (MovingPath == null || MovingPath.Count == 0)
        {
            LastCellPosition = NextCellPosition;
            return;
        }
        
        var nextNextCell = MovingPath.First.Value;
        MovingPath.RemoveFirst(); // the next standing node

        Vector3 nextNextCellPosition = Grid.GetWorldPositionOfNearestCell(nextNextCell.XIndex, nextNextCell.YIndex) ;

        LastCellPosition = NextCellPosition;
        NextCellPosition = nextNextCellPosition;
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

    #region ABILITY

    public virtual void MoveAbility()
    {
        ShowMovablePath();
        CellHighlightSelectMouseInput mouseInput = new CellHighlightSelectMouseInput(Grid, FinishSelectCell);
        InputManager.Instance.ChangeMouseInput(mouseInput);
    }

    private void FinishSelectCell(CellSelectHighlighter cellSelectHighlighter)
    {
        if (cellSelectHighlighter != null)
        {
            var newTask = new CharacterMovementTask(
                CharacterMovementTask.StartPosition.NextCell,
                cellSelectHighlighter.transform.position, 
                () => StateMachine.SetToState(CharacterMovementState.Idling));
            StateMachine.SetToState(CharacterMovementState.Moving, null, new object[]{newTask});
        }
        HideMovablePath();
    }
    
    public virtual void SecondAbility()
    {
        
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
    

    #endregion
}
