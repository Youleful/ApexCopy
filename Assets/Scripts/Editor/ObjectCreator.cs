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
        //��ʼ����GUI
        Handles.BeginGUI();

        //�涨��ʾ��GUI����
        GUILayout.BeginArea(new Rect(0, 0,400,400));

        model = EditorGUILayout.ObjectField("�������", model, typeof(GameObject), true) as GameObject;

        positionY = EditorGUILayout.FloatField(positionY) ;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (Event.current.type == EventType.KeyDown)
            {
                //���߻�õ�

                Position = hitInfo.point;
                Position.y = positionY;
                EditorGUILayout.LabelField("ѡ�е�����", Position.ToString());
            }
            EditorGUILayout.LabelField("�������Ϸ�е�����", hitInfo.point.ToString());
        }

        EditorGUILayout.LabelField("�������ѡ������");
        EditorGUILayout.LabelField("ѡ�е�����", Position.ToString());
        if (GUILayout.Button("����",GUILayout.Width(200)))
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
