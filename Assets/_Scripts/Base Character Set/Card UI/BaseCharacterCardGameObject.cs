using System;
using System.Collections.Generic;
using Shun_Card_System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace _Scripts.Cards.Card_UI
{
    public class BaseCharacterCardGameObject : BaseCardGameObject
    {
        public CharacterSet CharacterSet { get; private set; }


        [Header("Child Components")]
        public CharacterCardButton AbilityButton;

        [SerializeField] private TMP_Text  _nameText;
        [SerializeField] private TMP_Text  _abilityDescription;
        [SerializeField] private Transform _cardVisualTransform;
        [FormerlySerializedAs("_sortingGroup")] public SortingGroup SortingGroup;

        private CharacterCardButton _selectingButton;
        private readonly Dictionary<CharacterCardButton,Action<Action, Action>> _executeAbilityBaseOnButton = new();
        private readonly Dictionary<CharacterCardButton,Func<Action, Action, bool>> _forceEndAbilityBaseOnButton = new();
        private readonly Dictionary<CharacterCardButton, int> _remainingUseCountBaseOnButtons = new();
        private readonly Dictionary<CharacterCardButton, int> _originalUseCountBaseOnButtons = new();
        

        [Header("Hover")]
        [SerializeField] private float _hoverEnlargeValue = 1.5f;

        [SerializeField] private AudioClip _hoverSfx;
        
        private void Start()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            AbilityButton.Initialize(this);
            
        }
        
        public void InitializeCharacter(CharacterSet characterSet)
        {
            CharacterSet = characterSet;
            
            if (CharacterSet.CharacterInformation == null)
            {
                Debug.LogWarning("No information in "+ gameObject.name);
                return;
            }
            
            _executeAbilityBaseOnButton.Add(AbilityButton, CharacterSet.CharacterMapGameObject.MoveAbility);
            
            
            _forceEndAbilityBaseOnButton.Add(AbilityButton, CharacterSet.CharacterMapGameObject.ForceEndMoveAbility);
            
            
            _originalUseCountBaseOnButtons.Add(AbilityButton, CharacterSet.CharacterInformation.Ability1UseCount);

            _abilityDescription.text = CharacterSet.CharacterInformation.Ability1Description;
            _nameText.text = CharacterSet.CharacterInformation.CharacterName;
            
            ResetAbilityUse();
        }

        public virtual void ExecuteAbility(CharacterCardButton cardButton)
        {
            if (_remainingUseCountBaseOnButtons[cardButton] <= 0) return;
            
            if (_selectingButton == null)
            {
                _executeAbilityBaseOnButton[cardButton].Invoke(() => SuccessfullySelectionAbility(_selectingButton), () => _selectingButton = null);
                _selectingButton = cardButton;

                LockCardPlay();
            }
            else if (_selectingButton == cardButton)
            {
                if (_forceEndAbilityBaseOnButton[cardButton].Invoke(null,null))
                {
                    _selectingButton.Deselect();
                    _selectingButton = null;
                    UnlockCardPlay();
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
                    _executeAbilityBaseOnButton[cardButton].Invoke(() => SuccessfullySelectionAbility(_selectingButton), () => _selectingButton = null);
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
            
            UnlockCardPlay();
            
            if (CheckAllExhaustUse())
            {
                // End Card
                CardManager.Instance.ExhaustCard(this);
                ResetAbilityUse();
                
            }
            else
            {
                
            }
        }

        protected void LockCardPlay()
        {
            CardManager.Instance.LockPlayCard();
        }

        protected void UnlockCardPlay()
        {
            CardManager.Instance.UnlockPlayCard();
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
            }
        }
        
        protected override void ValidateInformation()
        {
            
            
        }


        public override void StartHover()
        {
            base.StartHover();
            _cardVisualTransform.localScale *= _hoverEnlargeValue;
            SortingGroup.sortingOrder +=100;
            
            AudioManager.Instance.PlaySFX(_hoverSfx);
        }
        
        public override void EndHover()
        {
            base.EndHover();
            _cardVisualTransform.localScale /= _hoverEnlargeValue;
            SortingGroup.sortingOrder -=100;
            
            
        }
    }
}