using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void SendDamage(float damage, Vector3 pos);
    public float BackDamageMulty();
    public MaterialType GetMaterialType();
}

public enum MaterialType
{
    Wood,
    Stone,
    Building,
    Animal
}
