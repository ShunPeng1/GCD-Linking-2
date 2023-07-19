
using System;
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using UnityEngine;

public class MouseInputManager : BaseCardMouseInput
{
    [SerializeField] private CameraMovement _cameraMovement;
    private IMouseInteractable _mouseInteractable;
    
    protected override void Update()
    {
        UpdateMousePosition();
        CastMouse();

        var button = FindFirstInMouseCast<CharacterCardButton>();
        if (button != null)
        {
            _mouseInteractable = button;
            button.Hover();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            StartDragCard();
        }

        if (Input.GetMouseButton(0))
        {
            CastMouse();
            DragCard();
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastMouse();
            EndDragCard();
        }
    }
}
