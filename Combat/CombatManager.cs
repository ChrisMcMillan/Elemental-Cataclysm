using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls logic during combat sections of the game. 
public class CombatManager : MonoBehaviour {

    public enum CombatPhase { Summoning, AbilityPick};

    CombatPhase _currentCombatPhase;

    public CombatAnnouncementText combatAnnouncementText;
    public Transform playerPosition;
    public Transform enemyPosition;
    public CombatButtonsManager combatButtonsMangerRef;
    public GameObject playerMonster;
    public GameObject enemyMonster;

    MonsterData _playerMonsterDataRef;
    MonsterData _enemyMonsterDataRef;

    bool _isPlayerTurn;
    bool _appliedAilments;
    bool _playerDeafeated;

    float _turnWaitTime = 4.0f;
    float _endOfCombatWaitTime = 12.0f;

    public CombatPhase CurrentCombatPhase {
        get {
            return _currentCombatPhase;
        }
    }

    // Use this for initialization
    void Start () {
        StartCombatEvent();
    }

    //initializes combat event.
    void StartCombatEvent() {

        _appliedAilments = false;
        playerMonster.SetActive(false);
        ChangeCombatPhase(CombatPhase.Summoning, true);

        InitEnemyMonsterData();
        AnnounceEnemyMonster();
    }

    //Gathers data about the current room the player is in and creates an enemy monster base on that data. 
    void InitEnemyMonsterData() {

        _enemyMonsterDataRef = new MonsterData();
        object newMosIdenity = 
            MonsterData.PickRandomMonsterIdenityOfElement(GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity);

        if (GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.MyRoomEncounterType == Room.RoomEncounterType.Boss) {

            UniqueBossData.UniqueBossIdentity uniBosIden = GameManager.instance.DugeonData.CurrentFloor.UniqueBossType;
            _enemyMonsterDataRef = UniqueBossData.FindUniqueBossDataType(uniBosIden);
            if (uniBosIden != UniqueBossData.UniqueBossIdentity.NONE) newMosIdenity = uniBosIden;
        }
        
        if(newMosIdenity.GetType() != typeof(UniqueBossData.UniqueBossIdentity)) _enemyMonsterDataRef.InitData(newMosIdenity,true,true);
        else _enemyMonsterDataRef.InitData(newMosIdenity,false,true);

        _enemyMonsterDataRef.ControlledLevelUp(GameManager.instance.DugeonData.CurrentFloor.FloorLevel);
        enemyMonster = UpdateMonsterSkeleton(enemyMonster, _enemyMonsterDataRef, true);
        if (GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.MyRoomEncounterType == Room.RoomEncounterType.Boss) _enemyMonsterDataRef.BigBossUpgrade();
    }

    //Display the name of the enemy monster, at the start of combat.
    void AnnounceEnemyMonster() {
        string annoText = "";

        if (GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.MyRoomEncounterType == Room.RoomEncounterType.Boss) annoText += "Big Boss ";
       
        annoText += MonsterData.GetMonsterIdentityString(_enemyMonsterDataRef.Identity);
        combatAnnouncementText.AddToAnnouncementQueue(new CombatAnnouncementText.AnnouncementTextInfo(annoText,
            Elemental.FindElementalColor(_enemyMonsterDataRef.MonsterElementalType)));
    }

    //Load a monster skeleton base on the monster's identity. 
    GameObject SwapSkeletons(object arg, GameObject oldSkeleton, Transform newPosition) {

        GameObject newSkeleton;
        newSkeleton = Instantiate(GameManager.instance.GameResorcesRef.FindSkeleton(arg), new Vector2(0, 0), Quaternion.identity);
        newSkeleton.transform.position = newPosition.position;
        newSkeleton.GetComponent<MonsterAnimator>().CorrectPosition();
        newSkeleton.transform.rotation = oldSkeleton.transform.rotation;
        newSkeleton.transform.parent = oldSkeleton.transform.parent;
        Destroy(oldSkeleton);

        return newSkeleton;
    }

    //Depending if it is a player monster or enemy monster, this function loads the monster in their respective position, on the battle field. 
    GameObject UpdateMonsterSkeleton(GameObject monsterSkeleton, MonsterData monsterData, bool isEnemyMonster) {

        Transform newPosition = (isEnemyMonster) ? enemyPosition : playerPosition;
        monsterSkeleton = SwapSkeletons(monsterData.Identity, monsterSkeleton, newPosition);
        monsterData.MyMosAnimatorRef = monsterSkeleton.GetComponent<MonsterAnimator>();
        if (isEnemyMonster) monsterData.MyMosAnimatorRef.FlipSkeleton();

        monsterSkeleton.SetActive(true);
      
        return monsterSkeleton;
    }

