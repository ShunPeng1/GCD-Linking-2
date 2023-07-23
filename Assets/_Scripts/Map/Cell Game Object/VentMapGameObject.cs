using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class VentMapGameObject : MapStaticGameObject
{

    [SerializeField] private bool _isOpen = true;
    public bool IsOpen { 
        get => _isOpen;
        set => _isOpen = value;
    }
    
    [SerializeField] private Animator _animator;

    public void CloseVent()
    {
        _isOpen = false;
        
    }
    
    public void OpenVent()
    {
        _isOpen = true;
        
    }

}
