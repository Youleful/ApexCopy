using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BattaryData", menuName = "BagItem/BatteryItem")]
public class BatteryItem : Item
{
    [SerializeField]
     float useTime=7.0f;
     public float batteryValue;

    public float GetUseTime()
    {
        return useTime;
    }

}
