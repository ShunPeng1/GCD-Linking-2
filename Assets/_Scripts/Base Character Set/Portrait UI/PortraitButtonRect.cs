using System;
using _Scripts.Input_and_Camera;
using UnityEngine;
using UnityEngine.UI;

public class PortraitButtonRect : MonoBehaviour
{
    private BaseCharacterMapDynamicGameObject _characterMapGameObject;
    public CharacterInformation CharacterInformation { get; protected set; }

    [SerializeField] private Button _button;
    
    
    private void Start()
    {
        _button.onClick.AddListener( ZoomToCharacter );
    }

    public void InitializeCharacter(CharacterInformation characterInformation,BaseCharacterMapDynamicGameObject characterMapDynamicGameObject)
    {
        CharacterInformation = characterInformation;
        _characterMapGameObject = characterMapDynamicGameObject;
    }

    private void ZoomToCharacter()
    {
        InputManager.Instance.CameraMovement.FocusOnGameObject(_characterMapGameObject.gameObject);
        
    }
    
}
