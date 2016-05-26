using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using SocketIO;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public List<GameObject> players;
    public Text test;
    public List<Text> playerTexts;

    private SocketIOComponent socket;
	private string lead;
	private string follow;
    private string player;
    private int playerID;

    private bool isStart = false;
    private bool isTurn = false;
	private bool isLogin = false;

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    private List<int> dances = new List<int>();
    private int turn = 3;
    private int phase = 0;
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
        //players[0].GetComponent<movetMentController>().setDance(1);
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
        lead = lead.Substring(1, lead.Length - 2);
        follow = follow.Substring(1, follow.Length - 2);
        player = player.Substring(1, player.Length - 2);
        playerTexts[0].text = lead;
        playerTexts[1].text = follow;
        isTurn = isLead();
        phase = 1;
        //if(isTurn) sendLeadDance();
	}

	public void sendLeadDance(){
        Dictionary<string, string> data = new Dictionary<string, string>();
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
        foreach (int i in dances)
        {
            arr.Add(i);
        }
        j.AddField("dances", arr);
        j.AddField("player", player);
        Debug.Log(j.ToString());
        test.text = lead;
        socket.Emit("LEADDANCE", j);
	}

	public void OnLeadDance(SocketIOEvent e){
        test.text = "On lead dance";
		movetMentController play = players[0].GetComponent<movetMentController>();
        test.text = "";
        ArrayObject a = ArrayObject.createFromJson(e.data.ToString());
        foreach ( int x in a.lead_dances)
        {
            print(x);
            test.text += x + " ";
            play.setDance(x);
        }
        //changeTurn();
		//if(isTurn) sendFollowDance();
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
            players[0].GetComponent<movetMentController>().setDance(i);
            foreach(int d in dances)
            {
                test.text += " " + d;
            }
        }
        else
        {
            if(phase == 1 && isTurn)
            {
                test.text = "send lead dance";
                sendLeadDance();
                changeTurn();
            }else if (phase == 2 && !isLead())
            {
                sendFollowDance();
            }
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
                        test.text = "up " + 1;
                        addDances(1);
                    }
                    //swipe down
                    if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        Debug.Log("down swipe");
                        test.text = "down " + 2;
                        addDances(2);
                    }
                    //swipe left
                    if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        Debug.Log("left swipe");
                        test.text = "left " + 4;
                        addDances(4);
                    }
                    //swipe right
                    if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        Debug.Log("right swipe");
                        test.text = "right " + 3;
                        addDances(3);
                    }
                }
            }
        }
    }
}
