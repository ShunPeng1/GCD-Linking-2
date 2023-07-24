using _Scripts.Cards.Card_UI;

public class CharacterSet
{
    public CharacterInformation CharacterInformation;
    public BaseCharacterMapDynamicGameObject CharacterMapGameObject;
    public BaseCharacterCardGameObject CharacterCardGameObject;

    public CharacterSet(CharacterInformation characterInformation, BaseCharacterMapDynamicGameObject characterMapGameObject, BaseCharacterCardGameObject characterCardGameObject)
    {
        CharacterInformation = characterInformation;
        CharacterMapGameObject = characterMapGameObject;
        CharacterCardGameObject = characterCardGameObject;
    }
}