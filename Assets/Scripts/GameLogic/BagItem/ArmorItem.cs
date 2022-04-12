using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="ArmorData",menuName ="BagItem/Armor")]
public class ArmorItem:ScriptableObject
{
    int level;
    public int maxArmor {
        get { return level*150; }
    }
    [SerializeField]
    int maxlevel=4;

    [SerializeField]
    int currentArmor=150;

     public int needDamage
    {
        get { return level * 150; }
    }

    int hasDoDamage=0;

    public void Initial()
    {
        level = 1;
        currentArmor = 150;
    }

    public void initialOnTheGround()
    {
        level = Random.Range(1, 3);
        currentArmor = level * 150;
    }

    public void SetLevel(int i)
    {
        level = i;
    }

    public void UpdateLevel()
    {
        if(hasDoDamage>=needDamage&&level<maxlevel)
        {
            level++;
            hasDoDamage = 0;
        }
    }
    
    public int GetLevel()
    {
        return level;
    }
    public int GetCurrArmor()
    {
        return currentArmor;
    }
    public void GetDamage(int damage)
    {
        currentArmor -= damage;
        if (currentArmor < 0)
            currentArmor = 0;
    }

    //已造成的伤害
    public int GetHasDoDamage()
    {
        return hasDoDamage;
    }

   
}
