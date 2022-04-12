using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="PlayerState",menuName ="PlayerState")]
public class PlayerState : ScriptableObject
{
    public Bag bag;
    public int life;

    public int maxLife;
    public int Q_CD ;
    public float Z_CD ;
}
