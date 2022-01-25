using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigItemStorage : MonoBehaviour
{

    public BigItem[] itemsContain;
    public BigItemConteiner[] itemsContainers;

    public BoxCollider box;
    public LayerMask layerMask;

    public AudioSource audioSource;
    public AudioClip resortsound;

    private void Awake()
    {
        itemsContain = new BigItem[itemsContainers.Length];
    }

    private void FixedUpdate()
    {
        CheckTrigger();
    }
    void CheckTrigger()
    {
        RaycastHit[] res = new RaycastHit[1000];
        int h = Physics.BoxCastNonAlloc(transform.position + box.center, box.size/2, Vector3.up, res, Quaternion.identity, 0.1f, layerMask);
        for (int i = 0; i < h; i++)
        {
            BigItem item = res[i].collider.gameObject.GetComponent<BigItem>();
           
            if (item != null && item.isGrabbed == false && item.CurrentStorage == null && item.storagble)
            {
                if (SetInItem(item))
                {
                    item.SelfCollider.enabled = false;
                    ChangeItemState(item);
                }
            }
        }
    }

    public bool SetInItem(BigItem item)
    {

        for (int i = 0; i < itemsContain.Length; i++)
        {
            if (itemsContain[i] == null)
            { 
                item.CurrentStorage = this;
                itemsContain[i] = item;
                return true;
            }
        }

        return false;
    }

    public void ChangeItemState(BigItem item)
    {
        item.transform.SetParent(item.CurrentStorage.transform);
        item.SelfRigidbody.isKinematic = true;
        Resort();
    }

    public void Resort()
    {

        for (int i = 0; i < itemsContainers.Length; i++)
        {
            itemsContainers[i].IsTake = false;
        }


        for (int i = 0; i < itemsContain.Length; i++)
        {
            BigItem item = itemsContain[i];

            if (item != null)
            {
                
                for (int c = 0; c < itemsContainers.Length; c++)
                {
                    if(itemsContainers[c].IsTake == false)
                    {
                        item.transform.position = itemsContainers[c].transform.position;
                        item.transform.rotation = itemsContainers[c].transform.rotation;
                        item.SelfCollider.enabled = true;
                        itemsContainers[c].IsTake = true;
                        break;
                    }
                }
            }
        }
        audioSource.PlayOneShot(resortsound);

    }

    public void Release(BigItem item)
    {
        for (int i = 0; i < itemsContain.Length; i++)
        {
            if(item == itemsContain[i])
            {
                itemsContain[i] = null;
                item.CurrentStorage = null;
                break;
            }
        }
        Resort();
    }


}
