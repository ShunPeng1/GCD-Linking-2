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
        protected BaseCharacterMapDynamicGameObject CharacterMapDynamicGameObject;
        
        [Header("Child Components")]
        public CharacterCardButton Ability1Button;
        public CharacterCardButton Ability2Button;
        [SerializeField] private Transform _cardVisualTransform;
        [SerializeField] private SortingGroup _sortingGroup;

        private CharacterCardButton _selectingButton;
        private readonly Dictionary<CharacterCardButton,Action<Action, Action>> _executeAbilityBaseOnButton = new();
        private readonly Dictionary<CharacterCardButton,Func<Action, Action, bool>> _forceEndAbilityBaseOnButton = new();
        private readonly Dictionary<CharacterCardButton, int> _remainingUseCountBaseOnButtons = new();
        private readonly Dictionary<CharacterCardButton, int> _originalUseCountBaseOnButtons = new();
        

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
        
        public void InitializeCharacter(CharacterInformation characterInformation, BaseCharacterMapDynamicGameObject characterMapDynamicGameObject )
        {
            CharacterInformation = characterInformation;
            CharacterMapDynamicGameObject = characterMapDynamicGameObject;
        
            if (CharacterInformation == null)
            {
                Debug.LogWarning("No information in "+ gameObject.name);
                return;
            }
            
            _executeAbilityBaseOnButton.Add(Ability1Button, CharacterMapDynamicGameObject.MoveAbility);
            _executeAbilityBaseOnButton.Add(Ability2Button, CharacterMapDynamicGameObject.SecondAbility);
            
            _forceEndAbilityBaseOnButton.Add(Ability1Button, CharacterMapDynamicGameObject.ForceEndMoveAbility);
            _forceEndAbilityBaseOnButton.Add(Ability2Button, CharacterMapDynamicGameObject.ForceEndSecondAbility);
            
            _originalUseCountBaseOnButtons.Add(Ability1Button, CharacterInformation.Ability1UseCount);
            _originalUseCountBaseOnButtons.Add(Ability2Button, CharacterInformation.Ability2UseCount);

            ResetAbilityUse();
        }

        public virtual void ExecuteAbility(CharacterCardButton cardButton)
        {
            if (_remainingUseCountBaseOnButtons[cardButton] <= 0) return;
            
            if (_selectingButton == null)
            {
                _executeAbilityBaseOnButton[cardButton].Invoke(() => SuccessfullySelectionAbility(_selectingButton), () => _selectingButton = null);
                _selectingButton = cardButton;
            }
            else if (_selectingButton == cardButton)
            {
                if (_forceEndAbilityBaseOnButton[cardButton].Invoke(null,null))
                {
                    _selectingButton.Deselect();
                    _selectingButton = null;
                }
                else
                {
                    Debug.Log("Can not force end ability");
                }
            }
            else if(_selectingButton != cardButton)
            {
                if (_forceEndAbilityBaseOnButton[_selectingButton].Invoke(null,null))
                {
                    _selectingButton.Deselect();
                    _executeAbilityBaseOnButton[cardButton].Invoke(() => _selectingButton = null, () => _selectingButton = null);
                    _selectingButton = cardButton;
                }
                else
                {
                    Debug.Log("Can not force end ability");
                }
            }

        }

        protected virtual void SuccessfullySelectionAbility(CharacterCardButton cardButton)
        {
            _selectingButton = null;
            cardButton.DisableInteractable();
            _remainingUseCountBaseOnButtons[cardButton]--;

            if (CheckAllExhaustUse())
            {
                // End Card
            }
        }

        protected virtual bool CheckAllExhaustUse()
        {
            int totalCount = 0;
            foreach (var (cardButton, count) in _remainingUseCountBaseOnButtons)
            {
                totalCount += count;
            }

            return totalCount <= 0;
        }
        
        protected virtual void ResetAbilityUse()
        {
            foreach (var (cardButton, count) in _originalUseCountBaseOnButtons)
            {
                _remainingUseCountBaseOnButtons[cardButton] = count;
                cardButton.EnableInteractable();
            }
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