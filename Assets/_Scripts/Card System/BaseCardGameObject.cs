
using DG.Tweening;
using UnityEngine;

namespace Shun_Card_System
{
    public class BaseCardGameObject : MonoBehaviour
    {
        public BaseCardInformation CardInformation;
        
        [SerializeField] private Color _selectHighlightColor = new Color(0.15f, 0.15f, 0.15f);

        [SerializeField] private bool _activeValidate = true;

        [SerializeField] private float _spawnDuration = 0.5f;
        [SerializeField] private Ease _spawnEase = Ease.OutCubic;
    
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

    }
}
