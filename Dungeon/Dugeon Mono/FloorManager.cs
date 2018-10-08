using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Use in the floor navigation scene to control the user interface. 
public class FloorManager : MonoBehaviour {

    public Text soulText;

    //Dispalys the player's current soul count.
    void Start() {
        soulText.text = "Soul: " + GameManager.instance.PlayerInfo.Soul.ToString();
    }

    //Takes the player to the player status scene. 
   public void StatusButtonEvent() {
        GameManager.instance.PreviousGameState = GameManager.GameState.FloorNavigation;
        GameManager.instance.ChangeGameState(GameManager.GameState.PlayerStatus);
    }
}
