using UnityEngine;
using UnityEngine.Events;

public interface IEventListener<T>
{
    public void Raise(T value);
}

public abstract class EventListener<T> : MonoBehaviour
{
    [SerializeField] EventChannel<T> eventChannel;
    [SerializeField] UnityEvent<T> unityEvent;

    public void Raise(T value) => unityEvent?.Invoke(value);

    protected void OnEnable() => eventChannel.Register(this);
    protected void OnDestroy() => eventChannel.Deregister(this);
}


