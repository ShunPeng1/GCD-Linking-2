using System;
using System.Collections.Generic;
using Shun_Card_System;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.Cards.Card_UI
{
    public class BaseCharacterCardGameObject : BaseCardGameObject
    {
        protected CharacterInformation CharacterInformation;
        protected BaseCharacterMapGameObject CharacterMapGameObject;
        
        [Header("Child Components")]
        [SerializeField] private CharacterCardButton _ability1Button;
        [SerializeField] private CharacterCardButton _ability2Button;
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
            _ability1Button.Initialize(this);
            _ability2Button.Initialize(this);
        }
        
        public void InitializeCharacter(CharacterInformation characterInformation, BaseCharacterMapGameObject characterMapGameObject )
        {
            CharacterInformation = characterInformation;
            CharacterMapGameObject = characterMapGameObject;
        
            if (CharacterInformation == null)
            {
                Debug.LogWarning("No information in "+ gameObject.name);
                return;
            }
            
            _executeAbilityBaseOnButton.Add(_ability1Button, CharacterMapGameObject.MoveAbility);
            _executeAbilityBaseOnButton.Add(_ability2Button, CharacterMapGameObject.SecondAbility);
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