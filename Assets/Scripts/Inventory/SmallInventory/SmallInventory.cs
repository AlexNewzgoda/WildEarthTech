using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallInventory : MonoBehaviour
{

    public SmallItemUI[] smallItems;
    public ItemSlot[] itemSlots;
    public GameObject ItemUIPrefab;
    public bool isActiveChange = true;

    public AudioSource audioSource;
    public void Awake()
    {
        smallItems = new SmallItemUI[itemSlots.Length];
        if(isActiveChange)
        gameObject.SetActive(false);
    }



    public bool ItemSetInFromUp(SmallItemObj item)
    {
        if (item.isStackable)
        {
            int itemleft = StackNewItem(item);
            if (itemleft > 0)
            {
                if (CheckInventory())
                {
                    SmallItemUI su = CreateNewItem(item);
                    su.StackSize = itemleft;
                    SoundOnDown(su);
                    smallItems[GetFreePlace()] = su;
                    item.gameObject.SetActive(false);
                    

                    return true;
                }
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        else
        {
            if (CheckInventory())
            {
                SmallItemUI su = CreateNewItem(item);
                smallItems[GetFreePlace()] = su;
                SoundOnUp(su);
                Resort();
                item.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }

    public bool ItemSetFromDrag(SmallItemUI item, ItemSlot itemSlot)
    {
        item.CurrentSmallInventory = this;
        smallItems[GetSlotIndx(itemSlot)] = item;
        SoundOnDown(item);
        Resort();
        return true;
    }
    public bool ItemSetFromDrag(SmallItemUI item, ItemSlot itemSlot, int oldpos)
    {
        int i = GetSlotIndx(itemSlot);
        if (i != -1)
        {
            if (smallItems[i] == null)
            {
                item.CurrentSmallInventory = this;
                smallItems[i] = item;
                SoundOnDown(item);
                Resort();
                return true;
            }
            else
            {
                if (item.isStacable == false || item.isStacable == true && item.Name != smallItems[i].Name)
                {
                  

                    if (item.CurrentSmallInventory.ItemSetFromDrag(smallItems[i], item.CurrentSmallInventory.itemSlots[oldpos]))
                    {
                        
                        item.CurrentSmallInventory = this;
                        SoundOnDown(item);
                        smallItems[i] = item;
                        Resort();
                        return true;
                    }
                    else
                    {
                        //Resort();
                        return false;
                    }
                }
                else if(smallItems[i].Name == item.Name)
                {
                    int c = StackNewItem(smallItems[i], item);
                    if(c > 0)
                    {
                        SmallItemUI s = CreateNewItem(item);
                        s.StackSize = c;
                        SoundOnDown(item);
                        item.CurrentSmallInventory.ItemSetFromDrag(s, item.CurrentSmallInventory.itemSlots[oldpos]);
                        Destroy(item.smallItemObj.gameObject);
                        Destroy(item.gameObject);
                        Resort();
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void SoundOnDown(SmallItemUI item)
    {
        if (item.InventoryDown != null)
            audioSource.PlayOneShot(item.InventoryDown, 1);
    }
    void SoundOnUp(SmallItemUI item)
    {
        if (item.InventoryUp != null)
        audioSource.PlayOneShot(item.InventoryUp, 1);
    }

    int GetSlotIndx(ItemSlot slot)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == slot)
            {
                return i;
            }

        }
        return -1;
    }

    int StackNewItem(SmallItemObj item)
    {
        for (int x = 0; x < smallItems.Length; x++)
        {
            SmallItemUI SU = smallItems[x];
            if (SU != null)
            {
                if (SU.Name == item.Name)
                {
                    
                    if (SU.StackSize + item.StackSize < item.MaxStackSize)
                    {
                        SU.StackSize += item.StackSize;
                        Resort();
                        SoundOnUp(SU);
                        return 0;
                    }
                    else
                    {
                        int extra = SU.StackSize + item.StackSize - item.MaxStackSize;
                        SU.StackSize = SU.StackSize + item.StackSize - extra;
                        
                        return extra;
                    }
                }
              
            }
        }
        return item.StackSize;
    }
    int StackNewItem(SmallItemUI item)
    {
        for (int x = 0; x < smallItems.Length; x++)
        {
            SmallItemUI SU = smallItems[x];
            if (SU != null)
            {
                if (SU.Name == item.Name)
                {
                    if (SU.StackSize + item.StackSize < item.MaxStackSize)
                    {
                        SU.StackSize += item.StackSize;
                        Resort();
                        return 0;
                    }
                    else
                    {
                        int extra = SU.StackSize + item.StackSize - item.MaxStackSize;
                        SU.StackSize = SU.StackSize + item.StackSize - extra;
                        return extra;
                    }
                }
            }
        }
        return item.StackSize;
    }
    int StackNewItem(SmallItemUI curitem, SmallItemUI item)
    {
        if (curitem.StackSize + item.StackSize < item.MaxStackSize)
        {
            curitem.StackSize += item.StackSize;
            Destroy(item.smallItemObj.gameObject);
            Destroy(item.gameObject);
            Resort();
            SoundOnUp(item);
            return 0;
        }
        else
        {
            int extra = curitem.StackSize + item.StackSize - item.MaxStackSize;
            curitem.StackSize = curitem.StackSize + item.StackSize - extra;

            return extra;
        }



    }
    

    bool CheckInventory()
    {
        for (int i = 0; i < smallItems.Length; i++)
        {
            if(smallItems[i] == null)
            {
                return true;
            }
        }
        return false;
    }

    int GetFreePlace()
    {
        for (int i = 0; i < smallItems.Length; i++)
        {
            if (smallItems[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    SmallItemUI CreateNewItem(SmallItemObj item)
    {
        GameObject pref = Instantiate(ItemUIPrefab, this.transform);
        SmallItemUI s_item = pref.GetComponent<SmallItemUI>();
        s_item.smallItemObj = item;
        s_item.image.sprite = item.icon;
        s_item.Name = item.Name;
        s_item.Description = item.Description;
        s_item.transform.position = Vector3.zero;
        s_item.MaxStackSize = item.MaxStackSize;
        s_item.StackSize = item.StackSize;
        s_item.isStacable = item.isStackable;
        s_item.CurrentSmallInventory = this;
        s_item.InventoryUp = item.InventoryUp;
        s_item.InventoryDown = item.InventoryDown;
        return s_item;
    }
    SmallItemUI CreateNewItem(SmallItemUI item)
    {
        GameObject pref = Instantiate(ItemUIPrefab, this.transform);
        SmallItemUI s_item = pref.GetComponent<SmallItemUI>();
        s_item.smallItemObj = Instantiate(item.smallItemObj);
        s_item.image.sprite = item.image.sprite;
        s_item.Name = item.Name;
        s_item.Description = item.Description;
        s_item.transform.position = Vector3.zero;
        s_item.MaxStackSize = item.MaxStackSize;
        s_item.StackSize = item.StackSize;
        s_item.isStacable = item.isStacable;
        s_item.CurrentSmallInventory = this;
        s_item.InventoryUp = item.InventoryUp;
        s_item.InventoryDown = item.InventoryDown;
        return s_item;
    }

    public void Resort()
    {
        for (int i = 0; i < smallItems.Length; i++)
        {
            if(smallItems[i] != null)
            {
                smallItems[i].transform.SetParent(itemSlots[i].transform);
                smallItems[i].transform.position = itemSlots[i].transform.position;

                if (smallItems[i].isStacable)
                {
                    if(smallItems[i].StackSize > 1)
                    {
                        smallItems[i].StackCounter.gameObject.SetActive(true);
                        smallItems[i].StackCounter.text = smallItems[i].StackSize.ToString();
                    }
                    
                }
            }
        }
    }

    public int ReleaseItem(SmallItemUI item)
    {
        for (int x = 0; x < smallItems.Length; x++)
        {
            if (smallItems[x] == item)
            {
                SoundOnUp(smallItems[x]);
                smallItems[x] = null;
                
                return x;
            }
               
            
        }
        return -1;
    }

    

}
