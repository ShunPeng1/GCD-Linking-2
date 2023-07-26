using System;
using System.Collections;
using Shun_Card_System;
using UnityEngine;
using UnityUtilities;

namespace _Scripts.Input_and_Camera
{
    public class InputManager : SingletonMonoBehaviour<InputManager>
    {
        public CameraMovement CameraMovement;
        [SerializeField] private float _changeInputCooldown = 0.2f;
        
        private readonly BaseCardMouseInput _defaultMouseInput = new BaseCardMouseInput();
        private CellSelectMouseInput _currentMouseInput = null;
        private bool _isInputInteractable = true;
        void Update()
        {
            if (!_isInputInteractable) return;
            
            _defaultMouseInput.UpdateMouseInput();
            
            if(_currentMouseInput != null && _isInputInteractable) 
                _currentMouseInput.UpdateMouseInput();
            
        }

        public void DisableInput()
        {
            _isInputInteractable = false;
        }
        
        
        public void EnableInput()
        {
            _isInputInteractable = true;
        }
        
        public void ChangeMouseInput(CellSelectMouseInput mouseInput)
        {
            _currentMouseInput = mouseInput;
            _currentMouseInput.AddFinishedSelection(RemoveCurrentMouseInput);
            _isInputInteractable = false;
            StartCoroutine(ChangMouseInputCooldown(mouseInput));
        }

        IEnumerator ChangMouseInputCooldown(CellSelectMouseInput mouseInput)
        {
            yield return new WaitForSeconds(_changeInputCooldown);
            _isInputInteractable = true;
        }

        public void RemoveCurrentMouseInput()
        {
            _currentMouseInput = null;
            _isInputInteractable = true;
        }
        
    }
}