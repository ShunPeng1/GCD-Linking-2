
using System;
using _Scripts.Cards.Card_UI;
using _Scripts.Managers;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;

public class MouseInputManager : BaseCardMouseInput
{
    [SerializeField] private CameraMovement _cameraMovement;
    private GridXY<MapCellItem> _grid;

    protected void Start()
    {
        _grid = MapManager.Instance.WorldGrid;
    }

    protected void InitializeGameStateFunctions()
    {
        
    }
    
    protected override void Update()
    {
        UpdateMousePosition();
        if(!IsDraggingCard) UpdateHoverObject();
        
        if (Input.GetMouseButtonDown(0))
        {
            CastMouse();
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
    
    /*
    protected override IMouseInteractable GetMouseInteractableInCastMouse()
    {
        foreach (var hit in MouseCastHits)
        {
            var characterCardButton = hit.transform.gameObject.GetComponent<BaseCardButton>();
            if (characterCardButton != null)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return characterCardButton;
            }

            var characterCardGameObject = hit.transform.gameObject.GetComponent<BaseCardGameObject>();
            if (characterCardGameObject != null)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return characterCardGameObject;
            }
            
            var cellHighlighter = hit.transform.gameObject.GetComponent<CellHighlighter>();
            if (cellHighlighter != null)
            {
                //Debug.Log("Mouse find "+ gameObject.name);
                return cellHighlighter;
            }
        }

        return null;
    }

    */
}
