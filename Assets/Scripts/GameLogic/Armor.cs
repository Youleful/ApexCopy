using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour 
{
    ArmorItem armorItem;

    private void Start()
    {
        armorItem = ScriptableObject.CreateInstance<ArmorItem>();
        armorItem.initialOnTheGround();
    }


    public ArmorItem GetArmorData()
    {
        return armorItem;
    }

    public void CallBack()
    {
        StartCoroutine(Create());
    }

    IEnumerator Create()
    {
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(true);
    }


}
