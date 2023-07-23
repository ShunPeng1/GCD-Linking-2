﻿using System;
using System.Collections.Generic;
using Shun_Card_System;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.Cards.Card_UI
{
    public class BaseCharacterCardGameObject : BaseCardGameObject
    {
        protected CharacterInformation CharacterInformation;
        protected BaseCharacterMapMovableGameObject CharacterMapMovableGameObject;
        
        [Header("Child Components")]
        public CharacterCardButton Ability1Button;
        public CharacterCardButton Ability2Button;
        [SerializeField] private Transform _cardVisualTransform;
        [SerializeField] private SortingGroup _sortingGroup;
        
        private Dictionary<CharacterCardButton,Action> _executeAbilityBaseOnButton = new();

        [Header("Hover")] 
        
        [SerializeField] private float _hoverEnlargeValue = 1.5f;
        
        private void Start()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Ability1Button.Initialize(this);
            Ability2Button.Initialize(this);
        }
        
        public void InitializeCharacter(CharacterInformation characterInformation, BaseCharacterMapMovableGameObject characterMapMovableGameObject )
        {
            CharacterInformation = characterInformation;
            CharacterMapMovableGameObject = characterMapMovableGameObject;
        
            if (CharacterInformation == null)
            {
                Debug.LogWarning("No information in "+ gameObject.name);
                return;
            }
            
            _executeAbilityBaseOnButton.Add(Ability1Button, CharacterMapMovableGameObject.MoveAbility);
            _executeAbilityBaseOnButton.Add(Ability2Button, CharacterMapMovableGameObject.SecondAbility);
        }

        public void ExecuteAbility(CharacterCardButton cardButton)
        {
            _executeAbilityBaseOnButton[cardButton].Invoke();
        }

        protected override void ValidateInformation()
        {
            
            
        }


        public override void StartHover()
        {
            _cardVisualTransform.localScale *= _hoverEnlargeValue;
            _sortingGroup.sortingOrder +=100;
        }
        
        public override void EndHover()
        {
            _cardVisualTransform.localScale /= _hoverEnlargeValue;
            _sortingGroup.sortingOrder -=100;
        }
    }
}