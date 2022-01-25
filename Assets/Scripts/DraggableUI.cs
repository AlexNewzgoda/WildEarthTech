using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public RectTransform Root;
    public RectTransform DraggableRoot;

    private Canvas RootCanvas;
    private CanvasScaler RootCanvasScaler;

    private void Awake()
    {
        RootCanvas = this.GetComponentInParent<Canvas>();
        RootCanvasScaler = this.GetComponentInParent<CanvasScaler>();
    }

    public void SetScreenPosition(Vector2 screenPosition)
    {  
        SetAnchoredPosition(screenPosition - new Vector2(Screen.width, Screen.height) * 0.5f);
    }

    public void SetAnchoredPosition(Vector2 anchoredPosition)
    {
        Vector2 canvasHalfSize = RootCanvasScaler.referenceResolution * 0.5f;
        Vector2 windowHalfSize = DraggableRoot.rect.size * 0.5f;

        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, -canvasHalfSize.x + windowHalfSize.x, canvasHalfSize.x - windowHalfSize.x);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, -canvasHalfSize.y + windowHalfSize.y, canvasHalfSize.y - windowHalfSize.y);

        DraggableRoot.anchoredPosition = anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition = DraggableRoot.anchoredPosition + eventData.delta / RootCanvas.scaleFactor;
        SetAnchoredPosition(newPosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Root.SetAsLastSibling();
    }
}
