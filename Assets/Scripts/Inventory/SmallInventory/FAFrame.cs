using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAFrame : MonoBehaviour
{
    public SmallInventory fastaccess;

    int index = 0;
    int oldselect = 0;

    void Update()
    {
        index += (int)Input.GetAxis("Mouse ScrollWheel");
        if(index > 8)
        {
            index = 0;
        }
        if(index < 0)
        {
            index = 8;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot1))
        {
            index = 0;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot2))
        {
            index = 1;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot3))
        {
            index = 2;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot4))
        {
            index = 3;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot5))
        {
            index = 4;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot6))
        {
            index = 5;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot7))
        {
            index = 6;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot8))
        {
            index = 7;
        }
        if (Input.GetKeyDown(InputSettings.key_Slot9))
        {
            index = 8;
        }


        transform.position = fastaccess.itemSlots[index].transform.position;
        UpdateUseItem();
    }

    void UpdateUseItem()
    {
        if(index != oldselect)
        {
            if(fastaccess.smallItems[oldselect] != null)
            {
                fastaccess.smallItems[oldselect].OnDeUse();
                oldselect = index;
            }
        }
        if (fastaccess.smallItems[index] != null)
        {
            fastaccess.smallItems[index].OnUse();
        }
        
    }

}
