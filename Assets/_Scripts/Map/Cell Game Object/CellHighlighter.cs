
using System;
using Shun_Card_System;
using UnityEngine;

public class CellHighlighter : MapCellGameObject, IMouseInteractable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _highlightValue = 50f;
    protected void Awake()
    {
        
    }

    public void StartHover()
    {
        _spriteRenderer.color += new Color(0, 0, 0, _highlightValue);
    }

    public void EndHover()
    {
        _spriteRenderer.color -= new Color(0, 0, 0, _highlightValue);
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
        throw new NotImplementedException();
    }

    public void EnableInteractable()
    {
        throw new NotImplementedException();
    }
}
