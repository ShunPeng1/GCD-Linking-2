using System;
using _Scripts.Input_and_Camera;
using _Scripts.Lights;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PortraitButtonRect : MonoBehaviour
{
    public CharacterSet CharacterSet { get; protected set; }

    [FormerlySerializedAs("_button")] public Button Button;
    [SerializeField] private Animator _animator;
    private static readonly int IsInDark = Animator.StringToHash("IsInDark");
    private static readonly int IsInnocent = Animator.StringToHash("IsInnocent");
    private static readonly int IsImposterSelect = Animator.StringToHash("IsImposterSelect");

    

    [Header("Audio")] 
    [SerializeField] private AudioClip _innocentStampSfx;

    [SerializeField] private AudioClip _lightFlickSfx;
    private void Start()
    {
        Button.onClick.AddListener( ZoomToCharacter );
        CharacterSet.CharacterRecognition.OnChangeValue += VisualizeCharacterRecognition;
    }

    public void InitializeCharacter(CharacterSet characterSet)
    {
        CharacterSet = characterSet;
        
    }

    public void SetImposterSelect(bool isSelect)
    {
        _animator.SetBool(IsImposterSelect, isSelect);
    }    
    
    private void VisualizeCharacterRecognition(CharacterRecognitionState oldRecognitionState, CharacterRecognitionState currentRecognitionState)
    {
        if (oldRecognitionState == currentRecognitionState) return;
        
        //Debug.Log("Update recognition of " + CharacterSet.CharacterMapGameObject.name + " to " + currentRecognitionState.ToString());
        
        switch (currentRecognitionState)
        {
            case CharacterRecognitionState.InLight:
                _animator.SetBool(IsInDark, false);
                AudioManager.Instance.PlaySFX(_lightFlickSfx);
                break;
            case CharacterRecognitionState.InDark:
                _animator.SetBool(IsInDark, true);
                AudioManager.Instance.PlaySFX(_lightFlickSfx);
                break;
            case CharacterRecognitionState.Innocent:
                _animator.SetTrigger(IsInnocent);
                AudioManager.Instance.PlaySFX(_innocentStampSfx);
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
