using DG.Tweening;
using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Base_Character_Set.Card_Region
{
    public class PlayCardHolder : BaseCardHolder
    {
        [Header("Tween")] 
        [SerializeField] private float _addDuration = 0.25f;
        [SerializeField] private Ease _addEase = Ease.InCubic;
        
        protected override void AttachCardVisual()
        {
            Transform cardTransform = CardGameObject.transform;
            cardTransform.DOLocalMove(Vector3.zero, _addDuration).SetEase(_addEase)
                .OnComplete(CardGameObject.EnableInteractable);

            
            cardTransform.DOScale(Vector3.one , _addDuration).SetEase(_addEase);
        }
        
        
    }
}