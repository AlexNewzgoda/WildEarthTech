using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyGroundDeformer : MonoBehaviour
{
    public float _rayDistance = 1f;
    public float Radius = 1f;
    public float Power = 1f;

    private void FixedUpdate()
    {
        //Rigidbody rigidBody = this.GetComponent<Rigidbody>();

        Ray ray = new Ray(this.transform.position + Vector3.up * 0.01f, Vector3.down);
        RaycastHit hit;

        RaycastHit[] hits = Physics.RaycastAll(ray, _rayDistance);

        //if(hits.Length < 1)
            //rigidBody.useGravity = true;

        foreach (RaycastHit h in hits)
        {
            DeformableGround ground;
            if (h.collider.TryGetComponent(out ground))
            {
                /*float height = ground.GetHeight(h.textureCoord, 1);

                if (h.distance < 1f)
                {
                    //rigidBody.useGravity = false;
                    rigidBody.AddForce(-Physics.gravity, ForceMode.Acceleration);
                    rigidBody.MovePosition(h.point + Vector3.up * height);
                    //rigidBody.AddForce(Vector3.up * height, ForceMode.VelocityChange);
                }
                else
                {
                    //rigidBody.useGravity = true;
                }

                if (height > 0.3f)
                {
                    rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, Vector3.zero, Mathf.Pow(1 - height, 2) * Time.deltaTime * 20f);

                    //if(h.distance < 0.6)
                    //rigidBody.velocity += Vector3.up * height * 2f;

                    if (h.distance < 0.6)
                    {
                        //Debug.Log(h.distance + " | " + height);
                        //rigidBody.AddForce(Vector3.up * 150, ForceMode.Acceleration);
                        rigidBody.MovePosition(rigidBody.position + Vector3.up * (height - 0.3f));
                    }
                }

                //if(h.distance < 1f)*/
                    ground.ApplyDeformSphere(new Vector2(h.textureCoord.x, h.textureCoord.y), Radius, Power, Mathf.Clamp01(1 - h.distance));
            }
        }


        /*if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            DeformableGround ground = hit.collider.GetComponent<DeformableGround>();

            if (ground != null)
            {
                //float height = ground.GetHeight(new Vector2(hit.textureCoord.x, hit.textureCoord.y), Mathf.Clamp01(1 - hit.distance) * _powerMultipler);
                //Debug.Log(height);
                ground.ApplyDeformSphere(new Vector2(hit.textureCoord.x, hit.textureCoord.y), Mathf.Clamp01(1 - hit.distance) * _powerMultipler);
            }
        }*/
    }
}
