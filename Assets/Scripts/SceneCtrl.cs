using UnityEngine;
using SocketIO;
using System.Collections;
using System.Collections.Generic;

public class SceneCtrl : MonoBehaviour {
	public CanvasGroup Login_Scene;
	public CanvasGroup Lobby_Scene;
	public CanvasGroup Room_Scene;

	private SocketIOComponent socket;
	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("CONNECTED", OnAuthen);
	}
		
	// Update is called once per frame
	void Update () {
	
	}

	void OnAuthen(SocketIOEvent e){
		Debug.Log("Change to lobby scene");
		StartCoroutine (FadeOut (Login_Scene, 1.0f));
		StartCoroutine (FadeIn (Lobby_Scene, 1.0f));
	}

	IEnumerator FadeIn(CanvasGroup canvasGroup, float speed)
	{
		while (canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += speed * Time.deltaTime;
			yield return null;
		}
		canvasGroup.interactable = true;
	}

	IEnumerator FadeOut(CanvasGroup canvasGroup, float speed)
	{
		while (canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= speed * Time.deltaTime;
			yield return null;
		}
		canvasGroup.interactable = false;
	}
}
