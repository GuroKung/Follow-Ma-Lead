using UnityEngine;
using System.Collections;
using System;

// Use this for initialization
[Serializable]
public class ArrayObject
{
    public int[] lead_dances;

    public static ArrayObject createFromJson(string json)
    {

        ArrayObject info = JsonUtility.FromJson<ArrayObject>(json);
        return info;
    }
}

