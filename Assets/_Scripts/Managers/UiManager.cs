using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtilities;

namespace _Scripts.Managers
{
    public class UiManager : SingletonMonoBehaviour<UiManager>
    {

        [SerializeField] private RectTransform _crimeBoard;
        [SerializeField] private GridLayoutGroup _characterPortraitButtonGroup;

        private List<PortraitButtonRect> _portraitButtonRects = new(); 
        public PortraitButtonRect CreatePortraitButton(PortraitButtonRect portraitButtonRectPrefab)
        {
            var portraitButtonRect = Instantiate(portraitButtonRectPrefab, _characterPortraitButtonGroup.transform);
            _portraitButtonRects.Add(portraitButtonRect);
            return portraitButtonRect;
        }

    }
}