    //Depending on what combat phase the combat event is in, it changes the fuctionality of the combat buttons.
    //This fuction play the transition animation for the combat buttons and runs the appropriate logic for the phase. 
    void ChangeCombatPhase(CombatPhase newPhase, bool init = false) {
        _currentCombatPhase = newPhase;

        if (init == false) {
            combatButtonsMangerRef.MyAnimator.SetTrigger("Disappear");
            combatButtonsMangerRef.TransitioningPhases = true;
        }
        else {
            combatButtonsMangerRef.LoadPlayerMonsterPictures();
        }

        combatButtonsMangerRef.PlayerMonsterDataRef = _playerMonsterDataRef;
        combatButtonsMangerRef.CurrentCombatPhase = _currentCombatPhase;

        if (_currentCombatPhase == CombatPhase.AbilityPick) {
            DecideTurn();
        }
    }

    //Decides which monster goes frist base on their speed.  
    void DecideTurn()
    {
        int playerSpeedRoll = _playerMonsterDataRef.ObatainSpeedRoll();
        int enemySpeedRoll = _enemyMonsterDataRef.ObatainSpeedRoll();

        if (playerSpeedRoll > enemySpeedRoll)
        {
            _isPlayerTurn = true;
            combatButtonsMangerRef.FadeButtons(false);
        }
        else if (playerSpeedRoll == enemySpeedRoll) {
            int RNG = UnityEngine.Random.Range(1, 101);
            _isPlayerTurn = RNG <= 50 ? true : false;
            if (!_isPlayerTurn) {
                combatButtonsMangerRef.FadeButtons(true);
                StartCoroutine(EnemyPickAbility());
            }
            else {
                combatButtonsMangerRef.FadeButtons(false);
            }
        }
        else
        {
            _isPlayerTurn = false;
            combatButtonsMangerRef.FadeButtons(true);
            StartCoroutine(EnemyPickAbility());
        }

        CombatAnnouncementText.AnnouncementTextInfo temp;

        if (_isPlayerTurn) temp = new CombatAnnouncementText.AnnouncementTextInfo("Your turn.", Color.white);
        else temp = new CombatAnnouncementText.AnnouncementTextInfo("Enemy turn.", Color.white);

        combatAnnouncementText.AddToAnnouncementQueue(temp);
    }

    //Checks if the player or the enemy monster has lost. If so, it begins the end of combat event.  
    void CheckForEndOfCombat() {
  
        if(GameManager.instance.PlayerInfo.PlayerDefeated() || _enemyMonsterDataRef.IsDead) {
            CombatAnnouncementText.AnnouncementTextInfo temp;
            if (_enemyMonsterDataRef.IsDead) temp = new CombatAnnouncementText.AnnouncementTextInfo("Victory!", Color.yellow);
            else temp = new CombatAnnouncementText.AnnouncementTextInfo("Deafeat!", Color.red);

            combatAnnouncementText.AddToAnnouncementQueue(temp);

            _playerDeafeated = GameManager.instance.PlayerInfo.PlayerDefeated();

            _endOfCombatWaitTime = GameManager.instance.PlayerInfo.AfterCombatEvent(_playerMonsterDataRef, _enemyMonsterDataRef,combatAnnouncementText);

            StartCoroutine(EndCombatEvent());
        }

        else if (_playerMonsterDataRef.IsDead) {
            ChangeCombatPhase(CombatPhase.Summoning);
        }
    }

   //This function makes sure that the combat button animation is not playing. Then, lets player use a monster ability or summon a monster. 
    public void playerAbilityButtonClickEvent(int index){
        if (combatButtonsMangerRef.TransitioningPhases) return;

        if (_currentCombatPhase == CombatPhase.AbilityPick) UsePlayerMonsterAbility(index);
        else SummonPlayerMonster(index);
    }

    //Allow the player to switch to summoing phase of combat. 
    public void SummonButttonEvent() {
        if (!_isPlayerTurn || _enemyMonsterDataRef.IsDead || combatButtonsMangerRef.TransitioningPhases) return;
        ChangeCombatPhase(CombatPhase.Summoning);
    }

    //Lets the player use a monster ability during the ability pick phase of combat. 
    void UsePlayerMonsterAbility(int index) {
        if (!_isPlayerTurn || _playerMonsterDataRef.IsDead || _enemyMonsterDataRef.IsDead) return;

        _playerMonsterDataRef.Abilities[index].UseAbility(_playerMonsterDataRef, _enemyMonsterDataRef);

        _isPlayerTurn = false;
        combatButtonsMangerRef.FadeButtons(true);
        if (!_enemyMonsterDataRef.IsDead) StartCoroutine(EnemyPickAbility());
       
        CheckForEndOfCombat();
    }

