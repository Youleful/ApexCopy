using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStateManager : MonoBehaviour
{
    public static WeaponStateManager weaponStateManager_Instance = null;

    Bag bag;
     Gun gun;
    //ÎÄ×Ö¿Ø¼þ
    Text ammo_count;
    Text gun_name;
    Text ammo_own;

    //Í¼Æ¬¿Ø¼þ
     Image weapon_image;
     Image kind_of_ammo;
    private void Awake()
    {
        weaponStateManager_Instance = this;
    }

    public void SetBag(Bag b)
    {
        bag = b;
    }

    public void SetGun(Gun g)
    {
        gun = g;
    }
    void Start()
    {
        foreach(Transform t  in  this.transform.GetComponentsInChildren<Transform>())
        {
            if(t.name.CompareTo("ammo_num")==0)
            {
                ammo_count = t.GetComponent<Text>();
            }
            else if(t.name.CompareTo("weapon_image") ==0)
            {
                weapon_image = t.GetComponent<Image>();
            }
            else if (t.name.CompareTo("GunName") == 0)
            {
                gun_name = t.GetComponent<Text>();
            }
            else if(t.name.CompareTo("kind_of_ammo")==0)
            {
                kind_of_ammo = t.GetComponent<Image>();
            }
            else if(t.name.CompareTo("ammo_own")==0)
            {
                ammo_own = t.GetComponent<Text>();
            }
        }
    }

    public void SetWeaponUI()
    {
        gun_name.text = gun.GetGunName();
        weapon_image.sprite = gun.GetGunSprite();
        kind_of_ammo.sprite = gun.GetAmmoSprite();
        ammo_count.text = string.Format("{0}", gun.GetCurrAmmo());
        ammo_own.text = bag.GetAmmo(gun.GetAmmoKind()).ToString();
    }
    
   

}
