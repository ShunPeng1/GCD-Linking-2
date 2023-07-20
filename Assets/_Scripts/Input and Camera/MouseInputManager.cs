
using System;
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using UnityEngine;

public class MouseInputManager : BaseCardMouseInput
{
    [SerializeField] private CameraMovement _cameraMovement;
    
    protected override void Update()
    {
        UpdateMousePosition();
        
        
        
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
