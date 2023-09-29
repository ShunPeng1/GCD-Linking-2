

using System;
using DG.Tweening;
using Shun_Grid_System;
using Shun_Utility;
using UnityEngine;

[RequireComponent(typeof(BaseCharacterMapDynamicGameObject))]
public class CharacterMovementVisual : MonoBehaviour
{
    [Header("Information")] 
    [SerializeField] private BaseCharacterMapDynamicGameObject _characterMapGameObject;
    protected GridXY<MapCellItem> Grid => MapManager.Instance.WorldGrid; 

    [Header("Animation")]
    [SerializeField] protected Animator Animator;
    protected bool IsTweenAnimation = false;
    private static readonly int XDirection = Animator.StringToHash("XDirection");
    private static readonly int YDirection = Animator.StringToHash("YDirection");


    [Header("Vent Animation")] 
    [SerializeField] private float _enterAndExitVentDuration = 0.333f;
    [SerializeField] private Ease _characterMoveEase;
    public virtual void Move(GridXYCell<MapCellItem> lastCell, GridXYCell<MapCellItem> nextCell)
    {
        if (IsTweenAnimation) return;
        
        var nextVent = nextCell.Item.GetFirstInCellGameObject<VentMapGameObject>();
        var lastVent = lastCell.Item.GetFirstInCellGameObject<VentMapGameObject>();
        if (nextVent != null && lastVent != null)
        {
            MoveByVent(lastVent, nextVent);
        }
        else
        {
            MoveOnFoot(Grid.GetWorldPositionOfNearestCell(lastCell),  Grid.GetWorldPositionOfNearestCell(nextCell));
        }
        
    }

    private void MoveOnFoot(Vector3 fromPosition, Vector3 nextCellPosition)
    {
        
        transform.position = Vector3.MoveTowards(transform.position, nextCellPosition, _characterMapGameObject.MoveSpeed * Time.fixedDeltaTime);

        //Animation
        Animator.SetFloat(XDirection, ShunMath.GetSignOrZero(nextCellPosition.x - transform.position.x));
        Animator.SetFloat(YDirection, ShunMath.GetSignOrZero(nextCellPosition.y - transform.position.y));
    }


    private void MoveByVent(VentMapGameObject lastVent, VentMapGameObject nextVent)
    {
        IsTweenAnimation = true;

        Sequence mySequence = DOTween.Sequence();
        
        mySequence.InsertCallback(0f, () => lastVent.UseVent(true));
        //mySequence.Append(transform.DOMoveY(1, _enterAndExitDuration).SetRelative());
        
        mySequence.InsertCallback(_enterAndExitVentDuration, 
            () => {
                lastVent.UseVent(false);
                nextVent.UseVent(true);
            });
       
        mySequence.InsertCallback(_enterAndExitVentDuration * 2, 
            () => {
                nextVent.UseVent(false);
                transform.position = nextVent.transform.position;
                IsTweenAnimation = false;
            });
        
        
    }
}