using _Scripts.Cards.Card_UI;
using UnityEngine;
using UnityUtilities;

public class CardManager : SingletonMonoBehaviour<CardManager>
{
    
    [Header("Card Region")] 
    [SerializeField] private HandCardRegion _handCardRegion;
    [SerializeField] private PlayCardRegion _playCardRegion;
    [SerializeField] private DeckCardRegion _discardCardRegion;
    [SerializeField] private DeckCardRegion _deckCardRegion;
    
    public BaseCharacterCardGameObject CreateCharacterMapGameObject(BaseCharacterCardGameObject prefab)
    {
        var deckTransform = _deckCardRegion.transform;
        var card = Instantiate(prefab, deckTransform.position,deckTransform.rotation, deckTransform.parent);
        _deckCardRegion.AddCard(card);
        return card;
    }


}
