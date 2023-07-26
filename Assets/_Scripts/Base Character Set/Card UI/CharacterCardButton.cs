using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Cards.Card_UI
{
    public class CharacterCardButton : BaseCardButton
    {
        private BaseCharacterCardGameObject _cardGameObject;
        

        [Header("Components")] 
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        
        [SerializeField] private Color _selectInteractableColor;
        [SerializeField] private Color _normalInteractableColor;
        [SerializeField] private AudioClip _clickSfx;
        public void Initialize(BaseCharacterCardGameObject baseCardGameObject)
        {
            _cardGameObject = baseCardGameObject;
        }
        
        public override void Select()
        {
            _cardGameObject.ExecuteAbility(this);
            _spriteRenderer.color = _selectInteractableColor;
            
            AudioManager.Instance.PlaySFX(_clickSfx);
        }

        public override void Deselect()
        {
            _spriteRenderer.color = _normalInteractableColor;
            
            AudioManager.Instance.PlaySFX(_clickSfx);
        }

        public void ResetSelect()
        {
            _spriteRenderer.color = _normalInteractableColor;
        }
        
        public override void StartHover()
        {
            
        }
        
        public override void EndHover()
        {
            
        }
        
        public override void DisableInteractable()
        {
            base.DisableInteractable();
            
        }
        
        public override void EnableInteractable()
        {
            base.EnableInteractable();
        }
    }
}