using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Input_and_Camera;
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
        [Header("Components")]
        private readonly List<PortraitButtonRect> _portraitButtonRects = new();
        
        [Header("Sprites")] 
        [SerializeField] private Sprite _imposterSprite;
        [SerializeField] private Sprite _detectiveSprite;
        [SerializeField] private Vector3 _crimeBoardPopInPopInOffset = new Vector3(300, -200, 0);
        [SerializeField] private Vector3 _crimeBoardPopInScale = new Vector3(1.2f, 1.2f, 0);

        
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
        [SerializeField] private string _imposterRecognitionFormat = "I Was In ";
        [SerializeField] private Animator _imposterRecognitionPortraitAnimator;

        [Header("End Round Animation")]
        [SerializeField] private Vector3 _imposterRecognitionPopInOffset = new Vector3(-500,-500,0);
        [SerializeField] private Vector3 _imposterRecognitionPopInScale = new Vector3(1.5f,1.5f,0);
        [SerializeField] private float _imposterRecognitionPopInDuration = 0.5f;
        [SerializeField] private float _imposterRecognitionPopShowDuration = 1.5f;
        [SerializeField] private Ease _imposterRecognitionPopInEase;
        private Sequence _imposterRecognitionPopInSequence;
        private static readonly int IsInDark = Animator.StringToHash("IsInDark");

        [Header("Imposter Selection Animation")] [SerializeField]
        private string _selectionDialog = "Choose your disguise wisely";
        
        [Header("Ultility")] 
        [SerializeField] private float _dialogSpeed = 15f;
        
        private void Start()
        {
            InitializeCurrentTurnGroup();
        }

        void InitializeCurrentTurnGroup()
        {
            
            _currentTurnPopDestination = _currentTurnPanel.position;
            _currentTurnButton.onClick.AddListener(() => _currentTurnPopInSequence.Complete());
        }

        
        public PortraitButtonRect CreatePortraitButton(PortraitButtonRect portraitButtonRectPrefab)
        {
            var portraitButtonRect = Instantiate(portraitButtonRectPrefab, _characterPortraitButtonGroup.transform);
            _portraitButtonRects.Add(portraitButtonRect);
            return portraitButtonRect;
        }

        public void ShowImposterSelection()
        {
            CardManager.Instance.LockPlayCard();
            
            _currentTurnImage.sprite = _imposterSprite;

            _currentTurnText.text = PlayerRole.Imposter.ToString() + _currentTurnFormat;

            Sequence showImposterSelectionSequence = DOTween.Sequence();

            
            Vector3 originalDestination = _currentTurnPopDestination;
            Vector3 popInDestination = _currentTurnPopDestination + _currentTurnPopInOffset;
            
            showImposterSelectionSequence.Append(_currentTurnPanel.DOMove(popInDestination, _currentTurnPopInDuration).SetEase(_currentTurnPopEase));
            showImposterSelectionSequence.AppendInterval(_currentTurnPopShowDuration);
            showImposterSelectionSequence.Append(_currentTurnPanel.DOMove(originalDestination, _currentTurnPopInDuration).SetEase(_currentTurnPopEase));
            showImposterSelectionSequence.AppendInterval(_currentTurnPopShowDuration);

            Vector3 imposterRecognitionPanelOriginalScale = _imposterRecognitionPanel.transform.lossyScale;
            Vector3 crimeBoardPanelOriginalScale = _crimeBoardPanel.transform.lossyScale;
            
            showImposterSelectionSequence.Append(_imposterRecognitionPanel.DOMove(_imposterRecognitionPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            showImposterSelectionSequence.Join(_crimeBoardPanel.DOMove(_crimeBoardPopInPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            showImposterSelectionSequence.Join(_imposterRecognitionPanel.DOScale(_imposterRecognitionPopInScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
            showImposterSelectionSequence.Join(_crimeBoardPanel.DOScale(_crimeBoardPopInScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));

            showImposterSelectionSequence.AppendCallback(() =>
            {
                StartCoroutine(PopInTextWordByWord(_imposterRecognitionText, _selectionDialog, _dialogSpeed));
            });
            
            
            showImposterSelectionSequence.AppendInterval(_imposterRecognitionPopShowDuration);
            
            /*
            
            imposterRecognitionPopInSequence.AppendInterval(_imposterRecognitionPopShowDuration);
            
            
            _imposterRecognitionPopInSequence.Append(_imposterRecognitionPanel.DOMove(-_imposterRecognitionPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_crimeBoardPanel.DOMove(-_crimeBoardPopInPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_imposterRecognitionPanel.DOScale(imposterRecognitionPanelOriginalScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_crimeBoardPanel.DOScale(crimeBoardPanelOriginalScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.AppendCallback(() =>
            {
                CardManager.Instance.UnlockPlayCard();
            });
            
            */
        }

        public void HideImposterSelection()
        {
            

        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K)) UpdateImposterRecognition();
        }

        
        
        public void UpdateImposterRecognition()
        {
            var imposterLastRoundRecognition = GameManager.Instance.ImposterLastRoundRecognition;
            
            CardManager.Instance.LockPlayCard();
            
            _imposterRecognitionPopInSequence.Complete();
            _imposterRecognitionPopInSequence = DOTween.Sequence();

            var imposterRecognitionPanelOriginalScale = _imposterRecognitionPanel.transform.lossyScale;
            var crimeBoardPanelOriginalScale = _crimeBoardPanel.transform.lossyScale;
            _imposterRecognitionPopInSequence.Append(_imposterRecognitionPanel.DOMove(_imposterRecognitionPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_crimeBoardPanel.DOMove(_crimeBoardPopInPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_imposterRecognitionPanel.DOScale(_imposterRecognitionPopInScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_crimeBoardPanel.DOScale(_crimeBoardPopInScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));

            _imposterRecognitionPopInSequence.AppendCallback(() =>
            {
                string textToDisplay = "";
                switch (imposterLastRoundRecognition)
                {
                    case CharacterRecognitionState.InLight:
                        _imposterRecognitionPortraitAnimator.SetBool(IsInDark, false);
                        textToDisplay = _imposterRecognitionFormat + "Light";
                        break;
                    case CharacterRecognitionState.InDark:
                        _imposterRecognitionPortraitAnimator.SetBool(IsInDark, true);
                        textToDisplay = _imposterRecognitionFormat + "Dark";
                        break;
                    case CharacterRecognitionState.Innocent:
                        textToDisplay = "Who am I?";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                StartCoroutine(PopInTextWordByWord(_imposterRecognitionText, textToDisplay, _dialogSpeed));
            });
            
            
            _imposterRecognitionPopInSequence.AppendInterval(_imposterRecognitionPopShowDuration);
            
            
            _imposterRecognitionPopInSequence.Append(_imposterRecognitionPanel.DOMove(-_imposterRecognitionPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_crimeBoardPanel.DOMove(-_crimeBoardPopInPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_imposterRecognitionPanel.DOScale(imposterRecognitionPanelOriginalScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.Join(_crimeBoardPanel.DOScale(crimeBoardPanelOriginalScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
            _imposterRecognitionPopInSequence.AppendCallback(() =>
            {
                CardManager.Instance.UnlockPlayCard();
            });

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
        
        public IEnumerator PopInTextWordByWord(TMP_Text tmpText, string textToDisplay, float charPerSecond = 1f)
        {
            tmpText.text = ""; // Clear the text
            
            foreach (var word in textToDisplay)
            {
                _imposterRecognitionText.text += word; // Append the next word

                // Wait for a short duration before popping in the next word
                yield return new WaitForSeconds(1/charPerSecond);
            }
        }
        
        
        
    }
}