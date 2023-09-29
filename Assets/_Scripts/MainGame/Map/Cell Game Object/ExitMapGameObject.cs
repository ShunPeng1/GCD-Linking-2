using System.Collections;
using System.Collections.Generic;
using Shun_Grid_System;
using UnityEngine;

public class ExitMapGameObject : MapStaticGameObject
{

    [SerializeField] private bool _isOpen = true;
    public bool IsOpen { 
        get => _isOpen;
        set => _isOpen = value;
    }
    
    [SerializeField] private Animator _animator;
    
    public void CloseExit()
    {
        _isOpen = false;
        
    }
    
    public void OpenExit()
    {
        _isOpen = true;
        
    }

}
