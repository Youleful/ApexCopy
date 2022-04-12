using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GunData", menuName = "BagItem/GunItem")]
public class GunItem :ScriptableObject
{
    [SerializeField]
    public AmmoKind ammoKind;

    //当前剩余子弹数
    public int currAmmo;

    [SerializeField]
    public string gunName;

    //每发子弹造成的伤害
    public int damage;

    //最大子弹数
    public int maxAmmo;

    //射速
    public float shootSpeed;

    public Sprite ammoSprite;
    public Sprite gunSprite;
    public  void intial(GunItem item)
    {
        ammoKind = item.ammoKind;
        currAmmo = item.currAmmo;
        maxAmmo = item.maxAmmo;
        shootSpeed = item.shootSpeed;
        gunName = item.gunName;
        ammoSprite = item.ammoSprite;
        gunSprite = item.gunSprite;
    }
}
