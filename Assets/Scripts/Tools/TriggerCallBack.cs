using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class TriggerCallBack : MonoBehaviour
{
    
    public UnityEvent OnEnter;
    public UnityEvent OnStay;
    public UnityEvent OnExit;

    public LayerMask LayerMask;

    public string IgnoreTag = "Ground";

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != IgnoreTag)
        {
            OnEnter.Invoke();
        }
        else
        {
            OnExit.Invoke();
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != IgnoreTag)
        {
            OnEnter.Invoke();
        }
        else
        {
            OnExit.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        OnExit.Invoke();
    }
}
