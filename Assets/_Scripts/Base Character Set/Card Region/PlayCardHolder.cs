using DG.Tweening;
using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Cards.Card_UI
{
    public class PlayCardHolder : BaseCardHolder
    {
        [Header("Tween")] 
        [SerializeField] private float _addDuration = 0.25f;
        [SerializeField] private Ease _addEase = Ease.InCubic;
        
        protected override void AttachCardVisual()
        {
            CardGameObject.transform.DOLocalMove(Vector3.zero, _addDuration).SetEase(_addEase)
                .OnComplete(CardGameObject.EnableInteractable);
        }
        
        
    }
}