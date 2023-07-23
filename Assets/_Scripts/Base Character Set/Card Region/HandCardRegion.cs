﻿using Shun_Card_System;

namespace _Scripts.Cards.Card_UI
{
    public class HandCardRegion : BaseCardRegion
    {
        protected override void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            characterCardGameObject.Ability1Button.Interactable = false;
            characterCardGameObject.Ability2Button.Interactable = false;
            
        }

        protected override void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            var characterCardGameObject = (BaseCharacterCardGameObject) baseCardGameObject;
            
            characterCardGameObject.Ability1Button.Interactable = true;
            characterCardGameObject.Ability2Button.Interactable = true;

        }
    }
}