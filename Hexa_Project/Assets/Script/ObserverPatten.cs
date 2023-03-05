using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public interface ISubject
    {
        void registerObserver(IObserver observer);
        void removeObserver(IObserver observer);
        void notifyObserver(string msg, BoardPosition boardPosition);
    }

    public interface IObserver
    {
        void recive(string msg, BoardPosition boardPosition);
    }
}
