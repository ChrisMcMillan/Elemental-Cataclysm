using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A shrine where they player can exchange soul for leveling up a monster.
public class ShrineOfKnowledge : ShrineManager {

    public Text playerSoulCountText;
    public Text soulCostCountText;

    // Use this for initialization
    void Start () {
        UpdateAllBase();
    }

    //Updates the player's soul count.
    void UpdatePlayerSoulCountText() {
        int playerSoulCount = GameManager.instance.PlayerInfo.Soul;
        playerSoulCountText.text = "Soul: " + playerSoulCount;
    }

    /*If the monster, in the shire, elemental type matches the room's elemental type then 
    it cost no soul to level it up.*/
    int FindSoulCost() {
        int soulCost = 2;
        Elemental.ElementalIdentity roomElement = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity;

        if (shireMonsterData.MonsterElementalType == roomElement) soulCost = 0;

        return soulCost;
    }

    //Updates the soul cost display.
    void UpdateSoulCostText() {
        string temp = "Soul Cost: ";

        if(shireMonsterData == null) {
            soulCostCountText.text = temp;
            return;
        }
        else {
            temp += FindSoulCost();
            soulCostCountText.text = temp;
        }
    }

    //Updates soul cost and player's soul count display.
    protected override void SpecialUpdate() {
        UpdateSoulCostText();
        UpdatePlayerSoulCountText();
    }

    //Stops player from leveling the monster if they don't have enough soul to do it.
    protected override bool SpecialMainEventBlocker() {
        int remainingSoul = GameManager.instance.PlayerInfo.Soul - FindSoulCost();

        if (remainingSoul < 1) return true;

        return false;
    }

    //Subtracts the player's soul and levels up the monster.
    protected override void MainEvent(Elemental.ElementalIdentity roomElement) {
        GameManager.instance.PlayerInfo.Soul -= FindSoulCost();
        shireMonsterData.LevelUp();
    }
}
