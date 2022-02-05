using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreBoostHarvest : MonoBehaviour, IDamagable
{
    public Ore mainstone;
    public float DamageBoost = 3;
    IDamagable stone;

    public GameObject partpref;

    private void Awake()
    {
        stone = mainstone.GetComponent<IDamagable>();
    }

    public float BackDamageMulty()
    {
       return  stone.BackDamageMulty();
    }

    public MaterialType GetMaterialType()
    {
        return stone.GetMaterialType();
    }

    public void SendDamage(float damage, Vector3 pos)
    {
        stone.SendDamage(damage * DamageBoost, pos);
        Instantiate(partpref, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

}
