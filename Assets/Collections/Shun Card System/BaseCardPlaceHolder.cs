using UnityEngine;

namespace Shun_Card_System
{
    /// <summary>
    /// This class is the card place holder of a card object in card place region.
    /// This can be used to move, animations,...
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardPlaceHolder : MonoBehaviour
    {
        [HideInInspector] public BaseCardPlaceRegion CardPlaceRegion;
        [HideInInspector] public int IndexInRegion;
        public BaseCardGameObject CardGameObject;

        public void InitializeRegion(BaseCardPlaceRegion cardPlaceRegion, int indexInRegion)
        {
            CardPlaceRegion = cardPlaceRegion;
            IndexInRegion = indexInRegion;
            transform.parent = cardPlaceRegion.transform;
        }
        
        public virtual void AttachCardGameObject(BaseCardGameObject cardGameObject)
        {
            if (cardGameObject == null) return;
            
            CardGameObject = cardGameObject;
            cardGameObject.transform.SetParent(transform);
            AttachCardVisual();
        }

        public virtual BaseCardGameObject DetachCardGameObject()
        {
            if (CardGameObject == null) return null;
            
            BaseCardGameObject detachedCard = CardGameObject;
            detachedCard.transform.SetParent(null);
            CardGameObject = null;

            return detachedCard;
        }

        protected virtual void AttachCardVisual()
        {
            CardGameObject.transform.localPosition = Vector3.zero;
        }
    }
}