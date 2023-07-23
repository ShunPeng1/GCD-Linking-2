using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Shun_Card_System
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseCardRegion : MonoBehaviour, IMouseInteractable
    {
        public enum MiddleInsertionStyle
        {
            AlwaysBack,
            InsertInMiddle,
            Cannot,
        }
        [SerializeField] protected BaseCardHolder CardHolderPrefab;
        [SerializeField] protected Transform SpawnPlace;
        [SerializeField] protected Vector3 CardOffset = new Vector3(5f, 0 ,0);

        
        [SerializeField] protected List<BaseCardHolder> _cardPlaceHolders = new();
        [SerializeField] protected int MaxCardHold;
        [SerializeField] protected MiddleInsertionStyle CardMiddleInsertionStyle = MiddleInsertionStyle.InsertInMiddle;
        protected BaseCardHolder TemporaryBaseCardHolder;
        protected int CardHoldingCount = 0;

        
        [SerializeField]
        private bool _interactable;
        public bool Interactable { get => _interactable; protected set => _interactable = value;}
        
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
                    var cardPlaceHolder = Instantiate(CardHolderPrefab, SpawnPlace.position + i * CardOffset,
                        Quaternion.identity, SpawnPlace);
                    _cardPlaceHolders.Add(cardPlaceHolder);
                    cardPlaceHolder.InitializeRegion(this, i);
                }    
            }
            
        }

        public List<BaseCardGameObject> GetAllCardGameObjects(bool getNull = false)
        {
            List<BaseCardGameObject> result = new();
            for (int i = 0; i < CardHoldingCount; i++)
            {
                if ((!getNull && _cardPlaceHolders[i].CardGameObject != null) || getNull) result.Add(_cardPlaceHolders[i].CardGameObject);
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

        protected BaseCardHolder FindEmptyCardPlaceHolder()
        {
            if (CardHoldingCount >= MaxCardHold) return null;
            return _cardPlaceHolders[CardHoldingCount];
        }
        
        public BaseCardHolder FindCardPlaceHolder(BaseCardGameObject baseCardGameObject)
        {
            foreach (var cardPlaceHolder in _cardPlaceHolders)
            {
                if (cardPlaceHolder.CardGameObject == baseCardGameObject) return cardPlaceHolder;
            }

            return null;
        }

        public bool AddCard(BaseCardGameObject cardGameObject, BaseCardHolder cardHolder)
        {
            if (cardHolder == null || cardHolder.IndexInRegion >= CardHoldingCount)
            {
                return AddCardAtBack(cardGameObject);
            }

            return CardMiddleInsertionStyle switch
            {
                MiddleInsertionStyle.AlwaysBack => AddCardAtBack(cardGameObject),
                MiddleInsertionStyle.InsertInMiddle => AddCardAtMiddle(cardGameObject, cardHolder.IndexInRegion),
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
            
            OnSuccessfullyAddCard(cardGameObject, cardPlaceHolder);
                
            return true;
        }
        
        private  bool AddCardAtMiddle(BaseCardGameObject cardGameObject, int index)
        {
            if (CardHoldingCount >= MaxCardHold)
            {
                return false;
            }
            
            ShiftRight(index);

            var cardPlaceHolder = _cardPlaceHolders[index];
            cardPlaceHolder.AttachCardGameObject(cardGameObject);
            
            CardHoldingCount++;
            
            OnSuccessfullyAddCard(cardGameObject, cardPlaceHolder);
            
            return true;
        }
        
        
        protected virtual void ShiftRight(int startIndex)
        {
            for (int i = _cardPlaceHolders.Count - 1; i > startIndex; i--)
            {
                var card = _cardPlaceHolders[i - 1].DetachCardGameObject();
                
                if (card == null) continue;
                _cardPlaceHolders[i].AttachCardGameObject(card);
                
                //SmoothMove(card.transform, _cardPlaceHolders[i].transform.position);

            }
        }
        
        
        protected virtual void ShiftLeft(int startIndex)
        {
            for (int i = startIndex; i < _cardPlaceHolders.Count - 1; i++)
            {
                var card = _cardPlaceHolders[i + 1].DetachCardGameObject();
                
                if (card == null) continue;
                
                _cardPlaceHolders[i].AttachCardGameObject(card);
                
                
                //SmoothMove(card.transform, _cardPlaceHolders[i].transform.position);

            }
        }
        
        public virtual bool RemoveCard(BaseCardGameObject cardGameObject)
        {
            for (int i = 0; i < _cardPlaceHolders.Count; i++)
            {
                if (_cardPlaceHolders[i].CardGameObject != cardGameObject) continue;
                _cardPlaceHolders[i].DetachCardGameObject();
                
                ShiftLeft(i);
                CardHoldingCount--;
                
                OnSuccessfullyRemoveCard(cardGameObject, _cardPlaceHolders[i]);
                return true;
            }
            return false;
        }
        
        public virtual bool RemoveCard(BaseCardGameObject cardGameObject,BaseCardHolder cardHolder)
        {
            if (cardHolder.CardGameObject != cardGameObject) return false;

            cardHolder.DetachCardGameObject();
            
            ShiftLeft(_cardPlaceHolders.IndexOf(cardHolder));
            CardHoldingCount--;

            OnSuccessfullyRemoveCard(cardGameObject, cardHolder);
            return true;
        }
        
        public virtual bool TakeOutTemporary(BaseCardGameObject cardGameObject,BaseCardHolder cardHolder)
        {
            if (!RemoveCard(cardGameObject, cardHolder)) return false;
            
            TemporaryBaseCardHolder = cardHolder;
            return true;
        }
        
        public virtual void ReAddTemporary(BaseCardGameObject baseCardGameObject)
        {
            AddCard(baseCardGameObject, TemporaryBaseCardHolder);
            
            TemporaryBaseCardHolder = null;
        }

        public virtual void RemoveTemporary(BaseCardGameObject baseCardGameObject)
        {
            TemporaryBaseCardHolder = null;
        }

        protected virtual void SmoothMove(Transform movingObject, Vector3 toPosition)
        {
            movingObject.position = toPosition;
        }

        protected virtual void OnSuccessfullyAddCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            
        }
        protected virtual void OnSuccessfullyRemoveCard(BaseCardGameObject baseCardGameObject, BaseCardHolder baseCardHolder)
        {
            
        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public void Deselect()
        {
            throw new NotImplementedException();
        }

        public void StartHover()
        {
            throw new NotImplementedException();
        }

        public void EndHover()
        {
            throw new NotImplementedException();
        }

        public virtual void DisableInteractable()
        {
            Interactable = false;
        }
        
        public virtual void EnableInteractable()
        {
            Interactable = true;
        }
    }
}