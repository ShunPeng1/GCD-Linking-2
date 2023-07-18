using UnityEngine;

namespace Shun_Card_System
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardButton : MonoBehaviour
    {
        public bool Interactable = true;
        protected BaseCardGameObject CardGameObject;

        public void Initialize(BaseCardGameObject baseCardGameObject)
        {
            CardGameObject = baseCardGameObject;
        }

        public virtual void Hover()
        {
            
        }
        
        public virtual void Unhover()
        {
            
        }

        public virtual void Execute()
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