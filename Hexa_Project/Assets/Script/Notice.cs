using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class Notice : MonoBehaviour, ISubject
    {
        List<IObserver> observers = new();

        public void notifyObserver(string msg)
        {
            for(int i = 0; i < observers.Count; ++i)
            {
                observers[i].recive(msg);
            }
        }

        public void registerObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void removeObserver(IObserver observer)
        {
            observers.Remove(observer);
        }
    }
}
