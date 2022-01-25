using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class RigidbodySerializer : MonoBehaviour, ISaveSerialized
{
    public struct RBData
    {
        public float Mass;
        public bool IsKinematic;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
    }

    public string GetSavedData()
    {
        Rigidbody rigidbody;
        if (this.TryGetComponent(out rigidbody))
        {
            return GameSerializer.GetDataFromObject(new RBData
            {
                Mass = rigidbody.mass,
                IsKinematic = rigidbody.isKinematic,
                Velocity = rigidbody.velocity,
                AngularVelocity = rigidbody.angularVelocity
            });
        }

        return string.Empty;
    }

    public void SetSavedData(string data)
    {
        RBData rbData = GameSerializer.GetObjectFromData<RBData>(data);

        Rigidbody rigidbody;
        if (this.TryGetComponent(out rigidbody))
        {
            rigidbody.mass = rbData.Mass;
            rigidbody.isKinematic = rbData.IsKinematic;
            rigidbody.velocity = rbData.Velocity;
            rigidbody.angularVelocity = rbData.AngularVelocity;
        }
    }
}
