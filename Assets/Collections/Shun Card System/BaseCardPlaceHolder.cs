using UnityEngine;

namespace Shun_Card_System
{
    public class BaseCardPlaceHolder : MonoBehaviour
    {
        public BaseCardGameObject BaseCardGameObject;

        public virtual void AttachCardGameObject(BaseCardGameObject baseCardGameObject)
        {
            BaseCardGameObject = baseCardGameObject;
        }

        public virtual void DetachCardGameObject()
        {
            BaseCardGameObject = null;
        }
    }
}