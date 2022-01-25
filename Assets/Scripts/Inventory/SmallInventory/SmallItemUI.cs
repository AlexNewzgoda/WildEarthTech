using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SmallItemUI : MonoBehaviour
{
    public SmallInventory CurrentSmallInventory;
    public SmallItemObj smallItemObj;

    public string Name;
    public string Description;

    public Image image;

    public bool isStacable = false;
    public int StackSize = 0;
    public int MaxStackSize = 100;

    public Text StackCounter;

    public AudioClip InventoryUp;
    public AudioClip InventoryDown;

    public void OnUse()
    {

    }
    public void OnDeUse()
    {

    }

}
