using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveViever : MonoBehaviour
{
    private GameObject target;
    public GameObject Ghost;
    public string InteractiveTag = "Interactive";
    private float InteractLeight = 2;
    public LayerMask layerMask;

    public static InteractiveViever interactiveViever;

    private void Awake()
    {
        interactiveViever = this;
        InteractLeight = Player.player.RayLeight;
        Ghost.transform.SetParent(null);
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5, layerMask))
        {
            if(Vector3.Distance(hit.point, transform.position) < InteractLeight)
            {
                if (hit.transform.gameObject.tag == InteractiveTag)
                {
                    if(hit.transform.GetComponent<IsInteractable>() != null)
                    {
                        if (Input.GetKeyDown(InputSettings.key_Use) && CursorController.IsActive == false)
                        {
                            hit.transform.GetComponent<IsInteractable>().OnAction();
                        }
                        
                    }

                    if (target == null || target != hit.transform.gameObject)
                    {

                        target = hit.transform.gameObject;

                        if (target.GetComponent<MeshFilter>() != null)
                        {
                            Ghost.GetComponent<MeshFilter>().mesh = target.GetComponent<MeshFilter>().mesh;
                        }
                        else
                        {
                            Ghost.GetComponent<MeshFilter>().mesh = null;
                        }

                        
                        Ghost.transform.position = target.transform.position;
                        Ghost.transform.rotation = target.transform.rotation;
                        Ghost.transform.localScale = target.transform.localScale;
                        Ghost.SetActive(true);
                    }

                }
                else
                {
                    ClearSign();
                }
            }
            else
            {
                ClearSign();
            }



        }
        else
        {
            ClearSign();
        }

        if(target != null)
        {
            Ghost.transform.position = target.transform.position;
            Ghost.transform.rotation = target.transform.rotation;
            Ghost.transform.localScale = target.transform.localScale;
        }
    }

    public void ClearSign()
    {
        Ghost.SetActive(false);
        target = null;
    }

}
