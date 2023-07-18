using System;
using System.Collections.Generic;
using Shun_Card_System;
using UnityEngine;

namespace _Scripts.Cards.Card_UI
{
    public class CharacterCardGameObject : BaseCardGameObject
    {
        [SerializeField] private CharacterInformation _characterInformation;
        
        [Header("Child Components")]
        [SerializeField] private CharacterCardButton _ability1Button;
        [SerializeField] private CharacterCardButton _ability2Button;

        private Dictionary<CharacterCardButton,Action> _executeAbilityBaseOnButton = new();
        
        private void Start()
        {
            InitializeComponents();
            InitializeInformation();
        }

        private void InitializeComponents()
        {
            _ability1Button.Initialize(this);
            _ability2Button.Initialize(this);
        }
        
        private void InitializeInformation()
        {
            if (_characterInformation == null)
            {
                Debug.LogWarning("No information in "+ gameObject.name);
                return;
            }
            
            _executeAbilityBaseOnButton.Add(_ability1Button, _characterInformation.Ability1);
            _executeAbilityBaseOnButton.Add(_ability2Button, _characterInformation.Ability2);
        }

        public void ExecuteAbility(CharacterCardButton cardButton)
        {
            _executeAbilityBaseOnButton[cardButton].Invoke();
        }

        protected override void ValidateInformation()
        {
            
            
        }
    }
}