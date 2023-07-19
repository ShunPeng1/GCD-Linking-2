
using System;

using UnityEngine;

public class CellHighlighter : MapCellGameObject
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _highlightValue = 50f;
    protected void Awake()
    {
        
    }

    public void Hover()
    {
        _spriteRenderer.color += new Color(0, 0, 0, _highlightValue);
    }

    public void Unhover()
    {
        _spriteRenderer.color -= new Color(0, 0, 0, _highlightValue);
    }
}
