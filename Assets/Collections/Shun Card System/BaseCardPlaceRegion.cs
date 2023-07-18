using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Shun_Card_System
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardPlaceRegion : MonoBehaviour
    {
        public enum MiddleInsertionStyle
        {
            AlwaysBack,
            InsertInMiddle,
            Cannot,
        }
        
        [SerializeField] protected BaseCardPlaceHolder CardPlaceHolderPrefab;
        [SerializeField] protected Transform SpawnPlace;
        [SerializeField] protected Vector3 CardOffset = new Vector3(5f, 0 ,0);

        
        [SerializeField] protected List<BaseCardPlaceHolder> _cardPlaceHolders = new();
        [SerializeField] protected int MaxCardHold;
        [SerializeField] protected MiddleInsertionStyle CardMiddleInsertionStyle = MiddleInsertionStyle.InsertInMiddle;
        protected BaseCardPlaceHolder TemporaryBaseCardHolder;
        protected int CardHoldingCount = 0;
        
        protected virtual void Awake()
        {
            InitializeCardPlaceHolder();
        }

        protected void InitializeCardPlaceHolder()
        {
            if (_cardPlaceHolders.Count != 0)
            {
                MaxCardHold = _cardPlaceHolders.Count;
                for (int i = 0; i < MaxCardHold; i++)
                {
                    _cardPlaceHolders[i].InitializeRegion(this, i);
                }
            }
            else
            {
                for (int i = 0; i < MaxCardHold; i++)
                {
                    var cardPlaceHolder = Instantiate(CardPlaceHolderPrefab, SpawnPlace.position + i * CardOffset,
                        Quaternion.identity, SpawnPlace);
                    _cardPlaceHolders.Add(cardPlaceHolder);
                    cardPlaceHolder.InitializeRegion(this, i);
                }    
            }
            
        }

        public List<BaseCardInformation> GetAllCardInformation(bool getNull = false)
        {
            List<BaseCardInformation> result = new();
            for (int i = 0; i < CardHoldingCount; i++)
            {
                result.Add(_cardPlaceHolders[i].CardGameObject.CardInformation);
            }

            return result;
        }

        public void DestroyAllCardGameObject()
        {
            foreach (var cardHolder in _cardPlaceHolders)
            {
                if (cardHolder.CardGameObject == null) continue;
                Destroy(cardHolder.CardGameObject.gameObject);
                cardHolder.CardGameObject = null;
            }
            
            CardHoldingCount = 0;
        }

        public BaseCardPlaceHolder FindEmptyCardPlaceHolder()
        {
            if (CardHoldingCount >= MaxCardHold) return null;
            return _cardPlaceHolders[CardHoldingCount];
        }

        public bool AddCard(BaseCardGameObject cardGameObject, BaseCardPlaceHolder cardPlaceHolder)
        {
            if (cardPlaceHolder == null || cardPlaceHolder.IndexInRegion >= CardHoldingCount)
            {
                return AddCardAtBack(cardGameObject);
            }

            return CardMiddleInsertionStyle switch
            {
                MiddleInsertionStyle.AlwaysBack => AddCardAtBack(cardGameObject),
                MiddleInsertionStyle.InsertInMiddle => AddCardAtMiddle(cardGameObject, cardPlaceHolder.IndexInRegion),
                MiddleInsertionStyle.Cannot => false,
                _ => false
            };
        }

        private bool AddCardAtBack(BaseCardGameObject cardGameObject)
        {
            if (CardHoldingCount >= MaxCardHold)
            {
                return false;
            }
            
            var cardPlaceHolder = _cardPlaceHolders[CardHoldingCount];
            cardPlaceHolder.AttachCardGameObject(cardGameObject);
            
            CardHoldingCount ++;
            UpdateCardPlaceHolderTransforms();
                
            return true;
        }
        
        private bool AddCardAtMiddle(BaseCardGameObject cardGameObject, int index)
        {
            if (CardHoldingCount >= MaxCardHold)
            {
                return false;
            }
            
            ShiftRight(index);

            var cardPlaceHolder = _cardPlaceHolders[index];
            cardPlaceHolder.AttachCardGameObject(cardGameObject);
            
            UpdateCardPlaceHolderTransforms();
            CardHoldingCount++;
            return true;
        }
        
        
        protected void ShiftRight(int startIndex)
        {
            for (int i = _cardPlaceHolders.Count - 1; i > startIndex; i--)
            {
                var card = _cardPlaceHolders[i - 1].DetachCardGameObject();
                
                if (card == null) continue;
                _cardPlaceHolders[i].AttachCardGameObject(card);
                
                //SmoothMove(card.transform, _cardPlaceHolders[i].transform.position);

            }
        }
        
        
        protected void ShiftLeft(int startIndex)
        {
            for (int i = startIndex; i < _cardPlaceHolders.Count - 1; i++)
            {
                var card = _cardPlaceHolders[i + 1].DetachCardGameObject();
                
                if (card == null) continue;
                
                _cardPlaceHolders[i].AttachCardGameObject(card);
                
                //SmoothMove(card.transform, _cardPlaceHolders[i].transform.position);

            }
        }
        
        public bool DetachCard(BaseCardGameObject cardGameObject)
        {
            for (int i = 0; i < _cardPlaceHolders.Count; i++)
            {
                if (_cardPlaceHolders[i].CardGameObject != cardGameObject) continue;
                
                _cardPlaceHolders[i].DetachCardGameObject();
                CardHoldingCount--;
                
                ShiftLeft(i);
                
                return true;
            }
            return false;
        }
        
        public bool DetachCard(BaseCardGameObject cardGameObject,BaseCardPlaceHolder cardPlaceHolder)
        {
            if (cardPlaceHolder.CardGameObject != cardGameObject) return false;

            cardPlaceHolder.DetachCardGameObject();
            
            ShiftLeft(_cardPlaceHolders.IndexOf(cardPlaceHolder));
            CardHoldingCount--;

            return true;
        }
        
        public bool TakeOutTemporary(BaseCardGameObject cardGameObject,BaseCardPlaceHolder cardPlaceHolder)
        {
            if (!DetachCard(cardGameObject, cardPlaceHolder)) return false;
            
            TemporaryBaseCardHolder = cardPlaceHolder;
            return true;
        }
        
        public void ReAddTemporary(BaseCardGameObject baseCardGameObject)
        {
            AddCard(baseCardGameObject, TemporaryBaseCardHolder);
            
            TemporaryBaseCardHolder = null;
        }

        public void RemoveTemporary(BaseCardGameObject baseCardGameObject)
        {
            TemporaryBaseCardHolder = null;
        }

        protected virtual void SmoothMove(Transform movingObject, Vector3 toPosition)
        {
            movingObject.position = toPosition;
        }

        protected virtual void UpdateCardPlaceHolderTransforms()
        {
            
        }
    }
}