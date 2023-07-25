using System;
using System.Collections.Generic;
using _Scripts.Lights;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityUtilities;

namespace _Scripts.Managers
{
    public class UiManager : SingletonMonoBehaviour<UiManager>
    {

        
        [Header("Crime Board")]
        [SerializeField] private RectTransform _crimeBoardPanel;
        [SerializeField] private GridLayoutGroup _characterPortraitButtonGroup;

        [Header("Current Turn")] 
        [SerializeField] private RectTransform _currentTurnPanel;
        [SerializeField] private TMP_Text _currentTurnText;
        [SerializeField] private string _currentTurnFormat = "'s Turn";
        
        [Header("Current Turn")] 
        [SerializeField] private RectTransform _lastRoundPanel;
        [SerializeField] private TMP_Text _imposterRecognitionText;
        [SerializeField] private string _imposterRecognitionFormat = "Was In ";

        
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
            _currentTurnText.text = GameManager.Instance.CurrentRolePlaying + _currentTurnFormat;
        }

        public void IDK()
        {
            
        }
        
    }
}