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
        //�൱�ڳ���2��(x >> 1) �� (x / 2) �Ľ����һ����
        Rect rect = new Rect(Input.mousePosition.x - (texture.width >> 1),
            Screen.height - Input.mousePosition.y - (texture.height >> 1),
            texture.width, texture.height);

        GUI.DrawTexture(rect, texture);
    }

}
