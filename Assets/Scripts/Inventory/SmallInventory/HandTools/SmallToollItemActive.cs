using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallToollItemActive : MonoBehaviour
{
    public SmallToolItemObj obj;
    public Animator animator;
    public Transform HitPose;
    public float HitRayLeight = 0.4f;

    public AudioSource audioSource;
    public AudioClip[] Starthit;
    public AudioClip[] Hits;


    public LayerMask layerMask;

    int Hitstate = 0;

    public float Radius = 10;
    public float Height = -1f;
    public AnimationCurve DeformProfile;
    public int PaintInLayer = 1;


    void Start()
    {

    }

    void Update()
    {

        if (obj.Durability <= 0)
        {
            AudioSource.PlayClipAtPoint(Hits[0], transform.position);
            obj.DestroyItem();
           
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && CursorController.IsActive == false)
        {
            animator.SetTrigger("Hit");
            
        }
        if (Hitstate == 1)
        {
            OnHit();
        }
    }

    public void Hit(int state)
    {
        Hitstate = state;
    }

    public void SoundOn()
    {
        audioSource.PlayOneShot(Starthit[Random.Range(0, Starthit.Length)]);
    }

    public virtual void OnHit()
    {
        if (Physics.Raycast(HitPose.position, HitPose.forward, out RaycastHit hit, HitRayLeight, layerMask))
        {
            IDamagable damagable = null;
            damagable = hit.collider.GetComponent<IDamagable>();

            if(damagable != null)
            {
                MaterialType mT = damagable.GetMaterialType();

                if(mT == MaterialType.Stone)
                {
                    obj.Durability -= obj.GetDamageMult * damagable.BackDamageMulty();
                    obj.itemUI.UpdateUIItem();
                    damagable.SendDamage(obj.StoneHarvest, hit.point);
                    audioSource.PlayOneShot(Hits[Random.Range(0, Hits.Length)]);
                    Hitstate = 0;
                    animator.SetTrigger("Stophit");
                }
                if (mT == MaterialType.Wood)
                {
                    obj.Durability -= obj.GetDamageMult * damagable.BackDamageMulty();
                    obj.itemUI.UpdateUIItem();
                    damagable.SendDamage(obj.WoodHarvest, hit.point);
                    audioSource.PlayOneShot(Hits[Random.Range(0, Hits.Length)]);
                    Hitstate = 0;
                    animator.SetTrigger("Stophit");
                }
                if (mT == MaterialType.Animal)
                { 
                    obj.Durability -= obj.GetDamageMult * damagable.BackDamageMulty();
                    obj.itemUI.UpdateUIItem();
                    damagable.SendDamage(obj.AnimalDamage, hit.point);
                    audioSource.PlayOneShot(Hits[Random.Range(0, Hits.Length)]);
                    Hitstate = 0;
                    animator.SetTrigger("Stophit");
                }
                if (mT == MaterialType.Building)
                {
                    obj.Durability -= obj.GetDamageMult * damagable.BackDamageMulty();
                    obj.itemUI.UpdateUIItem();
                    damagable.SendDamage(obj.BuildDamage, hit.point);
                    audioSource.PlayOneShot(Hits[Random.Range(0, Hits.Length)]);
                    Hitstate = 0;
                    animator.SetTrigger("Stophit");
                }
            }
            else
            {
                obj.Durability -= obj.GetDamageMult * 0.1f;
                obj.itemUI.UpdateUIItem();
                audioSource.PlayOneShot(Hits[Random.Range(0, Hits.Length)]);
                Hitstate = 0;
                animator.SetTrigger("Stophit");
            }

            DeformableTerrain deformableTerrain;
            if (hit.collider.TryGetComponent(out deformableTerrain))
            {
                deformableTerrain.ApplyDeformProfile(hit.textureCoord, Radius, Height, DeformProfile, PaintInLayer);
            }
        }
    }

}
