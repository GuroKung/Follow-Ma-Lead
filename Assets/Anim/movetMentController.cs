using UnityEngine;
using System.Collections;

public class movetMentController : MonoBehaviour {
    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void setDance(int i)
    {
        anim.SetInteger("PressKey", i);
    }
}
