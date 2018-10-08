using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Controls all data associated with player.  
public class PlayerData  {

    public const int MAX_MONSTER_EQUIP = 3;
    List<MonsterData> _equippedMonsters;
    List<MonsterData> _storedMonsters;
    List<TotemData> _playerTotems;
    MonsterData _lastMonsterInCombat;

    bool _souldHasFaded;
    bool _playerHasWonTheGame;

    int _soul;
    int _fireuneCount;
    int _waterRuneCount;
    int _windRuneCount;
    int _earthRuneCount;

    public List<MonsterData> EquippedMonsters {
        get {
            return _equippedMonsters;
        }
    }

    public List<MonsterData> StoredMonsters {
        get {
            return _storedMonsters;
        }
    }

    public int Soul {
        get {
            return _soul;
        }
        set {
            _soul = value;
        }
    }

    public bool SouldHasFaded {
        get {
            return _souldHasFaded;
        }

        set {
            _souldHasFaded = value;
        }
    }

    public List<TotemData> PlayerTotems {
        get {
            return _playerTotems;
        }
    }

    public bool PlayerHasWonTheGame {
        get {
            return _playerHasWonTheGame;
        }
        set {
            _playerHasWonTheGame = value;
        }
    }

    // Initializes all the player's data. 
    public void InitData(int soulArg) {

        _soul = soulArg;
        _equippedMonsters = new List<MonsterData>();
        _storedMonsters = new List<MonsterData>();
        _playerTotems = new List<TotemData>();
        _playerHasWonTheGame = false;
        _souldHasFaded = false;
        _fireuneCount = 0;
        _waterRuneCount = 0;
        _windRuneCount = 0;
        _earthRuneCount = 0;

        MonsterData newMonsterData;

        for (int i = 0; i < MAX_MONSTER_EQUIP; i++) {
            newMonsterData = new MonsterData();
            newMonsterData.InitData(MonsterData.PickRandomMonsterIdenity());
            _equippedMonsters.Add(newMonsterData);
        }

        //Totem testing
        //_playerTotems.Add(new TotemData(TotemData.TotemIdentity.Reclamation));
        /*foreach (TotemData.TotemIdentity value in TotemData.TotemIdentity.GetValues(typeof(TotemData.TotemIdentity))) {
            _playerTotems.Add(new TotemData(value));
        }*/
    }

    //Subtract's from the player's soul value. If soul becomes less then or equal to 0 then it returns true to indicate the player has lost.  
    public bool DrainSoul(int amount) {
        _soul -= amount;

        if(_soul <= 0) return true;
        
        return false;
    }

    //Checks if a player has a particular totem. 
    public bool PlayerHasTotem(TotemData.TotemIdentity arg) {
        for (int i = 0; i < _playerTotems.Count; i++) {
            if (_playerTotems[i].MyIdentity == arg) return true;
        }

        return false;
    }

    //Decides what player's rewards are after a battle and displays a message based the rewards earned. 
    public float AfterCombatEvent(MonsterData playerMonsterInCombat, MonsterData enemyMonster, CombatAnnouncementText combatAnnouncementText){

        CombatAnnouncementText.AnnouncementTextInfo temp;
        int soulReward;
        int expReward;
        float endOfCombatWaitTime;
        Room playerRoom = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom;

        if (playerRoom.MyRoomEncounterType == Room.RoomEncounterType.Boss) {
            soulReward = 4;
            expReward = enemyMonster.Level * 2;
        }

        else {
            if (PlayerHasTotem(TotemData.TotemIdentity.Runes)) soulReward = 2;
            else soulReward = UnityEngine.Random.Range(2,4);
            expReward = enemyMonster.Level;    
        }

        if(PlayerHasTotem(TotemData.TotemIdentity.Greed) && _lastMonsterInCombat == playerMonsterInCombat) {
            expReward += enemyMonster.Level / 2;
        }
        _lastMonsterInCombat = playerMonsterInCombat;

        _soul += soulReward;
        playerMonsterInCombat.RewardEXP(expReward);

        temp = new CombatAnnouncementText.AnnouncementTextInfo("+" + expReward + " Exp", Color.white);
        combatAnnouncementText.AddToAnnouncementQueue(temp);
        temp = new CombatAnnouncementText.AnnouncementTextInfo("+" + soulReward + " Soul", Color.white);
        combatAnnouncementText.AddToAnnouncementQueue(temp);

        temp = DecideRuneDrop(enemyMonster.MonsterElementalType);
        if(temp != null) combatAnnouncementText.AddToAnnouncementQueue(temp);

        endOfCombatWaitTime = (temp == null) ? 8.0f : 12.0f;

        RemoveDeadMonsters();
        SoftResetAllMonsters();

        //We don't want healing dismshing returns to apply to the boss after combat heal. 
        if (playerRoom.MyRoomEncounterType == Room.RoomEncounterType.Boss) HealAllMonsters(0.5f);
        if(PlayerHasTotem(TotemData.TotemIdentity.Bloodlust)) HealAllMonsters(0.2f);

        return endOfCombatWaitTime;
    }

