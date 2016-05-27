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
    public Text phaseText;
    public Text dance;

	public InputField username;
	public InputField password;

	public InputField regis_username;
	public InputField regis_password;

	// Playfab
	private string titleId = "145A";
	public string PlayFabId;

	public GameObject SceneCtrl;

    private SocketIOComponent socket;
	private string lead;
	private string follow;
    private string player;
    private int playerID;
    private int playerPos;
    private int[] pos;

    private bool isStart = false;
    private bool isTurn = false;
	private bool isLogin = false;

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    private List<int> dances = new List<int>();
    private int turn = 3;
    private int phase = 0;


    // Use this for initialization
    void Start () {
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
        pos = new int[2];
		// Socket Events
		socket.On("NET_AVAILABLE", onConnection);
		socket.On("CONNECTED", OnAuthen);
		socket.On("USER_JOIN", OnUserJoin);
		socket.On("GAMESTART", OnGameStart);
		socket.On("ON_LEADDANCE", OnLeadDance);
		socket.On("ON_CHECKDANCE", OnCheckDance);
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

	public void  Regis () {
		Debug.Log ("User Regis");
		//RegisterPlayFabUser
		RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
		{
			TitleId = titleId,
			Username = regis_username.text,
			Password = regis_password.text,
			RequireBothUsernameAndEmail = false
		};
		PlayFabClientAPI.RegisterPlayFabUser(request, (result) => {
			PlayFabId = result.PlayFabId;
			Debug.Log("Got PlayFabID: " + PlayFabId);

			SceneCtrl.GetComponent<SceneCtrl>().ToLogin();
		},
			(error) => {
				Debug.Log("Error logging in register");
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
        this.GetComponent<AudioController>().playGame();
		lead = e.data.GetField("player1").GetField("id").ToString();
		follow = e.data.GetField("player2").GetField("id").ToString();
        lead = lead.Substring(1, lead.Length - 2);
        follow = follow.Substring(1, follow.Length - 2);
        player = player.Substring(1, player.Length - 2);
        playerTexts[0].text = "lead";
        playerTexts[1].text = "follow";
        dance.text = turn + " moves";
        for(int i = 0; i<pos.Count(); i++)
        {
            pos[i] = i;
        }
		isTurn = isLead();
		if (isTurn)
        {
            playerPos = 0;

			test.text = "Let's dance";
        }
        else
        {
            playerPos = 1;

			test.text = "focus on opponent moves";
        }
        
        phase = 1;
        phaseText.text = "Lead Turn";
	}

	public void sendLeadDance(){
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
		movetMentController play = players[pos[0]].GetComponent<movetMentController>();
        
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
        phaseText.text = "Follow Turn";
		if (isTurn) {
			test.text = "your turn";
		}else{
			test.text = "opponent turn";	
		}
        
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
        checkDanceobject a = checkDanceobject.createFromJson(e.data.ToString());
        for(int i = 0;i<a.player1_dance.Count(); i++)
        {
            int x = a.player1_dance[i];
            players[pos[0]].GetComponent<movetMentController>().setDance(x);

            x = a.player2_dance[i];
            players[pos[1]].GetComponent<movetMentController>().setDance(x);
        }
        if (a.isEnd == true)
        {
            test.text = "Game is End";
            for(int i = 0; i<2; i++)
            {
                players[i].GetComponent<movetMentController>().setDance(5 + i);

            }
			if (isLead ()) {
				playerTexts [playerPos].text = "Winner";
			} else {
				playerTexts [playerPos].text = "Loser";
			}

            this.GetComponent<AudioController>().playEnd();
        }else {
            endTurn();
        }
        

    }
    private void addDances(int i)
    {
        if(dances.Count < turn)
        {
            dances.Add(i);
            if(dances.Count == turn)
            {
                addDances(0);
            }

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

        string temp = lead;
        lead = follow;
        follow = temp;

        int tempI = pos[0];
        pos[0] = pos[1];
        pos[1] = tempI;

        if (isLead())
            isTurn = isLead();
        
        phase = 1;
        phaseText.text = "Lead Turn";
        turn++;
        dance.text = turn + " moves";
        playerTexts[pos[0]].text = "lead";
        playerTexts[pos[1]].text = "follow";
		if (isTurn) {
			test.text = "Now, Your turn to lead";

		} else {
			test.text = "focus on opponent moves";

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
