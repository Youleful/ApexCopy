using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="stageOfGun",menuName ="new WeaponStore")]

public class Weapon :ScriptableObject
{
    public List<GameObject> weapon = new List<GameObject>();
}
