using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Base_Character_Set.Card_Region
{
    public class DeckCardRegion : BaseCardRegion
    {
    

        [Header("Audio")] 
        [SerializeField] private AudioClip _addCardSfx;
        protected override void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            characterCardGameObject.DisableInteractable();
            characterCardGameObject.AbilityButton.DisableInteractable();
            

            characterCardGameObject.SortingGroup.sortingOrder = -index;
            
            AudioManager.Instance.PlaySFX(_addCardSfx);
        }

        protected override void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder, int index)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            characterCardGameObject.SortingGroup.sortingOrder = index;
        }
    
    
    }
}
