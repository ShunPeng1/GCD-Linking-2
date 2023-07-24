
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shun_Card_System
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardGameObject : MonoBehaviour, IMouseInteractable
    {
        [SerializeField]
        private bool _interactable;
        public bool Interactable { get => _interactable; protected set => _interactable = value; }
        
        [SerializeField] protected bool ActivateOnValidate = false;
        
        [Header("Scale")]
        [SerializeField] private bool _isScaleWithOriginalParent = true;
        [SerializeField] private Transform _originalParent;
        [SerializeField] private Transform _currentParent;
        
        protected virtual void Awake()
        {
            if (!_isScaleWithOriginalParent) return;
            
            // Store the original parent when the script starts.
            _originalParent = transform.parent;
            _currentParent = _originalParent;
        }

        private void OnValidate()
        {
            if (ActivateOnValidate) ValidateInformation();
        }
        
        protected virtual void ValidateInformation()
        {
            
        }

        

        public virtual void Select()
        {
            
        }

        public virtual void Deselect()
        {
            
        }

        public virtual void StartHover()
        {
            
        }

        public virtual void EndHover()
        {
            
        }
        
        public virtual void DisableInteractable()
        {
            Interactable = false;
        }
        
        public virtual void EnableInteractable()
        {
            Interactable = true;
        }

        public void SetIntoParent(Transform newParent)
        {
            transform.parent = newParent;
            
            if (_isScaleWithOriginalParent) UpdateScaleInto(newParent);
            
        }

        public void SetOutOfParent(Transform newParent)
        {
            transform.parent = newParent;
            if (_isScaleWithOriginalParent) UpdateScaleOutOf(newParent);
            
        }

        private void UpdateScaleInto(Transform upperParent)
        {
            // Calculate the combined scale from the original parent to the current parent.
            Vector3 combinedScale = Vector3.one;
            Transform currentTransform = transform;
            while (currentTransform != upperParent)
            {
                combinedScale = Vector3.Scale(combinedScale, currentTransform.localScale);
                currentTransform = currentTransform.parent;
            }

            // Apply the combined scale to the object.
            transform.localScale = combinedScale;

            // Update parent
            _currentParent = upperParent;
        }

        private void UpdateScaleOutOf(Transform newParent)
        {
            // Calculate the combined scale from the new parent to the original parent.
            Vector3 combinedScale = Vector3.one;
            Transform currentTransform = newParent;
            while (currentTransform != _originalParent)
            {
                combinedScale = Vector3.Scale(combinedScale, currentTransform.localScale);
                currentTransform = currentTransform.parent;
            }

            // Apply the combined scale to the object.
            transform.localScale = Vector3.Scale(transform.localScale, combinedScale);

            // Update parent to the new parent.
            transform.SetParent(newParent, true);
        }
    }
}