    //This function frist vaildates the player's input then lets them summon a monster. 
    void SummonPlayerMonster(int index) {
        List<MonsterData> _playerMonstersRef = GameManager.instance.PlayerInfo.EquippedMonsters;

        if (index > _playerMonstersRef.Count - 1 || _playerMonstersRef[index].IsDead) return;

        _playerMonsterDataRef = _playerMonstersRef[index];
 
        playerMonster = UpdateMonsterSkeleton(playerMonster, _playerMonsterDataRef, false);

        ChangeCombatPhase(CombatPhase.AbilityPick);
    }

    //Lets the enemy monster pick their abilities. Then start the timer to switch to the player's turn. 
    void UseEnemyMonsterAbility() {

        EnemyMonsterPickAbility.PickAbility(_enemyMonsterDataRef, _playerMonsterDataRef);
   
        StartCoroutine(SwitchToPlayerTurn());
    }

    /* This function has delayed execution, meaing it is called after a specified amount time. First checks if ailements have
     been applied this turn. If not, then checks if they monster has any ailements currenly effecting them. If they don't  then 
     it lets monster pick an ability. But if they do have ailements effecting them, it calls the function again, starting delay 
     timer again. We need to do this because we need give time for hit animation to play, so the player has time to notice the 
     ailements are effecting the monster.*/
    IEnumerator EnemyPickAbility()
    {
        yield return new WaitForSeconds(_turnWaitTime);

        if (!_appliedAilments) {
            _appliedAilments = true;

            if (_enemyMonsterDataRef.AppplyAilmentEffects()) {
                CheckForEndOfCombat();

                if (_enemyMonsterDataRef.CheckForMatchingAilment(StatusAilment.StatusAilmentType.Stun) == true) {
                    _appliedAilments = false;
                    StartCoroutine(SwitchToPlayerTurn());
                }
                else if (!_enemyMonsterDataRef.IsDead) {
                    StartCoroutine(EnemyPickAbility());
                }      
            }
            else {
                EnemyTurnShift();
            }   
        }
        else {
            EnemyTurnShift();
        }
       
        yield return null;
    }

    //Resets the check of ailments on monsters. Then switches turns. 
    void EnemyTurnShift() {
        _appliedAilments = false;
        UseEnemyMonsterAbility();
        CheckForEndOfCombat();
    }

    //Same as EnemyPickAbility() comment. 
    IEnumerator SwitchToPlayerTurn() {
        yield return new WaitForSeconds(_turnWaitTime);
        if (!_appliedAilments) {
            _appliedAilments = true;
            if (_playerMonsterDataRef.AppplyAilmentEffects()) {
                CheckForEndOfCombat();

                if (_playerMonsterDataRef.CheckForMatchingAilment(StatusAilment.StatusAilmentType.Stun) == true) {
                    _appliedAilments = false;
                    StartCoroutine(EnemyPickAbility());
                }
                else {
                    StartCoroutine(SwitchToPlayerTurn());
                }
            }
            else {
                PlayerTurnShift();
            }
        }
        else {
            PlayerTurnShift();
        }
        yield return null;
    }

    //Switches to player turn and fades in butttons.
    void PlayerTurnShift() {
       
        _appliedAilments = false;
        _isPlayerTurn = true;
        combatButtonsMangerRef.FadeButtons(false);
    }

    /*End combat event needs a delyed execution to give time to show all combat announcements.
     If they player has lost, then this function go to the defeat scene. 
     If the combat encounter is a boss encounter, then the this function goes to the portal scene.
     If none of the above is true, then this function just goes to the floor navigation scene. */
    IEnumerator EndCombatEvent() {
        yield return new WaitForSeconds(_endOfCombatWaitTime);

        Room playerRoom = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom;
        playerRoom.RoomEncounterCleared = true;

        if (_playerDeafeated) {
            GameManager.instance.PlayerInfo.SouldHasFaded = false;
            GameManager.instance.ChangeGameState(GameManager.GameState.Defeat);
        }

        else if (playerRoom.MyRoomEncounterType == Room.RoomEncounterType.Boss) {

            if(GameManager.instance.DugeonData.CurrentFloor.UniqueBossType == UniqueBossData.UniqueBossIdentity.Shifter) {
                GameManager.instance.PlayerInfo.PlayerHasWonTheGame = true;
                GameManager.instance.ChangeGameState(GameManager.GameState.Defeat);
            }
            else {
                playerRoom.ChangeRoomEncounterType(Room.RoomEncounterType.Portal);
                GameManager.instance.ChangeGameState(Room.RoomEncounterType.Portal);
            }
   
        }

        else {
            GameManager.instance.ChangeGameState(GameManager.GameState.FloorNavigation);
        }
        
        yield return null;
    }
}
