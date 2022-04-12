using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunPool : MonoBehaviour
{
    public Weapon weapon;
    public GameObject[] gameObjects = new GameObject[2];
    public List<GameObject> pooledObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject t in weapon.weapon)
        {
            GameObject obj = (GameObject)Instantiate(t,this.transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }

        gameObjects[0] = null;
        gameObjects[1] = null;
        
    }

    private void Update()
    {
        
    }
    //获取哪个枪
    public GameObject GetGun(string name)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //添加对象池后别忘记修改这里
           
             if (pooledObjects[i].name.Contains(name))
              {
                    return pooledObjects[i];
             }  
        }
        return null;
    }

    //获取激活的
    public GameObject GetActiveGun()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //添加对象池后别忘记修改这里

            if (pooledObjects[i].activeInHierarchy )
            {
                return pooledObjects[i];
            }
        }
        return null;
    }


}
