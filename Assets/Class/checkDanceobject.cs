
using UnityEngine;
using System.Collections;
using System;

// Use this for initialization
[Serializable]
public class checkDanceobject
{
    public int[] player1_dance;
    public int[] player2_dance;
    public bool[] answer;
    public bool isEnd;
    public static checkDanceobject createFromJson(string json)
    {

        checkDanceobject info = JsonUtility.FromJson<checkDanceobject>(json);
        return info;
    }
}

