using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallToollItemActive : MonoBehaviour
{
    public SmallToolItemObj obj;
    public Animator animator;
    public Transform HitPose;

    int Hitstate = 0;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("Hit");
        }
        if (Hitstate == 1)
        {
            OnHit();
        }
    }

    public void Hit(int state)
    {
        Hitstate = state;
    }

    public virtual void OnHit()
    {
       
    }

}
