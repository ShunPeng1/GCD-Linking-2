using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Cards.Card_UI
{
    public class CharacterCardButton : BaseCardButton
    {
        private BaseCharacterCardGameObject _cardGameObject;

        public void Initialize(BaseCharacterCardGameObject baseCardGameObject)
        {
            _cardGameObject = baseCardGameObject;
        }
        
        public override void Select()
        {
            _cardGameObject.ExecuteAbility(this);
        }

        public override void Deselect()
        {
            
        }

        
        public override void StartHover()
        {
            
        }
        
        public override void EndHover()
        {
            
        }
        
        public override void DisableInteractable()
        {
            Interactable = false;
        }
        
        public override void EnableInteractable()
        {
            Interactable = true;
        }
    }
}