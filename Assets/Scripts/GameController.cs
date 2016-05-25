using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	private string player;
	private SocketIOComponent socket;
	private string lead;
	private string follow;
	private GameObject[] players;
    private bool isStart = false;
    private bool isTurn = false;
	private bool isLogin = false;
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private List<int> dances = new List<int>();
    private int turn = 3;
    // anime
    Animator anim;
    // Use this for initialization
    void Start () {
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		// Socket Events
		socket.On("CONNECTED", OnAuthen);
		socket.On("USER_JOIN", OnUserJoin);
		socket.On("GAMESTART", OnGameStart);
		socket.On("ON_LEADDANCE", OnLeadDance);
		socket.On("ON_CHECKDANCE", OnCheckDance);
        // anime
        anim = GetComponent<Animator>();
    }
		
    private bool isLead()
    {
        return lead == player;
        
    }
		
	public void OnAuthen(SocketIOEvent e){
		Debug.Log(e.data.ToString());
		player = e.data.GetField("id").ToString();
	}

	public void OnUserJoin(SocketIOEvent e){
		Debug.Log("is Joined");
		Debug.Log(e.data.ToString());
	}

	public void Join () {
		Debug.Log ("User Join game");
		JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
		string n = player.Substring (1, player.Length - 2);
		j.AddField("player_id", n);
		print (j.ToString());
		socket.Emit("JOIN_GAME", j);
	}

	public void OnGameStart(SocketIOEvent e){
		lead = e.data.GetField("player1").GetField("id").ToString();
		follow = e.data.GetField("player2").GetField("id").ToString();
        isTurn = isLead();
		Debug.Log(lead);
		Debug.Log(follow);
        if(isTurn) sendLeadDance();
	}

	public void sendLeadDance(){
		if(dances.Count >= turn)
        {
            Debug.Log("----send lead dance-----");
            Dictionary<string, string> data = new Dictionary<string, string>();
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
            foreach (int i in dances)
            {
                arr.Add(i);
            }
            j.AddField("dances", arr);
            j.AddField("player", lead);
            Debug.Log(j.ToString());
            socket.Emit("LEADDANCE", j);
        }
	}

	public void OnLeadDance(SocketIOEvent e){
		//player : [lead] dance follow e.data.dance
		Debug.Log(e.data.ToString());
        changeTurn();
		if(isTurn) sendFollowDance();
	}

    private void changeTurn()
    {
        isTurn = !isTurn;
        dances = new List<int>();
    }

	public void sendFollowDance(){
		//player : [follow] send input
		// Dictionary<string, string> data = new Dictionary<string, string>();
		// data["dance"] = new String[];
		// ["l","l","d","l"];
		if(dances.Count >= turn)
        {
            Debug.Log("------send Follow dance -----------");
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
            foreach (int i in dances)
            {
                arr.Add(i);
            }
            j.AddField("dances", arr);
            j.AddField("player", follow);
            Debug.Log(j.ToString());
            socket.Emit("FOLLOWDANCE", j);
        }
	}

	public void OnCheckDance(SocketIOEvent e){
        if (e.data.GetField("isEnd").ToString() == "true"){
			Debug.Log("You lose");
		}
        changeTurn();

    }
    private void addDances(int i)
    {
        if(dances.Count < turn)
        {
            dances.Add(i);
        }
    }

    // Update is called once per frame
    void Update () {
        if (isTurn)
        {
            //print ("Your turn ");
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    //save began touch 2d point
                    firstPressPos = new Vector2(t.position.x, t.position.y);
                }
                if (t.phase == TouchPhase.Ended)
                {
                    //save ended touch 2d point
                    secondPressPos = new Vector2(t.position.x, t.position.y);

                    //create vector from the two points
                    currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                    //normalize the 2d vector
                    currentSwipe.Normalize();

                    //swipe upwards
                    if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        Debug.Log("up swipe");

                        //anim
                        anim.SetInteger("PressKey", 1);
                        anim.SetInteger("PressKey2", 1);

                        addDances(0);
                    }
                    //swipe down
                    if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        Debug.Log("down swipe");

                        //anim
                        anim.SetInteger("PressKey", 2);
                        anim.SetInteger("PressKey2", 2);

                        addDances(1);
                    }
                    //swipe left
                    if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        Debug.Log("left swipe");

                        //anim
                        anim.SetInteger("PressKey", 3);
                        anim.SetInteger("PressKey2", 3);

                        addDances(2);
                    }
                    //swipe right
                    if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        Debug.Log("right swipe");

                        //anim
                        anim.SetInteger("PressKey", 4);
                        anim.SetInteger("PressKey2", 4);

                        addDances(3);
                    }
                }
            }
        }
    }
}
