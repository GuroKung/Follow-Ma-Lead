using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	private SocketIOComponent socket;
	private string lead;
	private string follow;
	private GameObject[] players;
	private bool isLogin = false;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		socket.On("NET_AVAILABLE", onConnection);
		socket.On("CONNECTED", OnAuthen);
		socket.On("GAMESTART", OnGameStart);
		socket.On("ON_LEADDANCE", OnLeadDance);
		socket.On("ON_CHECKDANCE", OnCheckDance);
	}

	public void onConnection(SocketIOEvent e){
		Debug.Log("is Connected");
		Debug.Log(e.data.ToString());

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["name"] = "Smart";
		if(!isLogin){
			socket.Emit("LOGIN", new JSONObject(data));
			isLogin = true;
		}
	}

	public void OnAuthen(SocketIOEvent e){
		Debug.Log(e.data.ToString());
	}

	public void OnGameStart(SocketIOEvent e){
		Debug.Log(e.data.ToString());
		Debug.Log ("on Game Start ");
		Debug.Log(e.data.GetField("player1").ToString());
		lead = e.data.GetField("player1").GetField("id").ToString();
		follow = e.data.GetField("player2").GetField("id").ToString();

		Debug.Log(lead);
		Debug.Log(follow);
		sendLeadDance();
	}

	public void sendLeadDance(){
		Debug.Log ("----send lead dance-----");
		Dictionary<string, string> data = new Dictionary<string, string>();
		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
		arr.Add("l");
		arr.Add("l");
		arr.Add("l");
		arr.Add("l");
		j.AddField("dances", arr);
		j.AddField("player", lead);
		Debug.Log(j.ToString());
		socket.Emit("LEADDANCE", j);
	}

	public void OnLeadDance(SocketIOEvent e){
		//player : [lead] dance follow e.data.dance
		Debug.Log(e.data.ToString());
		sendFollowDance();
	}

	public void sendFollowDance(){
		//player : [follow] send input
		// Dictionary<string, string> data = new Dictionary<string, string>();
		// data["dance"] = new String[];
		// ["l","l","d","l"];
		Debug.Log("------send Follow dance -----------");
		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
		arr.Add("l");
		arr.Add("l");
		arr.Add("d");
		arr.Add("l");
		j.AddField("dances", arr);
		j.AddField("player", follow);
		Debug.Log(j.ToString());
		socket.Emit("FOLLOWDANCE", j);
	}

	public void OnCheckDance(SocketIOEvent e){
		if(e.data.GetField("isEnd").ToString() == "true"){
			Debug.Log("You lose");
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
