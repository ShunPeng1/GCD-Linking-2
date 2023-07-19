namespace Shun_Card_System
{
    public interface IMouseInteractable
    {
        public void Select();

        public void Deselect();

        public void Hover();

        public void Unhover();

        public void DisableInteractable();

        public void EnableInteractable();
    }
}