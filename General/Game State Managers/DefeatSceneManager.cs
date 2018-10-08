using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Displays a message to they player base on how they lose or won.
public class DefeatSceneManager : MonoBehaviour {

    public Text defeatText; 
	
    /*Displays message base on if the player won or lost. 
     If the player lost, then it displays a different message
     depending on how the player lost.*/
	void Start () {
		
        if(GameManager.instance.PlayerInfo.PlayerHasWonTheGame == true) {
            defeatText.text = "You are victorious!!!";
            defeatText.color = Color.yellow;
        }
        else if(GameManager.instance.PlayerInfo.SouldHasFaded == true) {
            defeatText.text = "Your soul has faded...";
            defeatText.color = ColorGenerator.GenerateColor(0, 128, 128);
        }
        else {
            defeatText.text = "You have been defeated!";
            defeatText.color = ColorGenerator.GenerateColor(255, 0, 0);
        }
	}

    //Use in animation event to take the player back to the main menu.
    public void ReturnToMainMenuEvent() {
        GameManager.instance.ChangeGameState(GameManager.GameState.MainMenu);
    }
}
