using UnityEngine;
using SocketIO;
using System.Collections;
using System.Collections.Generic;

public class SceneCtrl : MonoBehaviour {
	public GameObject Login_Scene;
	public GameObject Lobby_Scene;
	public GameObject Room_Scene;


	private SocketIOComponent socket;
	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("CONNECTED", OnAuthen);
		socket.On("USER_JOIN", OnUserJoin);
	}
		
	// Update is called once per frame
	void Update () {
	
	}

	void OnUserJoin (SocketIOEvent e){
		Debug.Log("User join room");
		CanvasGroup room_canvas = Room_Scene.GetComponent<CanvasGroup>();

		StartCoroutine (FadeOut (Lobby_Scene, 1.0f));
		StartCoroutine (FadeIn (Room_Scene, 1.0f));
	}

	void OnAuthen(SocketIOEvent e){
		Debug.Log("Change to lobby scene");
		StartCoroutine (FadeOut (Login_Scene, 1.0f));
		StartCoroutine (FadeIn (Lobby_Scene, 1.0f));
	}

	IEnumerator FadeIn(GameObject scene, float speed)
	{	
		CanvasGroup canvasGroup = scene.GetComponent<CanvasGroup>();
		while (canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += speed * Time.deltaTime;
			yield return null;
		}
		scene.SetActive (true);
		canvasGroup.interactable = true;
	}

	IEnumerator FadeOut(GameObject scene, float speed)
	{
		CanvasGroup canvasGroup = scene.GetComponent<CanvasGroup>();
		while (canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= speed * Time.deltaTime;
			yield return null;
		}
		scene.SetActive (false);
		canvasGroup.interactable = false;
	}
}
