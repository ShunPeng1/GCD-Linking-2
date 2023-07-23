
using System;
using Shun_Card_System;
using UnityEngine;

public class CellSelectHighlighter : MapStaticGameObject, IMouseInteractable
{
    
    [SerializeField]
    private bool _interactable = false;
    public bool Interactable { get => _interactable; protected set => _interactable = value; }
    
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _validCellHighlightValue = 0.25f, _hoverHighlightValue = 0.25f;
    protected void Awake()
    {
        
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
        _spriteRenderer.color += new Color(0, 0, 0, _hoverHighlightValue);
    }

    public void EndHover()
    {
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
    }

    public void EnableInteractable()
    {
        Interactable = true;
    }
}
