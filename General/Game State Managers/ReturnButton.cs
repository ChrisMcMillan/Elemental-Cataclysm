using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Returns player to main menu.
public class ReturnButton : MonoBehaviour {

	public void ReturnButtonEvent() {
        GameManager.instance.ChangeGameState(GameManager.GameState.MainMenu);
    }
}
