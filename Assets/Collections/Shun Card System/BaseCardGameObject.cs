
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shun_Card_System
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardGameObject : MonoBehaviour, IMouseInteractable
    {
        public bool Interactable = true;
        [SerializeField] protected bool ActivateOnValidate = false;
        
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
    }
}
