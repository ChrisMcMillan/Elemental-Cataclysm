using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This gains stacks of burning rage, that increase the damage of its attacks. If 
 this boss takes water damage, then the stacks are reset to 0.*/
public class IgnisData : UniqueBossData {

    int _burningRageStack = 0;
    float _burningRageDamageMod = 0.10f;

    override protected void PickMonsterAbilities() {
        _abilities[0] = Ability.ObatainAbilityData(Ability.AbilityIdentity.Molten_Strike);
        _abilities[1] = Ability.ObatainAbilityData(Ability.AbilityIdentity.Lava_Lash);
        _abilities[2] = Ability.ObatainAbilityData(Ability.AbilityIdentity.Magma_Fury);
    }

    //This boss gains a stack of buring rage after it uses an ability.
    public void IncrementBuringRageStack() {
        _burningRageStack++;
    }

    //The more stacks of buring rage the boss has, the more damage it will do. 
    public float CaculateBuringRageDamageBonus(float effect) {
        float damageBonus = _burningRageDamageMod * _burningRageStack;
        //Debug.Log("Burning rage damage bonus " + damageBonus);
        //Debug.Log("Old effect number: " + effect);
        effect += effect * damageBonus;
        //Debug.Log("New effect number: " + effect);
        return effect;
    }

    /*Displays the current number of buring rage stacks that the boss has 
    as an announcement text.*/
    public void AnnounceBuringRageStacks() {
        string textString = "Buring Rage " + _burningRageStack.ToString();
        CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo(textString, Color.red);
        MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
    }

    //If this boss takes water damage, then the buring rage stacks will be reset.
    protected override void SpeicalTakeDamgeEvent(Ability abilityData) {

        if(abilityData.AbilityElmentalType == Elemental.ElementalIdentity.Water) {
            _burningRageStack = 0;
            CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo("Chill out", Color.blue);
            MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
            AnnounceBuringRageStacks();
        }
    }

    //Applies the buring rage damage bonus to the boss's next ability.
    public override float PreAbilityUseEvent(MonsterData user, MonsterData target, Ability.AbilityEffectData effectData) {
        IgnisData ignisUser = (IgnisData)user;
        effectData.modifiedEffect = ignisUser.CaculateBuringRageDamageBonus(effectData.modifiedEffect);
        ignisUser.IncrementBuringRageStack();
        return effectData.modifiedEffect;
    }

    public override void AfterAbilityUseEvent() {
        AnnounceBuringRageStacks();
    }
}
