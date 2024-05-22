using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class ActionOnClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public class OnMouseDownEvent<UnityEvent> { }
    public UnityEvent onMouseDownEvent;

    public class OnMouseUpEvent<UnityEvent> { }
    public UnityEvent onMouseUpEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        onMouseDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onMouseUpEvent.Invoke();
    }

}
