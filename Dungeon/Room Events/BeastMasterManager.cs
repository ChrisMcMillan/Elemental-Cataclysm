using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A Beast Master is room encounter where a player can obtain new monsters and add/withdraw monsters from storage.  
public class BeastMasterManager : MonoBehaviour {

    public CombatAnnouncementText announcementText;
    public List<MonsterInfoPanel> playerMonstersInfoPanels;
    public List<MonsterInfoPanel> beastMasterMontersInfoPanels;
    public List<MonsterInfoPanel> storedMonstersInfoPanels;
    public List<Text> beastMasterMonsterCostsText;
    public Text roomElementText;
    public RuneDisplay playerRuneDisplay;

	// Use this for initialization
	void Start () {

        UpdateAll();
    }

    //Allows the player to leave the room encounter. Checks if the player has monsters in their party. 
    public void ExitButtonEvent() {

        if(GameManager.instance.PlayerInfo.EquippedMonsters.Count == 0) {

            CombatAnnouncementText.AnnouncementTextInfo text = new CombatAnnouncementText.AnnouncementTextInfo("No monsters in party!", Color.red);
            announcementText.AddToAnnouncementQueue(text);

            return;
        }
       
        GameManager.instance.ChangeGameState(GameManager.GameState.FloorNavigation);
    }

    //Update all the user interface information. 
    void UpdateAll() {
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.StoredMonsters, storedMonstersInfoPanels);
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.DugeonData.CurrentFloor.BeastMasterMonsters, beastMasterMontersInfoPanels);

        UpdateBeastMasterMonsterCosts();
        playerRuneDisplay.UpdateRuneDisplay();
        RoomEventCommonFunctions.UpdateRoomElementText(roomElementText);
    }

    //Send a monster in the player's party into storage. 
    public void StorePlayerMonsterEvent(int index) {
        if (GameManager.instance.PlayerInfo.AddMonsterToStorage(index,true)) {
            RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
            RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.StoredMonsters, storedMonstersInfoPanels); 
        }
    }

    //Removes a monster from stoarge and places it in player party. 
    public void RemoveFromStorageEvent(int index) {
        if (GameManager.instance.PlayerInfo.AddMonsterToStorage(index, false)) {
            RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
            RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.StoredMonsters, storedMonstersInfoPanels);
        }
    }

    /*Updates the cost of obtaining new monsters. If the monster's elemental type matches that of room, then
     it cost 0 runes to obtain. Otherwise, the cost is half of the current floor level, using ceiling division.*/
    void UpdateBeastMasterMonsterCosts() {
        List<MonsterData> beastMasterMonsters = GameManager.instance.DugeonData.CurrentFloor.BeastMasterMonsters;
        Elemental.ElementalIdentity playerRoomElement = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity;
        int runeCost;

        //Hide all text
        for (int i = 0; i < beastMasterMonsterCostsText.Count; i++) {
            beastMasterMonsterCostsText[i].text = "";
        }

        for (int j = 0; j < beastMasterMonsters.Count; j++) {

            if(beastMasterMonsters[j].MonsterElementalType == playerRoomElement) runeCost = 0;
            else runeCost = beastMasterMonsters[j].FindMonsterRuneCost();

            beastMasterMonsterCostsText[j].text = runeCost.ToString();
            beastMasterMonsterCostsText[j].color = Elemental.FindElementalColor(beastMasterMonsters[j].MonsterElementalType);
        }
    }

    //Buys a monster if the player has enough runes and room in their party or storage. 
    public void PurchaseMonsterEvent(int index) {
        string errorMessage = GameManager.instance.PlayerInfo.PurchaseMonster(index, GameManager.instance.DugeonData.CurrentFloor.BeastMasterMonsters);

        if(errorMessage == "") {
            UpdateAll();
        }
        else {
            CombatAnnouncementText.AnnouncementTextInfo text = new CombatAnnouncementText.AnnouncementTextInfo(errorMessage, Color.red);
            announcementText.AddToAnnouncementQueue(text);
        }
    }
}
