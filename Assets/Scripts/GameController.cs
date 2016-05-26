using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
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

	public InputField username;
	public InputField password;

	// Playfab
	public string titleId = "145A";
	public string PlayFabId;

	public GameObject SceneCtrl;

    private SocketIOComponent socket;
	private string lead;
	private string follow;
    private string player;
    private int playerID;
    private int playerPos;

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
		socket.On("NET_AVAILABLE", onConnection);
		socket.On("CONNECTED", OnAuthen);
		socket.On("USER_JOIN", OnUserJoin);
		socket.On("GAMESTART", OnGameStart);
		socket.On("ON_LEADDANCE", OnLeadDance);
		socket.On("ON_CHECKDANCE", OnCheckDance);
        // anime
        anim = GetComponent<Animator>();
        //players[0].GetComponent<movetMentController>().setDance(1);
    }		

	void Awake(){
		PlayFabSettings.TitleId = titleId; // title id goes here.
	} 

	public void onConnection(SocketIOEvent e){
		Debug.Log("is Connected");
		Debug.Log(e.data.ToString());
	}

	public void Login () {
		Debug.Log ("User Login");
		LoginWithPlayFabRequest request = new LoginWithPlayFabRequest()
		{
			TitleId = titleId,
			Username = username.text,
			Password = password.text
		};

		PlayFabClientAPI.LoginWithPlayFab(request, (result) => {
			PlayFabId = result.PlayFabId;
			Debug.Log("Got PlayFabID: " + PlayFabId);

			if(result.NewlyCreated)
			{
				Debug.Log("(new account)");
			}
			else
			{
				Debug.Log("(existing account)");
			}
			Dictionary<string, string> data = new Dictionary<string, string>();
			data["name"] = username.text;
			data["password"] = password.text;
			socket.Emit("LOGIN", new JSONObject(data));
			SceneCtrl.GetComponent<SceneCtrl>().ToLooby();
		},
			(error) => {
				Debug.Log("Error logging in player");
				Debug.Log(error.ErrorMessage);
			});
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
        if (isLead())
        {
            playerPos = 0;
        }
        else
        {
            playerPos = 1;
        }
        isTurn = isLead();
        phase = 1;
        test.text = ""+isTurn;
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
        socket.Emit("LEADDANCE", j);
	}

	public void OnLeadDance(SocketIOEvent e){
        test.text = "On lead dance";
		movetMentController play = players[0].GetComponent<movetMentController>();
        
        ArrayObject a = ArrayObject.createFromJson(e.data.ToString());
        foreach ( int x in a.lead_dances)
        {
            play.setDance(x);
        }
        phase2();

    }
    void phase2()
    {
        isTurn = !isLead();
        phase = 2;
        test.text = "" + isTurn;
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
        test.text = "on Check Dance";
        checkDanceobject a = checkDanceobject.createFromJson(e.data.ToString());
        test.text = e.data.ToString();
        foreach(int x in a.player1_dance)
        {
            players[0].GetComponent<movetMentController>().setDance(x);
            string t = " " + x;
            playerTexts[0].text += t;
        }
        foreach (int x in a.player2_dance)
        {
            string t = " " + x;
            playerTexts[1].text += t;
            players[1].GetComponent<movetMentController>().setDance(x);
        }
        if (a.isEnd == true)
        {
            test.text = "Game is End";
            for(int i = 0; i<2; i++)
            {
                players[i].GetComponent<movetMentController>().setDance(5 + i);
            }
        }else {
            endTurn();
        }
        

    }
    private void addDances(int i)
    {
        if(dances.Count < turn)
        {
            dances.Add(i);
            players[playerPos].GetComponent<movetMentController>().setDance(i);

        }
        else
        {
            if(phase == 1 && isTurn)
            {
                test.text = "send lead dance";
                sendLeadDance();
                changeTurn();
            }else if (phase == 2 && isTurn)
            {
                test.text = "send follow dance";
                sendFollowDance();
                changeTurn();
            }
        }
    }
    private void endTurn()
    {
        GameObject pl = players[0];
        players[0] = players[1];
        players[1] = pl;

        string temp = lead;
        lead = follow;
        follow = temp;

        if (isLead())
        {
            playerPos = 1;
            isTurn = isLead();
        }

        else
        {
            playerPos = 0;
        }    

        phase = 1;
        turn++;
        test.text = "" + isTurn;
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
                     
                        addDances(1);
                    }
                    //swipe down
                    if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        Debug.Log("down swipe");
                        addDances(2);
                    }
                    //swipe left
                    if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        Debug.Log("left swipe");
                        addDances(4);
                    }
                    //swipe right
                    if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        Debug.Log("right swipe");



                        addDances(3);
                    }
                }
            }
        }
    }
}
