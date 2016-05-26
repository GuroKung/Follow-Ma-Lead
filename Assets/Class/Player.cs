using UnityEngine;
using System.Collections;
using System;

// Use this for initialization
[Serializable]
public class Player
{
    public string id;
    public bool isLead;
    public int chId;
    public bool isUser;


    public Player(string id, bool isLead, int ch, bool isUser)
    {
        this.id = id;
        this.isLead = isLead;
        this.chId = ch;
        this.isUser = isUser;
    }
}

