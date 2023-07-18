
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shun_Card_System
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardGameObject : MonoBehaviour
    {
        public bool Interactable = true;
        public BaseCardInformation CardInformation;
        
        private void OnValidate()
        {
            if(CardInformation == null) return;
            InitializeInformation();
        }
        
        protected virtual void InitializeInformation()
        {
            
        }

        public virtual void Select()
        {
            
        }

        public virtual void Deselect()
        {
            
        }

        public virtual void Hover()
        {
            
        }

        public virtual void Unhover()
        {
            
        }
        
        public virtual void DisableInteraction()
        {
            Interactable = false;
        }
        
        public virtual void EnableInteraction()
        {
            Interactable = true;
        }
    }
}
