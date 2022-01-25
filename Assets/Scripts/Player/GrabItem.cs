using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItem : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform GrabPosition;
    public BigItem GrabedItem;
    private float GrabLeight = 2;
    public Transform dir;

    

    private void Start()
    {
        GrabLeight = Player.player.RayLeight;
    }

    void Update()
    {
        if(GrabedItem == null)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, GrabLeight, layerMask))
            {
               

                BigItem item = hit.transform.GetComponent<BigItem>();
                SmallItemObj smallItem = hit.transform.GetComponent<SmallItemObj>();

                if (item != null)
                {
                    if (Input.GetKeyDown(InputSettings.key_Use))
                    {
                        if(item.CurrentStorage != null)
                        {
                            item.CurrentStorage.Release(item);
                        }
                        item.SelfCollider.enabled = false;
                        item.SelfRigidbody.isKinematic = true;
                        item.transform.SetParent(this.transform);
                        item.isGrabbed = true;
                        
                        GrabedItem = item;
                    }
                }

                if(smallItem != null)
                {
                    if (Input.GetKeyDown(InputSettings.key_Use))
                    {
                        Player.player.Inventory.ItemSetInFromUp(smallItem);
                    }
                }
            }
        }
        else
        {
            GrabedItem.transform.position = Vector3.Lerp(GrabedItem.transform.position, GrabPosition.position, 30*Time.deltaTime);
            GrabedItem.transform.rotation = GrabPosition.rotation;
            if (Input.GetKeyDown(InputSettings.key_Use))
            {
                GrabedItem.transform.position = dir.position + (Vector3.up*0.5f) + (dir.forward);
                GrabedItem.SelfCollider.enabled = true;
                GrabedItem.SelfRigidbody.isKinematic = false;
                GrabedItem.transform.SetParent(null);
                GrabedItem.isGrabbed = false;
                GrabedItem = null;
                
            }
        }
    }
}
