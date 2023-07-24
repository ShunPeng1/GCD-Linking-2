using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class VentMapGameObject : MapStaticGameObject
{

    [SerializeField] private bool _isUnlock = true;
    public bool IsUnlock { 
        get => _isUnlock;
        set => _isUnlock = value;
    }
    
    [SerializeField] private Animator _animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    public void LockVent()
    {
        _isUnlock = false;
    }
    
    public void UnlockVent()
    {
        _isUnlock = true;
    }

    public void UseVent(bool isOpen)
    {
        _animator.SetBool(IsOpen, isOpen);
    }

}
