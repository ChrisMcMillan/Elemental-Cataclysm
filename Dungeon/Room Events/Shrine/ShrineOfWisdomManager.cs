using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The Shrine of Wisdom allows the player to reroll a monster's abilities. 
public class ShrineOfWisdomManager : ShrineManager {

    // Use this for initialization
    void Start () {
        UpdateAllBase();
	}

    //The shrine monster will learn new abilities base on the room's elemental type. 
    protected override void MainEvent(Elemental.ElementalIdentity roomElement) {
        shireMonsterData.GainWisdom(roomElement);
    }
}
