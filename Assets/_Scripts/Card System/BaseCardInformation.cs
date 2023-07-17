using System.Collections.Generic;
using UnityEngine;

namespace Shun_Card_System
{
    
    public enum WordCardType
    {
        Noun,
        VerbPrefixUnary,
        VerbPostfixUnary,
        VerbBinary,
        VerbTernary,
        Value,
        Condition,
    }
    
    [CreateAssetMenu(fileName = "Card Information")]
    public abstract class BaseCardInformation : ScriptableObject
    {
        public WordCardType WordCardType;
        public int Priority;
        public string Name;
        public int Cost;
        public int ParametersCount;
        public int ResultCount;
        
        private List<object> _objectList = new();

        

        public void AddObject(object obj)
        {
            _objectList.Add(obj);
        }
        
        public void AddObjects(object [] array)
        {
            foreach (var obj in array)
            {
                _objectList.Add(array);
            }
        }

        public T GetObjectByType<T>()
        {
            foreach (object obj in _objectList)
            {
                if (obj is T)
                {
                    return (T)obj;
                }
            }

            return default(T);
        }

        public void RemoveObject(object obj)
        {
            _objectList.Remove(obj);
        }


        public abstract object[] Execute(object[] array);

    }
}