using System;
using UnityEngine;
using UnityEngine.EventSystems;

// This component provides push-to-talk functionality for any UI element.
// It raises events when the button is pressed and released.
public class PushToTalkButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnButtonPressed;
    public event Action OnButtonReleased;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("[PushToTalkButton] Pointer Down detected.");
        OnButtonPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("[PushToTalkButton] Pointer Up detected.");
        OnButtonReleased?.Invoke();
    }
}
