using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionHandler : MonoBehaviour
{
    public struct InteractionInfo
    {
        public InteractionInfo (InteractionHandler handler, InteractiveTarget target, int actionMode)
        {
            Handler = handler;
            Target = target;
            ActionMode = actionMode;
        }

        public InteractionHandler Handler;
        public InteractiveTarget Target;
        public int ActionMode;
    }

    public LayerMask IgnoredMask;
    public Transform CasterPoint;
    public float CasterLength = 2f;

    public InteractiveTarget Target { get; private set; }

    private RaycastHit _useActionHit;
    [Space]
    public UnityEvent<InteractionInfo> UseEvent;
    public UnityEvent<InteractionInfo> EnterEvent;
    public UnityEvent<InteractionInfo> StayEvent;
    public UnityEvent<InteractionInfo> ExitEvent;

    public void Use (int actionMode = 0)
    {
        if (Target != null)
        {
            Target.OnUse(this);
            UseEvent?.Invoke(new InteractionInfo(this, Target, actionMode));
        }
    }

    private void CastUpdate ()
    {
        if (Physics.Raycast(new Ray(CasterPoint.transform.position, CasterPoint.transform.forward), out _useActionHit, CasterLength, ~(IgnoredMask | 1<<2)))
        {
            InteractiveTarget iTarg = _useActionHit.collider.GetComponent<InteractiveTarget>();

            if (Target != iTarg)
            {
                if (Target != null)
                {
                    Target.OnExit(this);
                    ExitEvent?.Invoke(new InteractionInfo(this, Target, 0));
                }

                Target = iTarg;

                if (Target != null)
                {
                    Target.OnEnter(this);
                    EnterEvent?.Invoke(new InteractionInfo(this, Target, 0));
                }
            }
            else
            {
                if (Target != null)
                {
                    Target.OnStay(this);
                    StayEvent?.Invoke(new InteractionInfo(this, Target, 0));
                }
            }
        }
        else
        {
            if (Target != null)
            {
                Target.OnExit(this);
                ExitEvent?.Invoke(new InteractionInfo(this, Target, 0));
                Target = null;
            }
        }
    }

    private void FixedUpdate()
    {
        CastUpdate();
    }

    private void OnDisable()
    {
        if (Target != null)
            Target.OnExit(this);
    }
}
