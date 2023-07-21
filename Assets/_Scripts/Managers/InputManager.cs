using System;
using System.Collections;
using UnityEngine;
using UnityUtilities;

namespace _Scripts.Input_and_Camera
{
    public class InputManager : SingletonMonoBehaviour<InputManager>
    {
        [SerializeField] private CameraMovement _cameraMovement;
        [SerializeField] private float _changeInputCooldown = 0.5f;
        
        private readonly BaseCardMouseInput _defaultMouseInput = new BaseCardMouseInput();
        private CellHighlightSelectMouseInput _currentMouseInput = null;
        private bool _isInputInteractable = true;
        void Update()
        {
            if (!_isInputInteractable) return;
            
            _defaultMouseInput.UpdateMouseInput();
            
            if(_currentMouseInput != null && _isInputInteractable) 
                _currentMouseInput.UpdateMouseInput();
            
        }

        public void ChangeMouseInput(CellHighlightSelectMouseInput mouseInput)
        {
            _currentMouseInput = mouseInput;
            _currentMouseInput.AddFinishedSelection(RemoveCurrentMouseInput);
            _isInputInteractable = false;
            StartCoroutine(ChangMouseInputCooldown(mouseInput));
        }

        IEnumerator ChangMouseInputCooldown(CellHighlightSelectMouseInput mouseInput)
        {
            yield return new WaitForSeconds(_changeInputCooldown);
            _isInputInteractable = true;
        }

        private void RemoveCurrentMouseInput()
        {
            _currentMouseInput = null;
        }
        
    }
}