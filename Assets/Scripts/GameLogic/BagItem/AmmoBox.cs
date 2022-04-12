using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AmmoKind
{
    LightAmmo = 0,
    ShotGunAmmo = 1,
    Sniper = 2
}
[CreateAssetMenu(fileName ="AmmoData",menuName ="BagItem/AmmoBox")]
public class AmmoBox : Item
{
     public AmmoKind ammoKind;
}
