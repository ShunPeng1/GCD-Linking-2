using System;
using UnityEngine;


public class CharacterMovementTask 
{
    public StartPosition StartCellPosition;
    public Vector3 GoalCellPosition;
    public readonly Action GoalArrivalAction;
    public int Priority;
    
    public enum StartPosition
    {
        LastCell,
        NextCell,
        NearestCell
    }

    public CharacterMovementTask(StartPosition startCellPosition,Vector3 goalCellPosition, Action goalArrivalAction = null, int priority = 0)
    {
        StartCellPosition = startCellPosition;
        GoalCellPosition = goalCellPosition;
        GoalArrivalAction = goalArrivalAction;
        Priority = priority;
    }


    
}
