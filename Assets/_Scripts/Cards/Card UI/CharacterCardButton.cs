using Shun_Card_System;

namespace _Scripts.Cards.Card_UI
{
    public class CharacterCardButton : BaseCardButton
    {
        private CharacterCardGameObject _cardGameObject;

        public void Initialize(CharacterCardGameObject baseCardGameObject)
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

        
        public override void Hover()
        {
            
        }
        
        public override void Unhover()
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