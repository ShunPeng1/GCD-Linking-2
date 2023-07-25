using _Scripts.Cards.Card_UI;

public class CharacterSet
{
    public CharacterInformation CharacterInformation;
    public BaseCharacterMapDynamicGameObject CharacterMapGameObject;
    public BaseCharacterCardGameObject CharacterCardGameObject;
    public PortraitButtonRect CharacterPortraitButton;
    
    public CharacterSet(CharacterInformation characterInformation, BaseCharacterMapDynamicGameObject characterMapGameObject, BaseCharacterCardGameObject characterCardGameObject, PortraitButtonRect characterPortraitButton)
    {
        CharacterInformation = characterInformation;
        CharacterMapGameObject = characterMapGameObject;
        CharacterCardGameObject = characterCardGameObject;
        CharacterPortraitButton = characterPortraitButton;
    }
}