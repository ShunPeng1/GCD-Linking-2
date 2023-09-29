using _Scripts.Cards.Card_UI;
using _Scripts.DataWrapper;
using _Scripts.Lights;

public class CharacterSet
{
    public CharacterInformation CharacterInformation;
    public BaseCharacterMapDynamicGameObject CharacterMapGameObject;
    public BaseCharacterCardGameObject CharacterCardGameObject;
    public PortraitButtonRect CharacterPortraitButton;
    public ObservableData<CharacterRecognitionState> CharacterRecognition;

    public CharacterSet(CharacterInformation characterInformation, BaseCharacterMapDynamicGameObject characterMapGameObject, BaseCharacterCardGameObject characterCardGameObject, PortraitButtonRect characterPortraitButton, ObservableData<CharacterRecognitionState> characterRecognition)
    {
        CharacterInformation = characterInformation;
        CharacterMapGameObject = characterMapGameObject;
        CharacterCardGameObject = characterCardGameObject;
        CharacterPortraitButton = characterPortraitButton;
        CharacterRecognition = characterRecognition;
    }
}