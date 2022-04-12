using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimPoint_diy : MonoBehaviour
{
    public Texture texture;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnGUI()
    {
        //相当于除以2。(x >> 1) 和 (x / 2) 的结果是一样的
        Rect rect = new Rect(Input.mousePosition.x - (texture.width >> 1),
            Screen.height - Input.mousePosition.y - (texture.height >> 1),
            texture.width, texture.height);

        GUI.DrawTexture(rect, texture);
    }

}
