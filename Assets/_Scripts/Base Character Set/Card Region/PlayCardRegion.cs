using _Scripts.Cards.Card_UI;
using _Scripts.Input_and_Camera;
using Shun_Card_System;

namespace _Scripts.Base_Character_Set.Card_Region
{
    public class PlayCardRegion : BaseCardRegion
    {
        
        
        protected override void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            characterCardGameObject.Ability1Button.EnableInteractable();
            characterCardGameObject.Ability2Button.EnableInteractable();
            
            InputManager.Instance.CameraMovement.FocusOnGameObject(characterCardGameObject.CharacterSet.CharacterMapGameObject.gameObject);
            
        }

        protected override void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            
            characterCardGameObject.Ability1Button.DisableInteractable();
            characterCardGameObject.Ability2Button.DisableInteractable();

        }
        
        
        
    }
}