using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallItemObj : MonoBehaviour
{
    public string Name;

    [TextArea]
    public string Description;
    public Sprite icon;

    public bool isStackable = false;
    public int StackSize = 1;
    public int MaxStackSize = 100;
    public bool isUsability = false;
    public GameObject UITemplate;

    public SmallItemUI itemUI;

    public AudioClip InventoryUp;
    public AudioClip InventoryDown;
    public AudioClip InventoryHit;

    public AudioSource audioSource;

    public virtual void OnSelectUdate()
    {

    }
    public virtual void OnDeselect()
    {
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(InventoryHit);
    }

}
