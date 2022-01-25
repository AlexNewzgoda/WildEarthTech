using UnityEngine;

public class CursorTarget : MonoBehaviour
{
    private void OnEnable()
    {
        CursorController.Targets += Null;
    }
    private void OnDisable()
    {
        CursorController.Targets -= Null;
    }

    private void Null() {}
}