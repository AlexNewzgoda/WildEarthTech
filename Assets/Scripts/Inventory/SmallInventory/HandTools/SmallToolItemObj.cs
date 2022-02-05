using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallToolItemObj : SmallItemObj
{

    
    public GameObject ActiveTool;

    public float Durability = 1;
    public float GetDamageMult = 0.1f;
    public float WoodHarvest = 0.2f;
    public float StoneHarvest = 0.2f;
    public float AnimalDamage = 1;
    public float BuildDamage = 1;


    public override void OnSelectUdate()
    {
        GameObject t = Instantiate(ActiveTool);
        t.GetComponent<SmallToollItemActive>().obj = this;
        Player.player.SetTool(t);
    }

    public override void OnDeselect()
    {
        Player.player.RemoveTool();
    }

    public void DestroyItem()
    {
        Player.player.RemoveTool();
        Destroy(itemUI.gameObject);
        Destroy(gameObject);
    }

}
