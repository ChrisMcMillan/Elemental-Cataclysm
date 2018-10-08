using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The button that mutes the music.
public class MuteButton : MonoBehaviour {

    public GameObject xIcon;

	// Use this for initialization
	void Start () {

        xIcon.SetActive(GameManager.instance.MusicManagerRef.AudioRef.mute);
	}

    //Turns the music on and off and updates the mute button to display if the music is on or off.
    public void MuteButtonEvent() {
        GameManager.instance.MusicManagerRef.AudioRef.mute = !GameManager.instance.MusicManagerRef.AudioRef.mute;
        xIcon.SetActive(GameManager.instance.MusicManagerRef.AudioRef.mute);
    }
}
