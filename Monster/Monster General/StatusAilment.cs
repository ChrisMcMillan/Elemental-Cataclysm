using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A status ailment is an effect that is applied at the start of a monster's turn, during combat. 
 A monster gets a status from certain abilities that apply a status ailment. Status Ailments have
 stacks that determine how long they last in combat. One stack is subtracted at the start of a 
 monster's turn. Once a status ailment runs out of stacks, it will be removed from the monster.*/
public class StatusAilment {

    public enum StatusAilmentType {None, Poison, Burning, Regen, AttributeBuff, Stun};

    StatusAilmentType _alimentType;
    int _maxStacks;
    int _currentStacks;
    int _effectPerTick;

    public StatusAilmentType AlimentType {
        get {
            return _alimentType;
        }
    }

    public StatusAilment(StatusAilmentType identityArg, int stackCount, int totalAbilityEffect) {
        _alimentType = identityArg;
        _maxStacks = stackCount;
        _currentStacks = _maxStacks;
        _effectPerTick = totalAbilityEffect / _maxStacks;
    }

    /*Applies a status ailement's effect and subtracts a stack.*/
    public bool ApplyEffect(MonsterData mosData) {

        _currentStacks--;

        if(_alimentType == StatusAilmentType.Poison || _alimentType == StatusAilmentType.Burning) {

            mosData.SubatractHealth(_effectPerTick);
        }

        else if (_alimentType == StatusAilmentType.Regen) {
            
            mosData.ApplyHealing(_effectPerTick, false,Elemental.NeutralMod,true);
        }

        else if(_alimentType == StatusAilmentType.AttributeBuff) {
            if (_currentStacks == 0) {
                mosData.ResetAttributes();
            }
        }

        AnnoucemntText(mosData);

        if (_currentStacks == 0) return true;
        return false;
    }

    //Shows what ailment was just applied in an annoument text.   
    void AnnoucemntText(MonsterData mosData) {
        CombatAnnouncementText.AnnouncementTextInfo temp;
        string textOperator;
        Color textColor;

        if (_alimentType == StatusAilmentType.Regen) textOperator = "+";
        else textOperator = "-";

        if (_alimentType == StatusAilmentType.Burning) textColor = Color.red;
        else textColor = Color.green;

        if (_alimentType != StatusAilmentType.AttributeBuff && _alimentType != StatusAilmentType.Stun) {
            temp = new CombatAnnouncementText.AnnouncementTextInfo(textOperator + _effectPerTick + " " + _alimentType.ToString(), textColor);
            mosData.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }
        else if(_alimentType == StatusAilmentType.Stun && _currentStacks > 0) {
            temp = new CombatAnnouncementText.AnnouncementTextInfo("Stunned", Color.white);
            mosData.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }

        if (_currentStacks == 0) {
            string tempString;
            if (_alimentType == StatusAilmentType.AttributeBuff) tempString = "Stat bonus removed.";
            else tempString = _alimentType.ToString() + " removed.";

            temp = new CombatAnnouncementText.AnnouncementTextInfo(tempString, Color.white);
            mosData.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }
    }
}
