using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag
{
    Gun[] ownGun = new Gun[2];

    //物品储存栏
    List<Item> items = new List<Item>();

    DictionaryOfItem dictionary;

    public void SetGun(Gun g,int i)
    {
        try
        {
            ownGun[i] = g;
        }
        catch
        {
            Debug.Log("检查你的数组下标是否越界");
        }
    }

    public bool isMainGunNull()
    {
        return ownGun[0] == null;
    }

    public bool isSecondaryGunNull()
    {
        return ownGun[1] == null;
    }

    public Gun GetMainGun()
    {
       
            return ownGun[0];
       
    }

    public Gun GetSecondaryGun()
    {
        
            return ownGun[1];
    }

    int level;

    public int maxItemNum
    {
        get { return 10 + 2 * level; }
    }

    HelmetItem helmet;

    int maxLevel = 4;

    ArmorItem armor;

    public HelmetItem GetHelmetItem()
    {
        return helmet;
    }

    public ArmorItem GetArmorItem()
    {
        return armor;
    }

    public List<Item> GetItemList()
    {
        return items;
    }

    public Bag()
    {
        level = 1;
        helmet = ScriptableObject.CreateInstance<HelmetItem>();
        helmet.Initial();
        armor = ScriptableObject.CreateInstance<ArmorItem>();
        armor.Initial();
        dictionary = Resources.Load<DictionaryOfItem>("DictionaryOfItem");
        dictionary.Initialize();
    }
    public int GetLevel()
    {
        return level;
    }
    public void UpdateLevel()
    {
        if (level < maxLevel)
            level++;
    }

     bool isFull()
    {
        return items.Count <= maxItemNum;
    }

    void Sort()
    {
        //根据ID大小排序
        items.Sort((i1, i2) =>
        {
            if (i1 != i2)
            {
                return i1.sortID.CompareTo(i2.sortID);
            }
            else return 0;
        });
    }

    public void AddItem(int num, int id)
    {
        if (!isFull())
            return;
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.itemName = dictionary.dictionOfItem[id].itemName;
        newItem.maxNum = dictionary.dictionOfItem[id].maxNum;
        newItem.sortID = dictionary.dictionOfItem[id].sortID;
        newItem.itemSprite = dictionary.dictionOfItem[id].itemSprite;
        for (int i = 0; i < items.Count; i++)
        {
            //看是否有id相同的格子
            if (items[i].sortID == id && items[i].num != items[i].maxNum)
            {
                items[i].AddNum(num);
                if (items[i].num > items[i].maxNum && isFull())
                {
                    int remain = items[i].num - items[i].maxNum;
                    items[i].num = items[i].maxNum;
                    AddItem(remain, id);
                }
                return;
            }
        }
        newItem.num = num;
        items.Add(newItem);
        Debug.Log(newItem.num);
        Debug.Log(dictionary.dictionOfItem[id].num);
        Sort();
    }

    public int GetAmmo(AmmoKind ammo)
    {
        int sum=0;
        for(int i=0;i<items.Count;i++)
        {
            if(items[i].sortID==(int)ammo)
            {
                sum += items[i].num;
            }
        }
        return sum;
    }
    
    public void ReduceAmmo(AmmoKind ammo,int n)
    {

        int i=items.FindLastIndex((Item i) =>
        {
            return i.sortID == (int)ammo;
        }
        );
        if (items[i].num < n)
        {
            int remainAmmo = n-items[i].num;
            items.RemoveAt(i);
            ReduceAmmo(ammo,remainAmmo);
        }
        else
            items[i].num -= n;
    }

    public void SetArmor(ArmorItem armorItem)
    {
        armor = armorItem;

    }

    public void SetHelmetLevel(int i)
    {
        helmet.SetLevel(i);
    }

    public int GetHelmetLevel()
    {
        return helmet.GetLevel();
    }

    public bool isGunSame()
    {
        if (ownGun[0] != null && ownGun[1] != null)
          return ownGun[0].name.Equals(ownGun[1].name);
        else return false;
    }

    public bool isGunSame(string name)
    {
        return name.Equals(ownGun[0].name);
    }
}
