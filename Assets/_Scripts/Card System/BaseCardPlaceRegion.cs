using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Shun_Card_System
{
    public class BaseCardPlaceRegion : MonoBehaviour
    {
        public enum InsertionStyle
        {
            Scatter,
            StackToFront
        }
        
        [SerializeField] protected BaseCardPlaceHolder CardPlaceHolderPrefab;
        [SerializeField] protected Transform SpawnPlace;
        [SerializeField] protected Vector3 CardOffset = new Vector3(5f, 0 ,0);

        
        [SerializeField] protected List<BaseCardPlaceHolder> _cardPlaceHolders = new();
        [SerializeField] protected int MaxCardHold;
        [SerializeField] protected InsertionStyle CardInsertionStyle;
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
                return;
            }
            
            for (int i = 0; i < MaxCardHold; i++)
            {
                _cardPlaceHolders.Add( Instantiate(CardPlaceHolderPrefab, SpawnPlace.position + i * CardOffset, Quaternion.identity, SpawnPlace));
                
            }
        }

        public List<BaseCardInformation> GetAllCardInformation()
        {
            List<BaseCardInformation> result = new();
            for (int i = 0; i < CardHoldingCount; i++)
            {
                result.Add(_cardPlaceHolders[i].BaseCardGameObject.CardInformation);
            }

            return result;
        }

        public void DestroyAllCardGameObject()
        {
            for (int i = 0; i < CardHoldingCount; i++)
            {
                Destroy(_cardPlaceHolders[i].BaseCardGameObject.gameObject);
                _cardPlaceHolders[i].BaseCardGameObject = null;
                
            }

            CardHoldingCount = 0;
        }

        public BaseCardPlaceHolder FindEmptyCardPlaceHolder()
        {
            if (CardHoldingCount >= MaxCardHold) return null;
            return _cardPlaceHolders[CardHoldingCount];
        }

        public bool AddCard(BaseCardGameObject cardGameObject, BaseCardPlaceHolder baseCardPlaceHolder)
        {
            if (CardHoldingCount >= MaxCardHold)
            {
                return false;
            }

            int index = 0;
            if (baseCardPlaceHolder == null)
            {
                baseCardPlaceHolder = _cardPlaceHolders[CardHoldingCount];
                index = CardHoldingCount ;
            }
            else
            {
                index = _cardPlaceHolders.IndexOf(baseCardPlaceHolder);
            }
            
            if (index >= CardHoldingCount)
            {
                index = CardHoldingCount ;
                
                _cardPlaceHolders[index].BaseCardGameObject = cardGameObject;
                cardGameObject.transform.parent = transform;
                //card.transform.position = _cardPlaceHolders[index].transform.position;
                SmoothMove(cardGameObject.transform, _cardPlaceHolders[index].transform.position);
                
                CardHoldingCount ++;
                return true;
            }
            
            if(CardInsertionStyle == InsertionStyle.StackToFront) ShiftRight(index);
            
            _cardPlaceHolders[index].BaseCardGameObject = cardGameObject;
            cardGameObject.transform.parent = transform;
            //card.transform.position = _cardPlaceHolders[index].transform.position;
            SmoothMove(cardGameObject.transform, _cardPlaceHolders[index].transform.position);

            CardHoldingCount++;
            return true;
        }

        protected void ShiftRight(int startIndex)
        {
            for (int i = _cardPlaceHolders.Count - 1; i > startIndex; i--)
            {
                var card = _cardPlaceHolders[i-1].BaseCardGameObject;
                _cardPlaceHolders[i].BaseCardGameObject = card;
                
                if (card == null) continue;
                //card.transform.position = _cardPlaceHolders[i].transform.position;
                SmoothMove(card.transform, _cardPlaceHolders[i].transform.position);

            }
        }
        
        
        protected void ShiftLeft(int startIndex)
        {
            for (int i = startIndex; i < _cardPlaceHolders.Count - 1; i++)
            {
                var card = _cardPlaceHolders[i+1].BaseCardGameObject;
                _cardPlaceHolders[i].BaseCardGameObject = card;

                if (card == null) continue;
                //card.transform.position = _cardPlaceHolders[i].transform.position;
                SmoothMove(card.transform, _cardPlaceHolders[i].transform.position);

            }
            
            _cardPlaceHolders[^1].BaseCardGameObject = null;
        }
        
        public bool RemoveCard(BaseCardGameObject cardGameObject)
        {
            for (int i = 0; i < _cardPlaceHolders.Count; i++)
            {
                if (_cardPlaceHolders[i].BaseCardGameObject == cardGameObject)
                {
                    _cardPlaceHolders[i].BaseCardGameObject.transform.parent = null;
                    _cardPlaceHolders[i].BaseCardGameObject = null;
                    ShiftLeft(i);
                    
                    CardHoldingCount--;

                    return true;
                }
            }
            return false;
        }
        
        public bool RemoveCard(BaseCardGameObject cardGameObject,BaseCardPlaceHolder baseCardPlaceHolder)
        {
            if (baseCardPlaceHolder.BaseCardGameObject != cardGameObject) return false;

            baseCardPlaceHolder.BaseCardGameObject.transform.parent = null;
            baseCardPlaceHolder.BaseCardGameObject = null;
            
            if(CardInsertionStyle == InsertionStyle.StackToFront) ShiftLeft(_cardPlaceHolders.IndexOf(baseCardPlaceHolder));
            CardHoldingCount--;

            return true;
        }
        
        public bool TakeOutTemporary(BaseCardGameObject cardGameObject,BaseCardPlaceHolder baseCardPlaceHolder)
        {
            if (RemoveCard(cardGameObject, baseCardPlaceHolder))
            {
                
                TemporaryBaseCardHolder = baseCardPlaceHolder;
                return true;
            }
            return false;
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
    }
}