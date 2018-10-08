using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Shrines are special room encounters that allow the player to modify a monster's attributes.
 This class hold all the basic functionality of all the diffent shrine types.*/
public class ShrineManager : MonoBehaviour {

    public GameObject mainEventButton;
    public Text roomElementText;
    public MonsterInfoPanel shirneMonsterPanel;
    public List<MonsterInfoPanel> playerMonstersInfoPanels;

    protected MonsterData shireMonsterData;

    // Use this for initialization
    void Start () {
        Debug.LogError("Error: ShrineManager Start() should not be called. Please mono remove from gameobject.");
    }

    //Updates all information displayed in a shrine 
    protected void UpdateAllBase() {
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
        UpdateShireMonsterPanel();
        RoomEventCommonFunctions.UpdateRoomElementText(roomElementText);
        UpdateMainEventButton();
        SpecialUpdate();
    }

    //Updates shrine monster information.
    void UpdateShireMonsterPanel() {
        shirneMonsterPanel.gameObject.SetActive(false);

        if(shireMonsterData != null) {
            shireMonsterData.UpdateMonsterInfoPanel(shirneMonsterPanel);
            shirneMonsterPanel.gameObject.SetActive(true);
        }
    }

    /*Moves a monster from the player's party to the shire monster slot.
     If there is already a monster in the shire, then they swap places.*/
    public void AddMonsterToShrineEvent(int index) {
        MonsterData temp;

        if (shireMonsterData == null) {
            shireMonsterData = GameManager.instance.PlayerInfo.EquippedMonsters[index];
            GameManager.instance.PlayerInfo.EquippedMonsters.RemoveAt(index);
        }
        else {
            temp = shireMonsterData;
            shireMonsterData = GameManager.instance.PlayerInfo.EquippedMonsters[index];
            GameManager.instance.PlayerInfo.EquippedMonsters.RemoveAt(index);
            GameManager.instance.PlayerInfo.EquippedMonsters.Add(temp);
        }

        UpdateAllBase();
    }

    //When the player clicks the shire monster, this function places it back into their party. 
    public void RemoveMonsterFromShrineEvent() {
        GameManager.instance.PlayerInfo.EquippedMonsters.Add(shireMonsterData);
        shireMonsterData = null;

        UpdateAllBase();
    }

    //Allows the player to exit the shrine.
    public void ExitButtonEvent() {
        if (shireMonsterData != null) {
            GameManager.instance.PlayerInfo.EquippedMonsters.Add(shireMonsterData);
            shireMonsterData = null;
        }

        GameManager.instance.ChangeGameState(GameManager.GameState.FloorNavigation);
    }

    /*The main button they player uses to modify the shrine creature. 
     It fades in and out depending on if the player can use the button or not.*/
    void UpdateMainEventButton() {
        Image imageRef = mainEventButton.GetComponent<Image>();
        Text text = mainEventButton.GetComponentInChildren<Text>();

        Color c = imageRef.color;
        c.a = (CanActivateMainEvent()) ? 1.0f : 0.5f;
        imageRef.color = c;

        c = text.color;
        c.a = (CanActivateMainEvent()) ? 1.0f : 0.5f;
        text.color = c;
    }

    //Checks the player can use the main event button or not.
    bool CanActivateMainEvent() {

        if (shireMonsterData == null || GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomEncounterCleared == true) return false;
        if (SpecialMainEventBlocker() == true) return false;

        return true;
    }

    //Used by the shire subclasses to preform speical updates. 
    protected virtual void SpecialUpdate() {

    } 

    //Used by the shire subclasses to block using the main event button.
    protected virtual bool SpecialMainEventBlocker() {
        return false;
    }

    //Used by the shire subclasses to define what their main event button does.
    protected virtual void MainEvent(Elemental.ElementalIdentity roomElement) {
        
    }

    //Activates the shrie's main event. 
    public void MainButtonEvent() {
        if (CanActivateMainEvent() == false) return;

        Elemental.ElementalIdentity roomElement = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity;

        MainEvent(roomElement);

        GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomEncounterCleared = true;
        UpdateAllBase();
    }
}
