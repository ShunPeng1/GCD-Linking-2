using System.Collections.Generic;
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
        
        private readonly List<PortraitButtonRect> _portraitButtonRects = new(); 
        public PortraitButtonRect CreatePortraitButton(PortraitButtonRect portraitButtonRectPrefab)
        {
            var portraitButtonRect = Instantiate(portraitButtonRectPrefab, _characterPortraitButtonGroup.transform);
            _portraitButtonRects.Add(portraitButtonRect);
            return portraitButtonRect;
        }

        public void UpdateImposterRound()
        {
            
        }

        public void UpdateRolePlaying(PlayerRole playingRole)
        {
            _currentTurnText.text = playingRole.ToString() + _currentTurnFormat;
        }

        public void IDK()
        {
            
        }
        
    }
}