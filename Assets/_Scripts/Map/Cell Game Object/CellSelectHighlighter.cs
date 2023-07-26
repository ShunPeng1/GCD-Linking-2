
using System;
using Shun_Card_System;
using UnityEngine;

public class CellSelectHighlighter : MapStaticGameObject, IMouseInteractable
{
    
    [SerializeField]
    private bool _interactable = false;
    public bool Interactable { get => _interactable; protected set => _interactable = value; }
    public bool IsHovering { get; protected set; }

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _validCellHighlightValue = 0.25f, _hoverHighlightValue = 0.25f;
    [SerializeField] private Animator _animator;
    protected void Awake()
    {
        DisableInteractable();
    }

    public void StartHighlight()
    {
        _spriteRenderer.color += new Color(0, 0, 0, _validCellHighlightValue);
    }

    public void EndHighlight()
    {
        _spriteRenderer.color -= new Color(0, 0, 0, _validCellHighlightValue);
    }

    public void StartHover()
    {
        IsHovering = true;
        _spriteRenderer.color += new Color(0, 0, 0, _hoverHighlightValue);
    }

    public void EndHover()
    {
        IsHovering = false;
        _spriteRenderer.color -= new Color(0, 0, 0, _hoverHighlightValue);
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
        Interactable = false;
        if (IsHovering) EndHover();

        _animator.enabled = false;
    }

    public void EnableInteractable()
    {
        Interactable = true;
        _animator.enabled = true;
    }
}
