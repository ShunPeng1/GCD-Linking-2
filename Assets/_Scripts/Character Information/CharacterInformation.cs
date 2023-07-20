
using System;
using _Scripts.Cards.Card_UI;
using Shun_Card_System;
using Shun_Grid_System;
using UnityEngine;
using UnityUtilities;


//[CreateAssetMenu(fileName = "Character Information", menuName = "Character Information/")]
public abstract class CharacterInformation : BaseCardInformation
{
    public CharacterCardGameObject CharacterCardGameObjectPrefab;
    public BaseWorldCharacter WorldCharacterPrefab;
    protected BaseWorldCharacter CurrentWorldCharacter;
    
    [Header("Info")]
    public string CharacterName;

    [Header("Grid")]
    public float MoveCellCost;
    public float MoveSpeed;
    public LayerMask WallLayerMask;
    public TilemapAdjacencyCellSelection AdjacentAdjacencyCellSelection;
    public IPathfindingAlgorithm<GridXY<MapCellItem>, GridXYCell<MapCellItem>, MapCellItem> PathfindingAlgorithm;
    
    [Header("Lights")]
    public float LightRange;

    

    public virtual void MoveAbility()
    {
        
    }

    public abstract void Ability2();
    
    
    
}
