using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="HelmetData",menuName ="BagItem/Helmet")]
public class HelmetItem : ScriptableObject
{ 
     int level;
    int protectDamage
    {
        get { return level * 10; }
    }


   public void SetInitialLevel()
    {
        level = Random.Range(1,4);
    }

    public void Initial()
    {
        level =1;
    }
    public void SetLevel(int i)
    {
        level = i;
    }

   public void MinimizeDamage(int damage)
    {
        damage -= protectDamage;
    }

    public int GetLevel()
    {
        return level;
    }
}
