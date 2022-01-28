using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableGroundCollision : MonoBehaviour
{
    private DeformableGround _deformableGround;
    public List<Rigidbody> _rigidbodies = new List<Rigidbody>();
    public float VelocityDown = 0.1f;

    public struct RigidbodyData
    {
        public Rigidbody Body;
        public Vector3 Velocity;
        public float Time;
    }

    public void Reg (Rigidbody rbody)
    {
        if (!_rigidbodies.Contains(rbody))
        {
            _rigidbodies.Add(rbody);
        }
    }

    public void UnReg(Rigidbody rbody)
    {
        _rigidbodies.Remove(rbody);
    }

    private void Start()
    {
        _deformableGround = this.GetComponent<DeformableGround>();
    }

    private void FixedUpdate()
    {
        for(int q = 0; q < _rigidbodies.Count; q++)
        {
            Rigidbody rigidbody = _rigidbodies[q];

            if (!rigidbody.IsSleeping())
            {
                rigidbody.isKinematic = true;
                _deformableGround.GetHeightAsync(_deformableGround.WorldToUVCoord(rigidbody.transform.position), (x) => HeightResult(x, new RigidbodyData()
                {
                    Body = rigidbody,
                    Velocity = rigidbody.velocity,
                    Time = Time.time
                }), 0.05f);
            }
        }
    }

    private void HeightResult (float value, RigidbodyData rbodyData)
    {
        rbodyData.Body.isKinematic = false;
        if (rbodyData.Body.transform.position.y < value)
        {
            rbodyData.Body.velocity *= VelocityDown * Time.fixedDeltaTime;
            rbodyData.Body.angularVelocity *= VelocityDown * Time.fixedDeltaTime;
            rbodyData.Body.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }
    }
}
