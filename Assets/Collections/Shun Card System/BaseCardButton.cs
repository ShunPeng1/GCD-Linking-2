using UnityEngine;

namespace Shun_Card_System
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardButton : MonoBehaviour, IMouseInteractable
    {
        
        [SerializeField]
        private bool _interactable;
        public bool Interactable { get => _interactable; protected set => _interactable = value; }

        
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