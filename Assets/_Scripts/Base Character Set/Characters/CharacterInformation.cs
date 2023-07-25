
using System;
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;


[CreateAssetMenu(fileName = "Character Information", menuName = "Character Information")]
public class CharacterInformation : BaseCardInformation
{
    [Header("Prefabs")]
    public BaseCharacterCardGameObject BaseCharacterCardGameObjectPrefab;
    public BaseCharacterMapDynamicGameObject CharacterMapDynamicGameObjectPrefab;
    public PortraitButtonRect PortraitButtonRectPrefab;
    
    [Header("Info")]
    public string CharacterName;

    [Header("Grid")]
    public float MaxMoveCellCost = 5;
    public float MoveSpeed = 20;
    public LayerMask WallLayerMask;

    [Header("Ability")] 
    public int Ability1UseCount = 1;
    public int Ability2UseCount = 1;

    public string Ability1Description = "Ability 1 Description";
    public string Ability2Description = "Ability 1 Description";
    
    [Header("Light")] 
    public LightInformation LightInformation;
}
