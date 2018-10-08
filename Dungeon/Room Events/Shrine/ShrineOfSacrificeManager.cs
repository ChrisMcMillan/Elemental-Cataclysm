using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The Shrine of Scacrifice allows the player to scacrifice a monster for soul.
public class ShrineOfSacrificeManager : ShrineManager {

    public Text playerSoulCountText;
    public Text soulRewardCountText;

    // Use this for initialization
    void Start() {
        UpdateAllBase();
    }

    //Updates the player's soul count.
    void UpdatePlayerSoulCountText() {
        int playerSoulCount = GameManager.instance.PlayerInfo.Soul;
        playerSoulCountText.text = "Soul: " + playerSoulCount;
    }

    //Shows how much soul the player will get if they scacrifice a monster.
    void UpdateSoulRewardCountText() {
        string temp = "Sould Reward: ";

        if(shireMonsterData == null) {
            temp += 0;
        }
        else {
            temp += FindSoulReward(GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity);
        }

        soulRewardCountText.text = temp;
    }

    //If the shrine monster's elemental type matches the room's elemental type, then player will get more souls.
    int FindSoulReward(Elemental.ElementalIdentity roomElement) {
        int soulReward;

        if(shireMonsterData.MonsterElementalType == roomElement) {
            soulReward = 10;
        }
        else {
            soulReward = 5;
        }

        return soulReward;
    }

    protected override void SpecialUpdate() {
        UpdatePlayerSoulCountText();
        UpdateSoulRewardCountText();
    }

    //Stops the player from preforming a scacrifice if they have no monsters in their party.
    protected override bool SpecialMainEventBlocker() {

        if (GameManager.instance.PlayerInfo.EquippedMonsters.Count == 0) return true;

        return false;
    }

    //Rewards the player soul and deletes the shirne monster.
    protected override void MainEvent(Elemental.ElementalIdentity roomElement) {

        GameManager.instance.PlayerInfo.Soul += FindSoulReward(roomElement);
        shireMonsterData = null;
    }
}
