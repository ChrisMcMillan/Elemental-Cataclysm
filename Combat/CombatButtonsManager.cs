using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Switchs between displaying information about the player's party and displaying a monster's abilites. 
public class CombatButtonsManager : MonoBehaviour {

    public CombatButton[] combatButtonArray;
    public GameObject summonButton;

    Animator _myAnimator;
    bool _transitioningPhases = false;
    CombatManager.CombatPhase _currentCombatPhase;
    MonsterData _playerMonsterDataRef;

    public Animator MyAnimator {
        get {
            return _myAnimator;
        }
    }

    public CombatManager.CombatPhase CurrentCombatPhase {
        get {
            return _currentCombatPhase;
        }

        set {
            _currentCombatPhase = value;
        }
    }

    public MonsterData PlayerMonsterDataRef {
        get {
            return _playerMonsterDataRef;
        }

        set {
            _playerMonsterDataRef = value;
        }
    }

    public bool TransitioningPhases {
        get {
            return _transitioningPhases;
        }

        set {
            _transitioningPhases = value;
        }
    }

    // Use this for initialization
    void Start() {
        _myAnimator = GetComponent<Animator>();
        summonButton.SetActive(false);
    }

    //Called by animation event, at the end of the dispear animation.
    public void DecideNewLoadOutEvent() {
        if (_currentCombatPhase == CombatManager.CombatPhase.Summoning) {
            LoadPlayerMonsterPictures();    
        }
        else if (_currentCombatPhase == CombatManager.CombatPhase.AbilityPick) {
            LoadMonsterAbilityIcons(_playerMonsterDataRef);

        }

        _transitioningPhases = false;
    }

    //Load the information about the monsters in the player's party. 
    public void LoadPlayerMonsterPictures() {
        List<MonsterData> playerMonstersRef = GameManager.instance.PlayerInfo.EquippedMonsters;
        
        summonButton.SetActive(false);

        for (int i = 0; i < combatButtonArray.Length; i++) {

            if (i < playerMonstersRef.Count) {
                if (playerMonstersRef[i].IsDead == false) playerMonstersRef[i].UpdateCombatButton(combatButtonArray[i]);
                else combatButtonArray[i].LoadNoneIcon();
            }
            else {
                combatButtonArray[i].LoadNoneIcon();
            }
        }
    }

    //Load the abilites of the monster who is currently fighting. 
    public void LoadMonsterAbilityIcons(MonsterData playerMonster) {

        summonButton.SetActive(true);

        for (int i = 0; i < combatButtonArray.Length; i++) {
            combatButtonArray[i].LoadAbilityIcon((Ability.AbilityIdentity)playerMonster.Abilities[i].MyAbilityIdentity);
        }
    }

    //When it is not the player's turn, fade out the combat buttons. Vice versa.  
    public void FadeButtons(bool arg) {
        Image imageRef;
        Color c;

        for (int i = 0; i < combatButtonArray.Length; i++) {
            imageRef = combatButtonArray[i].GetComponent<Image>();
            c = imageRef.color;
            c.a = (arg) ? 0.5f : 1.0f;
            imageRef.color = c;
        }

        imageRef = summonButton.GetComponent<Image>();
        c = imageRef.color;
        c.a = (arg) ? 0.5f : 1.0f;
        imageRef.color = c;
    }
}
