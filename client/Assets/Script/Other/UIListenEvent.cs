using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIListenEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{

    public delegate void PointerEnter(PointerEventData eventData);
    public PointerEnter pointerEnter;
    public delegate void PointerExit(PointerEventData eventData);
    public PointerExit pointerExit;
    public delegate void Drag(PointerEventData eventData);
    public Drag drag;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pointerEnter != null)
            pointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (pointerExit != null)
            pointerExit(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (drag != null)
            drag(eventData);
    }
}
