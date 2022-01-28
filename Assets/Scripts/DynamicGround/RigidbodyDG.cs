using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyDG : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
    }

    private void OnTriggerStay (Collider other)
    {
        DeformableGroundCollision collision;
        if (other.TryGetComponent(out collision))
        {
            collision.Reg(_rigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DeformableGroundCollision collision;
        if (other.TryGetComponent(out collision))
        {
            collision.UnReg(_rigidbody);
        }
    }
}
