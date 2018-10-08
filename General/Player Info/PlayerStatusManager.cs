using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Controles the user interface elemenets in the player status scene.*/
public class PlayerStatusManager : MonoBehaviour {

    public Text soulCountText;
    public List<MonsterInfoPanel> monsterInfoPanels;
    public InputField nameChangeInputField;
    public GameObject nameChangeUI;
    public Image screenFade;

    int monsterNameChangeIndex;

	// Use this for initialization
	void Start () {
        AdjustScreenFade(0.0f);
        nameChangeUI.SetActive(false);
        soulCountText.text = "Soul: " + GameManager.instance.PlayerInfo.Soul;
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, monsterInfoPanels);
    }

    //When they player is picking a new name for a monster, this function fades out the background.
    void AdjustScreenFade(float newAlpha) {
        Color c = screenFade.color;
        c.a = newAlpha;
        screenFade.color = c;

        if(newAlpha > 0.0f) {
            screenFade.gameObject.SetActive(true);
        }
        else {
            screenFade.gameObject.SetActive(false);
        }
    }

    //Return the player to the pervious scene. 
    public void ReturnButtonEvent() {   
        GameManager.instance.ChangeGameState(GameManager.instance.PreviousGameState);
    }

    //Takes the player to their totem inventory. 
    public void TotemsButtonEvent() {
        GameManager.instance.ChangeGameState(GameManager.GameState.TotemInventory);
    }


    //When the player touches a monter panel, this function hides the rest of the monster panels and displays the text input for the new name.
    public void NameChangeSelectionEvent(int index) {
        monsterNameChangeIndex = index;

        for (int i = 0; i < monsterInfoPanels.Count; i++) {
            if(i != monsterNameChangeIndex) monsterInfoPanels[i].gameObject.SetActive(false);
        }

        AdjustScreenFade(0.5f);
        nameChangeUI.SetActive(true);
    }

    //After the player inputs a new name for the monster's, this function returns the user interface back to its normal state. 
    public void NameChangeEvent() {
        MonsterData monsterData = GameManager.instance.PlayerInfo.EquippedMonsters[monsterNameChangeIndex];

        if (nameChangeInputField.text != "") {
            monsterData.Name = nameChangeInputField.text;
            nameChangeInputField.text = "";
        }

        AdjustScreenFade(0.0f);
        nameChangeUI.SetActive(false);
    
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, monsterInfoPanels);
    }
}
