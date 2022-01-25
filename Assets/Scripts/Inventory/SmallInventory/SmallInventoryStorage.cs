using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallInventoryStorage : MonoBehaviour, IsInteractable
{
    public GameObject smallInventory;
    SmallInventory smallInventorystorage;
    public AudioSource audioSource;
    public AudioClip opensound;


    public void OnAction()
    {
        smallInventorystorage.gameObject.SetActive(true);
        smallInventorystorage.audioSource = Player.player.Inventory.audioSource;
        Player.player.OtherInventory = smallInventorystorage;
        audioSource.PlayOneShot(opensound);
    }

    private void Start()
    {
        GameObject g = Instantiate(smallInventory, GlobalCanvas.globalCanvas.PlayerCanvas);
        smallInventorystorage = g.GetComponent<SmallInventory>();
    }


}