    //Decides if the player get a rune reward after a battle. If so, it creates a message based on the amount and type earend. 
    CombatAnnouncementText.AnnouncementTextInfo DecideRuneDrop(Elemental.ElementalIdentity enemyMonsterElementType) {
        int RNG = UnityEngine.Random.Range(1, 101);
        string rewardMessage = "";
        CombatAnnouncementText.AnnouncementTextInfo temp;
        int runeReward = 1;

        if (PlayerHasTotem(TotemData.TotemIdentity.Runes)) runeReward *= 2;

        if (RNG > 60) return null;

        rewardMessage = "+" + runeReward.ToString() + " ";

        switch (enemyMonsterElementType) {
            case Elemental.ElementalIdentity.Earth: _earthRuneCount += runeReward; rewardMessage += Elemental.ElementalIdentity.Earth.ToString(); break;
            case Elemental.ElementalIdentity.Fire: _fireuneCount += runeReward; rewardMessage += Elemental.ElementalIdentity.Fire.ToString(); break;
            case Elemental.ElementalIdentity.Wind: _windRuneCount += runeReward; rewardMessage += Elemental.ElementalIdentity.Wind.ToString(); break;
            case Elemental.ElementalIdentity.Water: _waterRuneCount += runeReward; rewardMessage += Elemental.ElementalIdentity.Water.ToString(); break;
            default: Debug.LogError("Error: Rune reward for " + enemyMonsterElementType + " is not defined."); break;
        }

        rewardMessage += " Rune";

        temp = new CombatAnnouncementText.AnnouncementTextInfo(rewardMessage, Elemental.FindElementalColor(enemyMonsterElementType));

        return temp;
    }

    //Removes all dead monsters from the player's party. 
    void RemoveDeadMonsters() {
        int count = _equippedMonsters.Count - 1;

        while(count >= 0) {
            if (_equippedMonsters[count].IsDead) {
                if (PlayerHasTotem(TotemData.TotemIdentity.Reclamation)) _soul += 2;
             
                _equippedMonsters.RemoveAt(count);
            }

            count--;
        }
    } 

    //Does a soft reset on all monsters in the player's party.
    void SoftResetAllMonsters() {
        for (int i = 0; i < _equippedMonsters.Count; i++) {
            _equippedMonsters[i].SoftReset();
        }
    }

    //Checks if all the monsters the player's party are dead. 
    public bool PlayerDefeated() {

        int deadMonstersCount = 0;

        for (int i = 0; i < _equippedMonsters.Count; i++) {
            if (_equippedMonsters[i].IsDead) deadMonstersCount++;
        }

        if (deadMonstersCount == _equippedMonsters.Count) return true;

        return false;
    }

    //Heals all monsters in the player's party base on a percentage of their max health. 
    public void HealAllMonsters(float healingPercent) {
        int healingAmount = 0;
        for (int i = 0; i < _equippedMonsters.Count; i++) {
            healingAmount = Mathf.FloorToInt(_equippedMonsters[i].MaxHealth * healingPercent);
            _equippedMonsters[i].ApplyHealing(healingAmount);
        }
    }

