using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Manages over buttons clicks in the main menu.
public class MainMenuManager : MonoBehaviour {

    public Button startButton;

	// Use this for initialization
	void Start () {
        startButton.onClick.AddListener(() => GameManager.instance.StartGame(false));
    }

    //Takes the player to the credit scene.
    public void CreditsButtonEvent() {
        GameManager.instance.ChangeGameState(GameManager.GameState.Credits);
    }

    //Takes the player to how to play scene.
    public void HowToPlayButtonEvent() {
        GameManager.instance.ChangeGameState(GameManager.GameState.HowToPlay);
    }
}
