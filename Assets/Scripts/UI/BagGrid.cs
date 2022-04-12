using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BagGrid : MonoBehaviour
{
    public Image gridImage;
    public  Text gridNum;
    public Item item;
     
    public void Intialize()
    {
         gridImage.sprite = item.itemSprite;
        gridNum.text = item.num.ToString();
    }
}
