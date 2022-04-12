using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BagManager : MonoBehaviour
{
    public static BagManager Instance = null;

    Bag bag;
    Image armor;
    Image helmet;
    Image BagLevel;

    public void SetBag(Bag b)
    {
        bag = b;
        MainWeaponUI.instance.SetBag(b);
        SecondaryWeaponUI.instance.SetBag(b);
    }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
        {
             if (t.name.CompareTo("Bag_level") == 0)
            {
                BagLevel = t.GetComponent<Image>();
            }
        }
    }
    //每次游戏启动前动态的更新背包UI
   
    // Update is called once per frame

    public BagGrid bagGrid;
    public GameObject myBag;
    void Update()
    {
        UpdateItemToUI(); 
    }
    //将仓库中的物品显示到UI上
    public  void InsertItemToUI(Item item)
    {
        BagGrid grid = Instantiate(Instance.bagGrid, Instance.myBag.transform);
        grid.item = item;
        grid.Intialize();
    }

    public  void UpdateItemToUI()
    {
        for (int i = 0; i < Instance.myBag.transform.childCount; i++)
        {
            Destroy(Instance.myBag.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < Instance.bag.GetItemList().Count; i++)
        {
            InsertItemToUI(Instance.bag.GetItemList()[i]);
        }
        MainWeaponUI.instance.SetUI();
        SecondaryWeaponUI.instance.SetUI();
        armor = PlayerStateManager.playerStateManager_instance.GetArmor();
        helmet = PlayerStateManager.playerStateManager_instance.GetHelmet();
    }


    public void DestroyItemToUI()
    {

    }


   
}
