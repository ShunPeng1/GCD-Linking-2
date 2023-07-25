using System;
using System.Collections.Generic;
using _Scripts.Lights;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityUtilities;

namespace _Scripts.Managers
{
    public class UiManager : SingletonMonoBehaviour<UiManager>
    {
        [Header("Sprites")] 
        [SerializeField] private Sprite _imposterSprite;
        [SerializeField] private Sprite _detectiveSprite;
        
        [Header("Crime Board")] 
        [SerializeField] private CanvasGroup _crimeBoardCanvasGroup;
        [SerializeField] private RectTransform _crimeBoardPanel;
        [SerializeField] private GridLayoutGroup _characterPortraitButtonGroup;

        [Header("Current Turn")] 
        [SerializeField] private CanvasGroup _currentTurnCanvasGroup;
        [SerializeField] private RectTransform _currentTurnPanel;
        [SerializeField] private Image _currentTurnImage;
        [SerializeField] private Button _currentTurnButton;
        [SerializeField] private TMP_Text _currentTurnText;
        [SerializeField] private string _currentTurnFormat = "'s Turn";
        
        [SerializeField] private Vector3 _currentTurnPopInOffset = new Vector3(0, -3f, 0);
        [SerializeField] private float _currentTurnPopInDuration = 0.5f;
        [SerializeField] private float _currentTurnPopShowDuration = 2f;
        [SerializeField] private Ease _currentTurnPopEase;
        private Vector3 _currentTurnPopDestination;
        private Sequence _currentTurnPopInSequence;
        
        
        [Header("Imposter Recognition")] 
        [SerializeField] private CanvasGroup _imposterRecognitionCanvasGroup;
        [SerializeField] private RectTransform _imposterRecognitionPanel;
        [SerializeField] private TMP_Text _imposterRecognitionText;
        [SerializeField] private string _imposterRecognitionFormat = "Was In ";

        private void Start()
        {
            InitializeCurrentTurnGroup();
        }

        void InitializeCurrentTurnGroup()
        {
            
            _currentTurnPopDestination = _currentTurnPanel.position;
            _currentTurnButton.onClick.AddListener(() => _currentTurnPopInSequence.Complete());
        }

        private readonly List<PortraitButtonRect> _portraitButtonRects = new(); 
        public PortraitButtonRect CreatePortraitButton(PortraitButtonRect portraitButtonRectPrefab)
        {
            var portraitButtonRect = Instantiate(portraitButtonRectPrefab, _characterPortraitButtonGroup.transform);
            _portraitButtonRects.Add(portraitButtonRect);
            return portraitButtonRect;
        }

        public void UpdateImposterRecognition()
        {
            switch (GameManager.Instance.ImposterSet.CharacterRecognition.Value)
            {
                case CharacterRecognitionState.InLight:
                    _imposterRecognitionText.text = _imposterRecognitionFormat + "Light";
                    break;
                case CharacterRecognitionState.InDark:
                    _imposterRecognitionText.text = _imposterRecognitionFormat + "Dark";
                    break;
                case CharacterRecognitionState.Innocent:
                    _imposterRecognitionText.text = "Imposter Was Found";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdateRolePlaying()
        {
            _currentTurnImage.sprite = GameManager.Instance.CurrentRolePlaying switch
            {
                PlayerRole.Detective => _detectiveSprite,
                PlayerRole.Imposter => _imposterSprite,
                _ => throw new ArgumentOutOfRangeException()
            };

            _currentTurnText.text = GameManager.Instance.CurrentRolePlaying + _currentTurnFormat;

            _currentTurnPopInSequence.Complete();
            _currentTurnPopInSequence = DOTween.Sequence();
            
            Vector3 originalDestination = _currentTurnPopDestination;
            Vector3 popInDestination = _currentTurnPopDestination + _currentTurnPopInOffset;
            
            _currentTurnPopInSequence.Append(_currentTurnPanel.DOMove(popInDestination, _currentTurnPopInDuration).SetEase(_currentTurnPopEase));
            _currentTurnPopInSequence.AppendInterval(_currentTurnPopShowDuration);
            _currentTurnPopInSequence.Append(_currentTurnPanel.DOMove(originalDestination, _currentTurnPopInDuration).SetEase(_currentTurnPopEase));
            
        }

        public void IDK()
        {
            
        }
        
    }
}