
using System;
using Shun_Card_System;
using UnityEngine;

public class CellSelectHighlighter : MapStaticGameObject, IMouseInteractable
{
    
    [SerializeField]
    private bool _interactable = false;
    public bool Interactable { get => _interactable; protected set => _interactable = value; }
    public bool IsHovering { get; protected set; }

    [Header("Highlight")]
    [SerializeField] private float _validCellHighlightValue = 0.25f, _hoverHighlightValue = 0.25f;
    [SerializeField] private Color _chooseObjectColor = new Color(1,1,1,0);
    
    [Header("Components")]    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;


    private void OnValidate()
    {
        _chooseObjectColor.a = 0;
    }

    public void StartHover()
    {
        IsHovering = true;
       
        IncreaseTransparency(_hoverHighlightValue);

    }

    public void EndHover()
    {
        IsHovering = false;
        
        IncreaseTransparency(-_hoverHighlightValue);
    }

    public void Select()
    {
        throw new NotImplementedException();
    }

    public void Deselect()
    {
        throw new NotImplementedException();
    }

    public void DisableInteractable()
    {
        if (!Interactable) return;
        Interactable = false;
        if (IsHovering) EndHover();

        _animator.enabled = false;
        
        IncreaseTransparency(-_validCellHighlightValue);
    }

    public void EnableInteractable()
    {
        if (Interactable) return;
        Interactable = true;
        _animator.enabled = true;
        
        _animator.Rebind();

        IncreaseTransparency(_validCellHighlightValue);
        
    }

    private void IncreaseTransparency(float value)
    {
        _spriteRenderer.color = HighlightColorTransparent() + new Color(0, 0, 0,  _spriteRenderer.color.a + value);

    }
    
    private Color HighlightColorTransparent()
    {
        MapCellItem item = Cell.Item;

        if (item.GetFirstInCellGameObject<MapDynamicGameObject>() != null || item.GetFirstInCellGameObject<ExitMapGameObject>() != null)
        {
            return _chooseObjectColor;
        }
        else
        {
            return new Color(1, 1, 1, 0);
        }
    }
}
