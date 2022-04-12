using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;

    public int amountOfObjectInPool;
    public bool shouldExpand;
}

public class ObjectsPool : MonoBehaviour
{
    //µ¥ÀýÄ£Ê½
    public static ObjectsPool Instance = null;

    //
    // Start is called before the first frame update
    
    List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach(ObjectPoolItem items in itemsToPool)
        for(int i=0;i< items.amountOfObjectInPool;i++)
        {
            GameObject obj = (GameObject)Instantiate(items.objectToPool);       
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPooledObject(string tag)
    {

        for (int i = 0; i<pooledObjects.Count; i++) {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.CompareTag(tag))
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }

    public GameObject GetPooledObject_Item(string name)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].name.Contains(name))
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.name.Equals(name))
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
}


