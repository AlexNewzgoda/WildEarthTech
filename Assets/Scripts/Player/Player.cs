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

    public Transform HandPose;

    public Animator animator;

    float _x = 0;
    float _y = 0;
    public float AnimBlendSpeed = 10;

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

        _x = Mathf.Lerp(_x, Input.GetAxis("Mouse X"), Time.deltaTime * AnimBlendSpeed);
        _y = Mathf.Lerp(_y, Input.GetAxis("Mouse Y"), Time.deltaTime * AnimBlendSpeed);

        animator.SetFloat("x", _x);
        animator.SetFloat("y", _y);


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
