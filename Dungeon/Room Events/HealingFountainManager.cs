using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A healing fountain is room encounter where the player can heal their monsters.
public class HealingFountainManager : MonoBehaviour {

    public GameObject healingButton;
    public Text healingPowerText;
    public List<MonsterInfoPanel> playerMonstersInfoPanels;
    public List<MonsterInfoPanel> healingMontersInfoPanels;
    public Text roomElementText;

    float _baseHealingPower = 0.25f;
    float _midHealingPower = 0.50f;
    float _maxHealingPower = 0.75f;
    float _currentHealingPower;
    List<MonsterData> _healingFountainMonsters = new List<MonsterData>();

    // Use this for initialization
    void Start () {

        RoomEventCommonFunctions.UpdateRoomElementText(roomElementText);
        UpdateHealingPower();
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
        RoomEventCommonFunctions.UpdateInfoPanelSet(_healingFountainMonsters, healingMontersInfoPanels);

    }

    //Places a monster, from they player's party, into the healing group.
    public void PlacePlayerMonsterIntoHealingEvent(int index) {

        List<MonsterData> playerMonsters = GameManager.instance.PlayerInfo.EquippedMonsters;

        _healingFountainMonsters.Add(playerMonsters[index]);
        playerMonsters.RemoveAt(index);

        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
        RoomEventCommonFunctions.UpdateInfoPanelSet(_healingFountainMonsters, healingMontersInfoPanels);
        UpdateHealingPower();
    }

    //Removes a monster from the healing group and places it back into the player's party.
    public void RemoveMonsterFromHealingEvent(int index) {

        List<MonsterData> playerMonsters = GameManager.instance.PlayerInfo.EquippedMonsters;

        playerMonsters.Add(_healingFountainMonsters[index]);
        _healingFountainMonsters.RemoveAt(index);

        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
        RoomEventCommonFunctions.UpdateInfoPanelSet(_healingFountainMonsters, healingMontersInfoPanels);
        UpdateHealingPower();
    }

   //Updates the healing power display in the user interface. 
    void UpdateHealingPower() {

        int biggestMatchCount = 0;
        int matchCount;
        string healingText = "Healing Power: ";

        _currentHealingPower = _baseHealingPower;

        foreach (Elemental.ElementalIdentity value in Elemental.ElementalIdentity.GetValues(typeof(Elemental.ElementalIdentity))) {
            if (value == Elemental.ElementalIdentity.NONE) continue;

            matchCount = 0;

            for (int i = 0; i < _healingFountainMonsters.Count; i++) {
                if (_healingFountainMonsters[i].MonsterElementalType == value) matchCount++;
            }

            if (matchCount > biggestMatchCount) biggestMatchCount = matchCount;
        }

        if (_healingFountainMonsters.Count > 1) {
            switch (biggestMatchCount) {
                case 1: _currentHealingPower = _maxHealingPower; healingText += "High"; break;
                case 2: _currentHealingPower = _midHealingPower; healingText += "Medium"; break;
                default: _currentHealingPower = _baseHealingPower; healingText += "Low"; break;
            }
        }
        else {
            healingText += "Low";
        }

        healingPowerText.text = healingText;
        UpdateHealingButton();
    }

    //Fades the healing button in and out, depending on if you are able to activate the event or not. 
    void UpdateHealingButton() {
        Image imageRef = healingButton.GetComponent<Image>();
        Text text = healingButton.GetComponentInChildren<Text>();

        Color c = imageRef.color;
        c.a = (HealEventVaild()) ? 1.0f : 0.5f;
        imageRef.color = c;

        c = text.color;
        c.a = (HealEventVaild()) ? 1.0f : 0.5f;
        text.color = c;
    }

    /*Prevents the player from using the healing fountain if they have no
     monsters in the healing group or if they have already used the healing fountain.*/
    bool HealEventVaild() {
        if (_healingFountainMonsters.Count == 0 || GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomEncounterCleared == true) return false;
        return true;
    }

    /*Updates how powerful the healing fountain's healing effect is. The healing power is determined by 
    how many monsters, in the healing group, do not have the same elemental type. For example, 
    if healing group was fire, water and earth; then healing effect would be at full power. On the other hand,
    if the healing group was fire, fire and fire; then healing efect would be at its lowest power.
    If a monster's elemental type, in the healing group, matches the room's elemental type, then they
    are healed to full health, regardless of the current healing power.*/
    public void HealButtonEvent() {
        if (HealEventVaild() == false) return;


        int healingAmount;
        Elemental.ElementalIdentity roomElement = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity;

        for (int i = 0; i < _healingFountainMonsters.Count; i++) {

            if (_healingFountainMonsters[i].MonsterElementalType == roomElement) {
                healingAmount = _healingFountainMonsters[i].MaxHealth;
            }
            else {
                healingAmount = Mathf.RoundToInt(_healingFountainMonsters[i].MaxHealth * _currentHealingPower);
            }
            
            _healingFountainMonsters[i].ApplyHealing(healingAmount);
        }

        GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomEncounterCleared = true;

        UpdateHealingPower();
        RoomEventCommonFunctions.UpdateInfoPanelSet(GameManager.instance.PlayerInfo.EquippedMonsters, playerMonstersInfoPanels);
        RoomEventCommonFunctions.UpdateInfoPanelSet(_healingFountainMonsters, healingMontersInfoPanels);
    }

    /*Allows the player to leave the room event. Makes sure the monsters in the healing group
     are automaticly place back into the player's party, before they leave.*/ 
    public void ExitButtonEvent() {

        List<MonsterData> playerMonsters = GameManager.instance.PlayerInfo.EquippedMonsters;
        int count = _healingFountainMonsters.Count - 1;

        while (count >= 0) {
            playerMonsters.Add(_healingFountainMonsters[count]);
            _healingFountainMonsters.RemoveAt(count);
            count--;
        }

        GameManager.instance.ChangeGameState(GameManager.GameState.FloorNavigation);
    }
}
