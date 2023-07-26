using _Scripts.Cards.Card_UI;
using Shun_Card_System;

namespace _Scripts.Base_Character_Set.Card_Region
{
    public class DeckCardRegion : BaseCardRegion
    {
    
        protected override void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            characterCardGameObject.DisableInteractable();
            characterCardGameObject.Ability1Button.DisableInteractable();
            characterCardGameObject.Ability2Button.DisableInteractable();

            characterCardGameObject.SortingGroup.sortingOrder = -index;
        }

        protected override void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            characterCardGameObject.SortingGroup.sortingOrder = index;
        }
    
    
    }
}
