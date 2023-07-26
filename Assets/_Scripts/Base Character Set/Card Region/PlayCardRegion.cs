using _Scripts.Cards.Card_UI;
using _Scripts.Input_and_Camera;
using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Base_Character_Set.Card_Region
{
    public class PlayCardRegion : BaseCardRegion
    {
        

        [Header("Audio")] 
        [SerializeField] private AudioClip _addCardSfx;
        
        protected override void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            characterCardGameObject.AbilityButton.EnableInteractable();
            
            
            AudioManager.Instance.PlaySFX(_addCardSfx);
            
            InputManager.Instance.CameraMovement.FocusOnGameObject(characterCardGameObject.CharacterSet.CharacterMapGameObject.gameObject);
            
        }

        protected override void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            
            characterCardGameObject.AbilityButton.DisableInteractable();
            

        }
        
        
        
    }
}