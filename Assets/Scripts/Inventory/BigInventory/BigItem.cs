using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigItem : MonoBehaviour
{
    public string Name;

    [TextArea]
    public string Description;
    public Sprite icon;
    public AudioClip HitSound;
    public AudioSource audioSource;

    public Rigidbody SelfRigidbody;
    public Collider SelfCollider;

    public bool storagble = true;

    public BigItemStorage CurrentStorage;
    public bool isGrabbed = false;

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(HitSound);
    }
}
