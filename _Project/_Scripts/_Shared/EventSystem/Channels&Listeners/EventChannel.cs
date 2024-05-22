using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Tracing;
using UnityEngine;

public abstract class EventChannel<T> : ScriptableObject
{
    readonly HashSet<EventListener<T>> observers = new();
    readonly HashSet<IEventListener<T>> interfaceObservers = new();

    public void Invoke(T value)
    {
        Debug.Log("Invoked");
        foreach (EventListener<T> listener in observers)
        {
            listener.Raise(value);
        }
        foreach (IEventListener<T> listener in interfaceObservers)
        {
            listener.Raise(value);
        }

    }

    public void Register(IEventListener<T> listener) => interfaceObservers.Add(listener);
    public void Deregister(IEventListener<T> listener) => interfaceObservers.Remove(listener);


    public void Register(EventListener<T> observer) => observers.Add(observer);
    public void Deregister(EventListener<T> observer) => observers.Remove(observer);
    
}

public readonly struct Empty { }

[CreateAssetMenu(fileName = "NewEmptyEventChannel", menuName = "EventSystem/EventChannel")]
public class EventChannel : EventChannel<Empty> { } 
