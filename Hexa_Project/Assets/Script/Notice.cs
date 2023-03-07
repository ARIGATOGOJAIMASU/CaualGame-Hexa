using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class Notice : MonoBehaviour, ISubject
    {
        List<IObserver> observers;

        public void notifyObserver(string msg, Vector2 boardPosition)
        {
            for(int i = 0; i < observers.Count; ++i)
            {
                observers[i].recive(msg, boardPosition);
            }
        }

        public void registerObserver(IObserver observer)
        {
            if (observers == null)
            {
                observers = new();
            }

            observers.Add(observer);
        }

        public void removeObserver(IObserver observer)
        {
            observers.Remove(observer);
        }
    }
}
