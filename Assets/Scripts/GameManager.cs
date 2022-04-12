using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    public List<Vector3> AmmosPositon;
    public List<Vector3> GunPositon;

    public List<GameObject> Object;
   
    private void Awake()
    {
        for (int i = 0; i < Object.Count; i++)
        {
            if (Object[i].CompareTag("Gun"))
            {
                Object[i].AddComponent<Rigidbody>();
                Object[i].AddComponent<BoxCollider>();
            }
        }
       

    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        for(int i =0;i<Object.Count;i++)
        if(!Object[i].activeInHierarchy)
        {
            StartCoroutine(CallBack(Object[i]));
        }
    }

    IEnumerator CallBack(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.SetActive(true);
    }
}


