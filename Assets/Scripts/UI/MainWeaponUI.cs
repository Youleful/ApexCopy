using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainWeaponUI : MonoBehaviour
{
    public static MainWeaponUI instance = null;
    Bag bag;
    Text gun_name;
    Image weapon_image;
    Image kind_of_ammo;
    // Start is called before the first frame update

    public void SetBag(Bag b)
    {
        bag = b;
    }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
        {
           
             if (t.name.CompareTo("weapon_image") == 0)
            {
                weapon_image = t.GetComponent<Image>();
            }
            else  if (t.name.CompareTo("GunName") == 0)
            {
                gun_name = t.GetComponent<Text>();
            }
            else if (t.name.CompareTo("kind_of_ammo") == 0)
            {
                kind_of_ammo = t.GetComponent<Image>();
            }
        }
    }

    public void SetUI()
    {
        if (bag.isMainGunNull())
            return;
        weapon_image.sprite = bag.GetMainGun().GetGunSprite();
        gun_name.text = bag.GetMainGun().GetGunName();
        kind_of_ammo.sprite = bag.GetMainGun().GetAmmoSprite();
    }

}
