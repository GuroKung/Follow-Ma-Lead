using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {
    public AudioSource lobby;                 //Drag a reference to the audio source which will play the music.
    public AudioSource end;
    public AudioSource game;
    public static AudioController instance = null;     //Allows other scripts to call functions from SoundManager.             
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.


    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start () {
        game.Stop();
        end.Stop();
        lobby.Play();
	}
     
    public void playLobby()
    {
        game.Stop();
        end.Stop();
        lobby.Play();

    }
    public void playGame()
    {
        end.Stop();
        lobby.Stop();
        game.Play();
    }
    public void playEnd()
    {
        lobby.Play();
        game.Stop();
        end.Play();

    }

    // Update is called once per frame
    void Update () {
	
	}
}
