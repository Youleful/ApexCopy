using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
[CustomEditor(typeof(Test))]
public class ObjectCreator : Editor
{
    private GameObject model;
    private Vector3 Position;
    private float positionY;
    // Start is called before the first frame update
    private void OnSceneGUI()
    {
        Test test = (Test)target;
        //开始绘制GUI
        Handles.BeginGUI();

        //规定显示的GUI区域
        GUILayout.BeginArea(new Rect(0, 0,400,400));

        model = EditorGUILayout.ObjectField("添加物体", model, typeof(GameObject), true) as GameObject;

        positionY = EditorGUILayout.FloatField(positionY) ;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (Event.current.type == EventType.KeyDown)
            {
                //射线获得点

                Position = hitInfo.point;
                Position.y = positionY;
                EditorGUILayout.LabelField("选中的坐标", Position.ToString());
            }
            EditorGUILayout.LabelField("鼠标在游戏中的坐标", hitInfo.point.ToString());
        }

        EditorGUILayout.LabelField("按任意键选中坐标");
        EditorGUILayout.LabelField("选中的坐标", Position.ToString());
        if (GUILayout.Button("创建",GUILayout.Width(200)))
       {
            GameObject obj = Instantiate(model, Position, Quaternion.identity);
            obj.AddComponent<Rigidbody>();
            obj.AddComponent<BoxCollider>();
        }

        GUILayout.EndArea();

        Handles.EndGUI();
    }

    private void OnInspectorUpdate()
    {
        this.Repaint();
    }
}
