using System;
using _Scripts.Input_and_Camera;
using _Scripts.Lights;
using UnityEngine;
using UnityEngine.UI;

public class PortraitButtonRect : MonoBehaviour
{
    public CharacterSet CharacterSet { get; protected set; }

    [SerializeField] private Button _button;
    [SerializeField] private Animator _animator;
    private static readonly int IsInDark = Animator.StringToHash("IsInDark");
    private static readonly int IsInnocent = Animator.StringToHash("IsInnocent");

    private void Start()
    {
        _button.onClick.AddListener( ZoomToCharacter );
        CharacterSet.CharacterRecognition.OnChangeValue += VisualizeCharacterRecognition;
    }

    public void InitializeCharacter(CharacterSet characterSet)
    {
        CharacterSet = characterSet;
        
    }

    private void VisualizeCharacterRecognition(CharacterRecognitionState oldRecognitionState, CharacterRecognitionState currentRecognitionState)
    {
        if (oldRecognitionState == currentRecognitionState) return;
        
        Debug.Log("Update recognition of " + CharacterSet.CharacterMapGameObject.name + " to " + currentRecognitionState.ToString());
        
        switch (currentRecognitionState)
        {
            case CharacterRecognitionState.InLight:
                _animator.SetBool(IsInDark, false);
                break;
            case CharacterRecognitionState.InDark:
                _animator.SetBool(IsInDark, true);
                break;
            case CharacterRecognitionState.Innocent:
                _animator.SetTrigger(IsInnocent);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentRecognitionState), currentRecognitionState, null);
        }
    }

    private void ZoomToCharacter()
    {
        InputManager.Instance.CameraMovement.FocusOnGameObject(CharacterSet.CharacterMapGameObject.gameObject);
        
    }
    
}
