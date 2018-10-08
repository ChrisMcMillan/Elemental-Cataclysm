using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This boss regens health every time it attacks unless it takes fire damage.
public class EverLastingData : UniqueBossData {

    float _regenRate = 0.075f;

    int _maxBurntCount = 3;
    int _curBurntCount = 3;

    override protected void PickMonsterAbilities() {
        _abilities[0] = Ability.ObatainAbilityData(Ability.AbilityIdentity.Carnivorous);
        _abilities[1] = Ability.ObatainAbilityData(Ability.AbilityIdentity.Fervour);
        _abilities[2] = Ability.ObatainAbilityData(Ability.AbilityIdentity.Lifebloom);
    }

    //The boss stops regenerating health if it has taken fire damage recently.
    protected override void SpeicalTakeDamgeEvent(Ability abilityData) {

        if (abilityData.AbilityElmentalType == Elemental.ElementalIdentity.Fire) {
            _curBurntCount = 0;
            CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo("Burnt!", Color.red);
            MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }
    }

    //The boss regen health after it attacks.
    public override void AfterAbilityUseEvent() {
        if (_curBurntCount == _maxBurntCount) {

            int regenHealth = Mathf.RoundToInt(this.MaxHealth * _regenRate);
            ApplyHealing(regenHealth, false, Elemental.NeutralMod, true);

            CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo("Regrowth +" + regenHealth,
                Color.green);
            MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }
        else {
            _curBurntCount++;
        }
    }
}
