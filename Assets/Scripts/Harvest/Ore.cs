using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MonoBehaviour, IDamagable
{
    public GameObject OrePrefab;
    public float BackDamageMultyplayer = 1f;
    public int OreCount = 10;


    public float OtherHP = 30;

    public MaterialType materialType;



    public GameObject tempeffect;

    private void Awake()
    {
        OtherHP = Random.Range(OtherHP / 2, OtherHP);
        OreCount = Random.Range(OreCount / 2, OreCount);
    }

    public float BackDamageMulty()
    {
        return BackDamageMultyplayer;
    }

    public MaterialType GetMaterialType()
    {
        return materialType;
    }

    public void SendDamage(float damage, Vector3 pos)
    {
        OtherHP -= damage;
        Instantiate(tempeffect, pos, Quaternion.LookRotation( pos - transform.position));
        int r = Random.Range(0,5);
        if(r == 0)
        {
           GameObject g = Instantiate(OrePrefab, pos +(pos - transform.position) * 0.2f, Quaternion.identity);
            g.GetComponent<Rigidbody>().velocity = (pos - transform.position).normalized*2;
            OreCount--;
        }

        if(OtherHP <= 0)
        {
            for (int i = 0; i < OreCount; i ++)
            {
                GameObject g = Instantiate(OrePrefab, gameObject.transform.position + Random.insideUnitSphere*0.5f + Vector3.up, Quaternion.identity);
                g.GetComponent<Rigidbody>().velocity = (g.transform.position - transform.position).normalized*2;
            }
            Destroy(gameObject);
        }

    }

    

}
