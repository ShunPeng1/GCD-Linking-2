
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using UnityEngine;


[CreateAssetMenu(fileName = "Character Card Information", menuName = "Character Card Information/")]
public abstract class CharacterInfomation : BaseCardInformation
{
    public CharacterCardGameObject CharacterCardGameObjectPrefab;
    public BaseWorldCharacter WorldCharacterPrefab;
    
    public string CharacterName;
    public float Range;
    
    
    public abstract void Init();

    public abstract void Ability1();

    public abstract void Ability2();
}
