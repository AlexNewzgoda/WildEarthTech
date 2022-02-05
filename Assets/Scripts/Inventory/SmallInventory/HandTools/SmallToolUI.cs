using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallToolUI : SmallItemUI

{
    public Image durab;


    private void Start()
    {
        durab.fillAmount = smallItemObj.GetComponent<SmallToolItemObj>().Durability;
    }

    public override void UpdateUIItem()
    {
        durab.fillAmount = smallItemObj.GetComponent<SmallToolItemObj>().Durability;
    }
}
