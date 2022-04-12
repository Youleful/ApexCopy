using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet : MonoBehaviour
{
    HelmetItem helmetItem;

    private void Start()
    {
        helmetItem = ScriptableObject.CreateInstance<HelmetItem>();
        helmetItem.SetInitialLevel();
    }

    public int GetHelmetLevel()
    {
        Debug.Log(helmetItem.GetLevel());
        return helmetItem.GetLevel();
    }

    public void CallBack()
    {
        StartCoroutine(Create());
    }

    IEnumerator Create()
    {
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(true);
    }
}
