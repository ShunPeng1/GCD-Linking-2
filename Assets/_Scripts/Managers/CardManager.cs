using System.Collections.Generic;
using _Scripts.Cards.Card_UI;
using _Scripts.Managers;
using Shun_Card_System;
using UnityEngine;
using UnityUtilities;

public class CardManager : SingletonMonoBehaviour<CardManager>
{
    
    [Header("Card Region")] 
    [SerializeField] private HandCardRegion _handCardRegion;
    [SerializeField] private PlayCardRegion _playCardRegion;
    [SerializeField] private DeckCardRegion _exhaustCardRegion;
    [SerializeField] private DeckCardRegion _deckCardRegion;

    [Header("Stats")] 
    [SerializeField] private int _startRoundAddCardCount;
    private List<BaseCharacterCardGameObject> _allCards = new();
    private RandomBag<BaseCardGameObject> _deckCardBag;

    public void Initialize()
    {
        var deckCards = _deckCardRegion.GetAllCardGameObjects().ToArray();
        _deckCardBag = new RandomBag<BaseCardGameObject>(deckCards, 1);
    }
    
    public BaseCharacterCardGameObject CreateCharacterMapGameObject(BaseCharacterCardGameObject prefab)
    {
        var deckTransform = _deckCardRegion.transform;
        var card = Instantiate(prefab, deckTransform.position,deckTransform.rotation, deckTransform.parent);
        _deckCardRegion.AddCard(card);
        return card;
    }
    
    
    public void AddFromDeckToHand()
    {

        for (int i = 0; i < _startRoundAddCardCount; i++)
        {
            var card = _deckCardBag.PopRandomItem();
            _deckCardRegion.RemoveCard(card);
            _handCardRegion.AddCard(card);
        }
    }

    public void LockPlayCard()
    {
        _playCardRegion.DisableInteractable();
    }
    
    public void UnlockPlayCard()
    {
        _playCardRegion.EnableInteractable();
    }
    
    public void ExhaustCard(BaseCardGameObject baseCardGameObject)
    {
        _playCardRegion.RemoveCard(baseCardGameObject);
        _exhaustCardRegion.AddCard(baseCardGameObject);
        
        _playCardRegion.EnableInteractable();
        
        if(_handCardRegion.CardHoldingCount == 0) GameManager.Instance.EndRound();
        else GameManager.Instance.EndTurn();
    }
    
    

    public void ShuffleBackToDeck()
    {
        if (_deckCardRegion.CardHoldingCount != 0) return;

        foreach (var card in _exhaustCardRegion.GetAllCardGameObjects())
        {
            _exhaustCardRegion.RemoveCard(card);
            _deckCardRegion.AddCard(card);
        }
    }
    

}
