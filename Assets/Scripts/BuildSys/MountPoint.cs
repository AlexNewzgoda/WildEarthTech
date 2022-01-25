using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountPoint : MonoBehaviour
{
    public BlockConfig MainParent;
    public Transform MountPos;
    public GameObject MountedBlock;
    public Collider ColliederSelf;

    public void CheckSide()
    {
        if(Physics.Raycast(transform.position, ( MountPos.position - transform.position).normalized, out RaycastHit hit, 1.2f))
        {
            
                if (hit.transform.gameObject.GetComponent<MountPoint>() != null)
                {
                    hit.transform.gameObject.GetComponent<MountPoint>().MountedBlock = this.gameObject;
                    hit.transform.gameObject.GetComponent<MountPoint>().ColliederSelf.enabled = false;
                    MountedBlock = hit.transform.gameObject;
                }
            
        }
        if(MountedBlock == null)
        {
            ColliederSelf.enabled = true;
        }
        
    }

    public void Deconstruction()
    {
        if(MountedBlock == null)
        {
            ColliederSelf.enabled = true;
        }
    }

}
