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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anim.SetInteger("pressKey", 1);
            anim.SetInteger("pressKey2", 1);
            anim.SetInteger("PressKey", 1);
            anim.SetInteger("PressKey2", 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anim.SetInteger("pressKey", 2);
            anim.SetInteger("pressKey2", 2);
            anim.SetInteger("PressKey", 2);
            anim.SetInteger("PressKey2", 2);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.SetInteger("pressKey", 3);
            anim.SetInteger("pressKey2", 3);
            anim.SetInteger("PressKey", 3);
            anim.SetInteger("PressKey2", 3);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.SetInteger("pressKey", 4);
            anim.SetInteger("pressKey2", 4);
            anim.SetInteger("PressKey", 4);
            anim.SetInteger("PressKey2", 4);
        }
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            anim.SetInteger("pressKey", 5);
            anim.SetInteger("pressKey2", 5);
            anim.SetInteger("PressKey", 5);
            anim.SetInteger("PressKey2", 5);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            anim.SetInteger("pressKey", 6);
            anim.SetInteger("pressKey2", 6);
            anim.SetInteger("PressKey", 6);
            anim.SetInteger("PressKey2", 6);
        }
    }
}
