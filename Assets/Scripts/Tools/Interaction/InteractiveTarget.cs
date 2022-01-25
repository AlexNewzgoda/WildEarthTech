using UnityEngine;
using UnityEngine.Events;

public class InteractiveTarget : MonoBehaviour
{
    [Header("Info")]
    public string Text;
    public Texture2D Icon;
    public UnityEvent InfoUpdateEvent;

    [Space]
    public UnityEvent<InteractionHandler> UseEvent;

    [Space]
    public UnityEvent<InteractionHandler> EnterEvent;
    public UnityEvent<InteractionHandler> StayEvent;
    public UnityEvent<InteractionHandler> ExitEvent;

    public virtual void OnUse(InteractionHandler sender)
    {
        UseEvent?.Invoke(sender);
    }

    public virtual void OnEnter(InteractionHandler sender)
    {
        EnterEvent?.Invoke(sender);
    }

    public virtual void OnStay(InteractionHandler sender)
    {
        StayEvent?.Invoke(sender);
    }

    public virtual void OnExit(InteractionHandler sender)
    {
        ExitEvent?.Invoke(sender);
    }
}
