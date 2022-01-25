using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTargetCanvasGroup : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private bool _interactable = false;

    private void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        OnInteractableChange();
    }

    private void Update()
    {
        if(_canvasGroup.interactable != _interactable)
        {
            _interactable = _canvasGroup.interactable;
            OnInteractableChange();
        }
    }

    private void OnInteractableChange ()
    {
        if(_interactable)
        {
            CursorController.Targets += Null;
        }
        else
        {
            CursorController.Targets -= Null;
        }
    }

    private void Null() { }
}
