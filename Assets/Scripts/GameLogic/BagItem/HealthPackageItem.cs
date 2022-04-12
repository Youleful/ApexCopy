using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HealthPackageData", menuName = "BagItem/HealthPackageItem")]
public class HealthPackageItem : Item
{
    [SerializeField]
     float useTime=7.0f;
    public float healthValue=100.0f;

    public float GetUseTime()
    {
        return useTime;
    }
}
