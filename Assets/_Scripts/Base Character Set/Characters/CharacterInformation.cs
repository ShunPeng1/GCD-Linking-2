
using System;
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;


[CreateAssetMenu(fileName = "Character Information", menuName = "Character Information")]
public class CharacterInformation : BaseCardInformation
{
    public BaseCharacterCardGameObject BaseCharacterCardGameObjectPrefab;
    public BaseCharacterMapDynamicGameObject CharacterMapDynamicGameObjectPrefab;
    
    
    [Header("Info")]
    public string CharacterName;

    [Header("Grid")]
    public float MaxMoveCellCost = 5;
    public float MoveSpeed = 20;
    public LayerMask WallLayerMask;
    
    
    [Header("Lights")]
    public float LightRange;
    
    
    
}
