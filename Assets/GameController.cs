using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	private SocketIOComponent socket;
	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		socket.On("NET_AVAILABLE", onConnection);
		socket.On ("CONNECTED", OnAuthen);
	}

	public void onConnection(SocketIOEvent e){
		Debug.Log("is Connected");
		Debug.Log(e.data.ToString());

		Dictionary<string, string> data = new Dictionary<string, string>();
		data["name"] = "John Doe";
		socket.Emit("LOGIN", new JSONObject(data));
	}

	public void OnAuthen(SocketIOEvent e){
		Debug.Log(e.data.ToString());
	}

	// Update is called once per frame
	void Update () {

	}
}
