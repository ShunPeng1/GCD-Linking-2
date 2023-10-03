using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Input_and_Camera;
using _Scripts.Lights;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private Button _portraitSelectionButton;
        [SerializeField] private Vector3 _portraitSelectionOffsetPosition = new Vector3(0, -100, 0);
        private PortraitButtonRect _choosingPortraitButton;
        private readonly Dictionary<PortraitButtonRect,UnityAction> _portraitClickListeners = new();
        
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
        [SerializeField] private AudioClip _dragSfx;
        [SerializeField] private float _dialogSpeed = 15f;
        
        
        private void Start()
        {
            InitializeCurrentTurnGroup();
            InitializeCrimeBoard();
        }

        void InitializeCurrentTurnGroup()
        {
            
            _currentTurnPopDestination = _currentTurnPanel.position;
            _currentTurnButton.onClick.AddListener(() => _currentTurnPopInSequence.Complete());
        }

        void InitializeCrimeBoard()
        {
            _portraitSelectionButton.onClick.AddListener( ()=>
            {
                if (_choosingPortraitButton == null) return;
                
                _choosingPortraitButton.SetImposterSelect(false);
                
                
                GameManager.Instance.StartGame(_choosingPortraitButton.CharacterSet);
                foreach (var portraitButtonRect in _portraitButtonRects)
                {
                    portraitButtonRect.Button.onClick.RemoveListener(_portraitClickListeners[portraitButtonRect]);
                }
            });
        }

        
        public PortraitButtonRect CreatePortraitButton(PortraitButtonRect portraitButtonRectPrefab)
        {
            var portraitButtonRect = Instantiate(portraitButtonRectPrefab, _characterPortraitButtonGroup.transform);
            _portraitButtonRects.Add(portraitButtonRect);

            _portraitClickListeners[portraitButtonRect] = () => ChoosePortrait(portraitButtonRect);
            portraitButtonRect.Button.onClick.AddListener(_portraitClickListeners[portraitButtonRect]);
            
            return portraitButtonRect;
        }

        private void ChoosePortrait(PortraitButtonRect portraitButtonRect)
        {
            if (_choosingPortraitButton != null) _choosingPortraitButton.SetImposterSelect(false);
            _choosingPortraitButton = portraitButtonRect;
            _choosingPortraitButton.SetImposterSelect(true);
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
            showImposterSelectionSequence.Join(_portraitSelectionButton.transform.DOLocalMove(_portraitSelectionOffsetPosition, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));

            AudioManager.Instance.PlaySFX(_dragSfx);
                
            showImposterSelectionSequence.AppendCallback(() =>
            {
                StartCoroutine(PopInTextWordByWord(_imposterRecognitionText, _selectionDialog, _dialogSpeed));
            });
            
            
            showImposterSelectionSequence.AppendInterval(_imposterRecognitionPopShowDuration);



            GameManager.Instance.StartGameAction += () =>
            {
                Sequence hideImposterSelectionSequence = DOTween.Sequence();
                hideImposterSelectionSequence.Pause();
                hideImposterSelectionSequence.Append(_imposterRecognitionPanel.DOMove(-_imposterRecognitionPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
                hideImposterSelectionSequence.Join(_crimeBoardPanel.DOMove(-_crimeBoardPopInPopInOffset, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));
                hideImposterSelectionSequence.Join(_imposterRecognitionPanel.DOScale(imposterRecognitionPanelOriginalScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
                hideImposterSelectionSequence.Join(_crimeBoardPanel.DOScale(crimeBoardPanelOriginalScale, _imposterRecognitionPopInDuration).SetEase(_currentTurnPopEase));
                hideImposterSelectionSequence.Join(_portraitSelectionButton.transform.DOLocalMove(-_portraitSelectionOffsetPosition, _imposterRecognitionPopInDuration).SetRelative().SetEase(_currentTurnPopEase));

                hideImposterSelectionSequence.AppendCallback(() =>
                {
                    CardManager.Instance.UnlockPlayCard();
                });

                hideImposterSelectionSequence.Play();
                
                
                AudioManager.Instance.PlaySFX(_dragSfx);
            };
        }

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K)) UpdateImposterRecognition();
        }

        
        
        public void UpdateImposterRecognition()
        {
            var imposterLastRoundRecognition = GameManager.Instance.ImposterLastRoundRecognition;
            
            CardManager.Instance.LockPlayCard();
            AudioManager.Instance.PlaySFX(_dragSfx);
            
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
                
                AudioManager.Instance.PlaySFX(_dragSfx);
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
            
            AudioManager.Instance.PlaySFX(_dragSfx);
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

        public void EndGame(PlayerRole playerRole, string display)
        {
            CardManager.Instance.LockPlayCard();
            CardManager.Instance.CardRegionsParent.SetActive(false);
            
            _currentTurnImage.sprite = playerRole switch
            {
                PlayerRole.Detective => _detectiveSprite,
                PlayerRole.Imposter => _imposterSprite,
                _ => throw new ArgumentOutOfRangeException()
            };
            

            _currentTurnText.text = playerRole + " Win!\n" + display;

            _currentTurnPopInSequence.Kill();
            Sequence endGameSequence = DOTween.Sequence();

            Vector3 popInDestination = _currentTurnPopDestination + _currentTurnPopInOffset;
            
            endGameSequence.Append(_currentTurnPanel.DOMove(popInDestination, _currentTurnPopInDuration).SetEase(_currentTurnPopEase));
            endGameSequence.Join(_crimeBoardCanvasGroup.DOFade(0, _currentTurnPopInDuration).SetEase(_currentTurnPopEase));
            endGameSequence.Join(_imposterRecognitionCanvasGroup.DOFade(0, _currentTurnPopInDuration).SetEase(_currentTurnPopEase));

            

            
        }
        
    }
}