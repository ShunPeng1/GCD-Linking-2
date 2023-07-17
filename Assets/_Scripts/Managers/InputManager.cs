using System.Collections;
using System.Collections.Generic;
using Shun_Card_System;
using UnityEngine;
using UnityUtilities;

public class InputManager : MonoBehaviour
    {
        
        private Vector3 _mousePosition;
        private int _currentColor = 0;
        
        [Header("Drag Objects")]
        private bool _isDragging = false;
        private Vector3 _cardOffset;
        private Vector3 _cardBeforeDragPosition;
        private BaseCardGameObject _draggingCard;
        private BaseCardPlaceRegion _lastCardPlaceRegion;
        private BaseCardPlaceHolder _lastCardPlaceHolder;
        
        [Header("Spin Object")] 
        [SerializeField] private float _spinDuration = 0.25f;
        private bool _isSpinning = false;
        
        private void Update()
        {
            UpdateMousePosition();
            
            if (Input.GetMouseButtonDown(0))
            {
               StartDragMouse();
            }

            if (Input.GetMouseButton(0))
            {
                DragMouse();
                
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndDragMouse();
            }
        }

        private void UpdateMousePosition()
        {
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePosition = new Vector3(worldMousePosition.x, worldMousePosition.y, 0);
        }

        private TResult CastMouseFindFirst<TResult>()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(_mousePosition, Vector2.zero);

            foreach (var hit in hits)
            {
                var result = hit.transform.gameObject.GetComponent<TResult>();
                if (result != null)
                {
                    //Debug.Log("Mouse find "+ gameObject.name);
                    return result;
                }
            }

            //Debug.Log("Mouse cannot find "+ typeof(TResult));
            return default;
        }

        private void StartDragMouse()
        {
            _draggingCard = CastMouseFindFirst<BaseCardGameObject>();
            _lastCardPlaceHolder = CastMouseFindFirst<BaseCardPlaceHolder>();
            _lastCardPlaceRegion = CastMouseFindFirst<BaseCardPlaceRegion>();

            if (_draggingCard == null) return;
            
            _cardOffset = _draggingCard.transform.position - _mousePosition;
            _isDragging = true;
            _cardBeforeDragPosition = _draggingCard.transform.position;

            _draggingCard.Select();
            
            if (_lastCardPlaceHolder == null)
            {
                _lastCardPlaceRegion = null;
                return;
            }
            if (!_lastCardPlaceRegion.TakeOutTemporary(_draggingCard, _lastCardPlaceHolder))
            {
                _lastCardPlaceHolder = null;
                _lastCardPlaceRegion = null;
            }

        }

        private void DragMouse()
        {
            if (!_isDragging) return; 
            
            _draggingCard.transform.position = _mousePosition + _cardOffset;
            
        }

        private void EndDragMouse()
        {
            if (!_isDragging) return;
            
            _draggingCard.Deselect();
            AddCardToHolder();

            _draggingCard = null;
            _lastCardPlaceHolder = null;
            _lastCardPlaceRegion = null;
            _isDragging = false;

        }

        private void AddCardToHolder()
        {
            
            var placeRegion = CastMouseFindFirst<BaseCardPlaceRegion>();
            var placeHolder = CastMouseFindFirst<BaseCardPlaceHolder>();
            
            if (placeHolder == null)
            {
                if (placeRegion != null && placeRegion != _lastCardPlaceRegion && placeRegion.AddCard(_draggingCard, placeHolder))
                {
                    if (_lastCardPlaceHolder != null)
                    {
                        _lastCardPlaceRegion.RemoveTemporary(_draggingCard);
                        return;
                    }
                }
                
                if (_lastCardPlaceRegion != null) _lastCardPlaceRegion.ReAddTemporary(_draggingCard);
            }
            else
            {
                if (!placeRegion.AddCard(_draggingCard, placeHolder))
                {
                    if(_lastCardPlaceRegion != null) _lastCardPlaceRegion.ReAddTemporary(_draggingCard);
                }
                
                if (_lastCardPlaceHolder != null)
                {
                    _lastCardPlaceRegion.RemoveTemporary(_draggingCard);
                }

            }

        }
        
    }
