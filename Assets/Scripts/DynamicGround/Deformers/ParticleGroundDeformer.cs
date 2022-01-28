using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGroundDeformer : MonoBehaviour
{
    public float _powerMultipler = 1f;

    private ParticleSystem Particles;

    private void Awake()
    {
        Particles = this.GetComponent<ParticleSystem>();
        //_deformableGround = this.GetComponent<DeformableGround>();
    }

    private void OnParticleCollision(GameObject other)
    {
        DeformableGround deformableGround;
        if(other.TryGetComponent(out deformableGround))
        {

            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            int cCount = ParticlePhysicsExtensions.GetCollisionEvents(Particles, other, collisionEvents);

            for(int q = 0; q < cCount; q++)
            {
                Vector2 uvCoord = deformableGround.WorldToUVCoord(collisionEvents[q].intersection);
                deformableGround.ApplyDeformSphere(uvCoord, _powerMultipler, 1, 1);
            }
        }
    }
}
