using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class movetMentController : MonoBehaviour {
    Animator anim;
    List<int> dance = new List<int>();
    int firstElement;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        firstElement = dance.First();
            if (anim.GetInteger("PressKey") == 0 && firstElement!=null)
            {
                anim.SetInteger("PressKey", firstElement);
                dance.Remove(firstElement);
            }
        }

    public void setDance(int i)
    {
        dance.Add(i);
    }
}
