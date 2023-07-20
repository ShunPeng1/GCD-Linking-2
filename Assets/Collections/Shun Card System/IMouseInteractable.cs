﻿namespace Shun_Card_System
{
    public interface IMouseInteractable
    {
        public void Select();

        public void Deselect();

        public void StartHover();

        public void EndHover();

        public void DisableInteractable();

        public void EnableInteractable();
    }
}