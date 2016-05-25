using UnityEngine;
using System.Collections;

public class movetCon : MonoBehaviour
{
    Animator anim;
    // Use this for initialization
    void Start()
    {
        Debug.Log("eiei");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("eiei");
            anim.SetInteger("PressKey", 1);     
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anim.SetInteger("PressKey", 2);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.SetInteger("PressKey", 3);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.SetInteger("PressKey", 4);
        }
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            anim.SetInteger("PressKey", 5);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            anim.SetInteger("PressKey", 6);
        }
    }
}

