using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player player;

    public SmallInventory Inventory;
    public SmallInventory FastAccess;
    public SmallInventory OtherInventory;
    public float RayLeight = 2;


    private void Awake()
    {
        player = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputSettings.key_Inventory))
        {
            Inventory.Resort();
            Inventory.gameObject.SetActive(!Inventory.gameObject.activeSelf);
        }
        if(OtherInventory!= null && OtherInventory.gameObject.activeSelf && Inventory.gameObject.activeSelf == false)
        {
            Inventory.gameObject.SetActive(true);
        }
    }

    public void CloseOtherInventory()
    {
        if(OtherInventory != null)
        {
            OtherInventory.gameObject.SetActive(false);
            OtherInventory = null;
        }
    }
}