    //Updates the rune count in the player's UI. 
    public void UpdatePlayerRuneImages(List<Text> textGroup, List<Image> imageGroup) {
        int count = 0;

        foreach (Elemental.ElementalIdentity item in Elemental.ElementalIdentity.GetValues(typeof(Elemental.ElementalIdentity))) {
            if (item == Elemental.ElementalIdentity.NONE) continue;

            switch (item) {
                case Elemental.ElementalIdentity.Wind:
                    textGroup[count].text = _windRuneCount.ToString(); break;
                case Elemental.ElementalIdentity.Earth: textGroup[count].text = _earthRuneCount.ToString(); break;
                case Elemental.ElementalIdentity.Water: textGroup[count].text = _waterRuneCount.ToString(); break;
                case Elemental.ElementalIdentity.Fire: textGroup[count].text = _fireuneCount.ToString(); break;
                default: Debug.LogError("Error: no rune count for: " + item); break;
            }

            imageGroup[count].sprite = GameManager.instance.GameResorcesRef.FindRuneImage(item);
            textGroup[count].color = Elemental.FindElementalColor(item);
            count++;
        }
    }

    //Use to add and remove monsters from storage. Makes sure that the either group is not full before transfer. 
    public bool AddMonsterToStorage(int index, bool adding) {

        if (adding) {
            if (_storedMonsters.Count == MAX_MONSTER_EQUIP) return false;
            _storedMonsters.Add(_equippedMonsters[index]);
            _equippedMonsters.RemoveAt(index);
        }

        else {
            if (_equippedMonsters.Count == MAX_MONSTER_EQUIP) return false;
            _equippedMonsters.Add(_storedMonsters[index]);
            _storedMonsters.RemoveAt(index);
        }

        return true;
    }

    //Makes sure that the player has enough runes and room for a new monster. 
    public string PurchaseMonster(int index, List<MonsterData> beastMasterMonsters) {

        Elemental.ElementalIdentity playerRoomElement = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity;
        string notEnoughtRunesErrowMessage = "Not enough runes.";
        int runeCost;

        if (beastMasterMonsters[index].MonsterElementalType == playerRoomElement) runeCost = 0;
        else runeCost = beastMasterMonsters[index].FindMonsterRuneCost();

        if (_equippedMonsters.Count < MAX_MONSTER_EQUIP) {
            if (SpendRunes(runeCost, beastMasterMonsters[index].MonsterElementalType) == false) return notEnoughtRunesErrowMessage;

            _equippedMonsters.Add(beastMasterMonsters[index]);
            beastMasterMonsters.RemoveAt(index); 
        }
        else if(_storedMonsters.Count < MAX_MONSTER_EQUIP) {
            if (SpendRunes(runeCost, beastMasterMonsters[index].MonsterElementalType) == false) return notEnoughtRunesErrowMessage;

            _storedMonsters.Add(beastMasterMonsters[index]);    
            beastMasterMonsters.RemoveAt(index);   
        }
        else {
            return "Not enough room for monster.";
        }

        return "";
    }

    //Makes sure that the player has enough runes to spend.  
    public bool SpendRunes(int amount, Elemental.ElementalIdentity elementalType) {

        switch (elementalType) {
            case Elemental.ElementalIdentity.Wind:
                if ((_windRuneCount - amount) < 0) return false;
                else _windRuneCount -= amount;
                break;
            case Elemental.ElementalIdentity.Earth:
                if ((_earthRuneCount - amount) < 0) return false;
                else _earthRuneCount -= amount;
                break;
            case Elemental.ElementalIdentity.Water:
                if ((_waterRuneCount - amount) < 0) return false;
                else _waterRuneCount -= amount;
                break;
            case Elemental.ElementalIdentity.Fire:
                if ((_fireuneCount - amount) < 0) return false;
                else _fireuneCount -= amount;
                break;
            default: Debug.LogError("Error: Elemental type not found."); break;
        }

        return true;
    }
}
