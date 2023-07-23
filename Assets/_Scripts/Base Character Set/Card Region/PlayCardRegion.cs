﻿using Shun_Card_System;

namespace _Scripts.Cards.Card_UI
{
    public class PlayCardRegion : BaseCardRegion
    {
        protected override void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            characterCardGameObject.Ability1Button.EnableInteractable();
            characterCardGameObject.Ability2Button.EnableInteractable();
            
        }

        protected override void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            
            characterCardGameObject.Ability1Button.DisableInteractable();
            characterCardGameObject.Ability2Button.DisableInteractable();

        }
    }
}