using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The shrine of power allows player to give permanent attribute bonus to a monster.
public class ShrineOfPowerManager : ShrineManager {

    public Text statBonusText;

	// Use this for initialization
	void Start () {
        UpdateAllBase();
    }

    //Updates the text that tells the player how much of a attribute bonus a monster will receive. 
    void UpdateStatBonusText() {
        Elemental.ElementalIdentity roomElement = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity;
        int floorLevel = GameManager.instance.DugeonData.CurrentFloor.FloorLevel;
        int minRoll = floorLevel - 2;
        string temp = "Stat Bonus:";

        if (minRoll < 1) minRoll = 1;

        temp += minRoll + "-" + (floorLevel + 2);
        statBonusText.color = Elemental.FindElementalColor(roomElement);
        statBonusText.text = temp;
    }

    //Updates the text that tells the player how much of a attribute bonus a monster will receive. 
    protected override void SpecialUpdate() {
        UpdateStatBonusText();
    }

    //Gives the shrine monster an attribute bonus.
    protected override void MainEvent(Elemental.ElementalIdentity roomElement) {
        shireMonsterData.GainPower(roomElement);
    }
}
