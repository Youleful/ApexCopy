using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Item : ScriptableObject
{
    public int num;
    public int maxNum;
    public string itemName;
    public int sortID;
    public Sprite itemSprite;

    //改变物体数量
    public void AddNum(int n)
    {
        num += n;
    }

    public void SubstractNum(int n)
    {
        num -= n;
    }

}
