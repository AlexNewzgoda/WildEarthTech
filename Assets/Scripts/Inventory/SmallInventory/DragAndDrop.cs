using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    SmallItemUI item;
    int itempos;

    public Transform droppose;

    void Start()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (CursorController.IsActive)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if(item == null)
                {
                    GetItem();
                }
                else
                {
                    item.transform.position = Input.mousePosition;
                }
            }
            else
            {
                if(item != null)
                {
                    CheckPlaceForDrop();
                }
            }
            
        }
    }

    List<RaycastResult> RaycastUI()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        return results;
    }

    void GetItem()
    {
        List<RaycastResult> results = RaycastUI();

        for (int i = 0; i < results.Count; i++)
        {
            SmallItemUI su = results[i].gameObject.GetComponent<SmallItemUI>();
            if(su != null)
            {
                itempos = su.CurrentSmallInventory.ReleaseItem(su);
                su.transform.SetParent(GlobalCanvas.globalCanvas.transform);
                item = su;
                return;
            }
        }
    }

    void CheckPlaceForDrop()
    {
        item.gameObject.SetActive(false);
        List<RaycastResult> results = RaycastUI();
        item.gameObject.SetActive(true);

        ItemSlot itemslot = null;
        SmallInventory inventory = null;
        for (int i = 0; i < results.Count; i++)
        {
            if (itemslot == null)
                itemslot = results[i].gameObject.GetComponent<ItemSlot>();

            if (inventory == null)
                inventory = results[i].gameObject.GetComponentInParent<SmallInventory>();
        }


        if (inventory != null)
        {
            if (inventory.ItemSetFromDrag(item, itemslot, itempos) == false)
            {
                DropItemOut();
            }
            else
            {
                if(inventory.frame != null)
                {
                    inventory.frame.UpdateEmptyToUse();
                }
            }
            ClearGrab();
        }
        else
        {
            DropItemOut();
        }
    }

    void ClearGrab()
    {
        item = null;
        itempos = -1;
    }

    void DropItemOut()
    {
        item.smallItemObj.transform.position = droppose.position;
        item.smallItemObj.gameObject.SetActive(true);
        item.smallItemObj.StackSize = item.StackSize;
        Destroy(item.gameObject);
        itempos = -1;
    }

}
