using System;
using DG.Tweening;
using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Cards.Card_UI
{
    public class HandCardRegion : BaseCardRegion
    {
        [SerializeField] private Vector3 _moveInteractableOffset = new Vector3(0,-2, 0);
        [SerializeField] private float _moveInteractableDuration = 0.25f;
        [SerializeField] private Ease _moveInteractableEase = Ease.OutCubic;
        private Vector3 _destinationPosition;


        private void Start()
        {
            _destinationPosition = transform.localPosition;
        }

        protected override void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            
            characterCardGameObject.Ability1Button.DisableInteractable();
            characterCardGameObject.Ability2Button.DisableInteractable();
            
        }

        protected override void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            //var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            

        }

        public override void EnableInteractable()
        {
            base.EnableInteractable();
            _destinationPosition += _moveInteractableOffset; 
            transform.DOLocalMove(_destinationPosition, _moveInteractableDuration).SetEase(_moveInteractableEase);

        }

        public override void DisableInteractable()
        {
            base.DisableInteractable();
            _destinationPosition -= _moveInteractableOffset; 
            transform.DOLocalMove(_destinationPosition, _moveInteractableDuration).SetEase(_moveInteractableEase);
        }
    }
}