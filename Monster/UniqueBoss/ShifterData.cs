using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This boss can change its element type during combat and gains abilities of that element.*/
public class ShifterData : UniqueBossData {

    int _shiftCounter = 0;
    int _shiftCounterMax = 2;

    //Picks abilities of the boss's current elemental type. 
    override protected void PickMonsterAbilities() {
        PickRandomAbilitiesOfElement(_monsterElementalType);
    }

    /*Changes the boss's elemental type every two turns and gives it
     abilities of that element.*/
    public override void AfterAbilityUseEvent() {
        _shiftCounter++;

        if(_shiftCounter == _shiftCounterMax) {
            _monsterElementalType = Elemental.PickRandomElementalIdentity();
            PickRandomAbilitiesOfElement(_monsterElementalType);
            _shiftCounter = 0;

            CombatAnnouncementText.AnnouncementTextInfo temp =
                new CombatAnnouncementText.AnnouncementTextInfo("Shifting: " + _monsterElementalType.ToString(),
                Elemental.FindElementalColor(_monsterElementalType));

            MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }
    }
}
