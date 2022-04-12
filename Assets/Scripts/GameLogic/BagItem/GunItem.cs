using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GunData", menuName = "BagItem/GunItem")]
public class GunItem :ScriptableObject
{
    [SerializeField]
    public AmmoKind ammoKind;

    //��ǰʣ���ӵ���
    public int currAmmo;

    [SerializeField]
    public string gunName;

    //ÿ���ӵ���ɵ��˺�
    public int damage;

    //����ӵ���
    public int maxAmmo;

    //����
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
