using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="DictionaryOfItem",menuName ="BagItem/DictionaryOfItem")]
public class DictionaryOfItem : ScriptableObject
{
    public Dictionary<int, Item> dictionOfItem = new Dictionary<int, Item>();
    public List<Item> basicItem = new List<Item>();
    public void Initialize()
    {
        for(int i=0;i<basicItem.Count;i++)
        {
            basicItem[i].sortID = i;
            dictionOfItem.Add(i, basicItem[i]);
        }
    }
}
