using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*An ability is special skill that a monster can use. This class controls all
 the data associated with abilities.*/
public class Ability{

    static int _standardEffect = 30;

    public enum AbilityIdentity {
        Venomn_Bite, Flame_Blast, Wind_Gust, Healing_Wave, Ice_Spear, Self_Immolation, Regrowth, Dash,
        Carnage, Boulder_Toss, Thunder_Bolt, Creeping_Madness, Molten_Strike, Lava_Lash, Magma_Fury, Carnivorous,
        Fervour, Lifebloom, Freeze, Thunder_Punch, Pollution, Stun_Breaker
    };

    public class AbilityEffectData {
      
        public float modifiedEffect;
        public float baseEffect;
        public float elementalEffecteMod;
        public bool landedCri = false;
    }

    AbilityIdentity _myAbilityIdentity;
    Elemental.ElementalIdentity _abilityElmentalType;
    StatusAilment.StatusAilmentType _abilityAlimentType;
    bool _isOffensiveAbility;
    bool _isSpell;
    int _minEffect;
    int _maxEffect;
    int _stackCount;

    public int MinEffect {
        get {
            return _minEffect;
        }
    }

    public int MaxEffect {
        get {
            return _maxEffect;
        }
    }

    public object MyAbilityIdentity {
        get {
            return _myAbilityIdentity;
        }
    }

    public Elemental.ElementalIdentity AbilityElmentalType {
        get {
            return _abilityElmentalType;
        }
    }

    public StatusAilment.StatusAilmentType AbilityAlimentType {
        get {
            return _abilityAlimentType;
        }
    }

    public bool IsOffensiveAbility {
        get {
            return _isOffensiveAbility;
        }
    }

    public void InitData(AbilityIdentity abilityIden, Elemental.ElementalIdentity elementalType,StatusAilment.StatusAilmentType abilityAlimentType, 
        float baseEffectMod,bool isOffensiveAbility, bool isSpell, int customStackCount = 0) {

        _myAbilityIdentity = abilityIden;
        _abilityElmentalType = elementalType;
        _abilityAlimentType = abilityAlimentType;
        _maxEffect = Mathf.RoundToInt(_standardEffect * baseEffectMod);
        _minEffect = Mathf.RoundToInt(_maxEffect * 0.8f);
        _isOffensiveAbility = isOffensiveAbility;
        _isSpell = isSpell;

        if (customStackCount > 0) _stackCount = customStackCount;
        else _stackCount = Mathf.RoundToInt(1.0f / baseEffectMod);
    }

    //Coverts an ability identity to string, to be used in a combat announcement. 
    public static string GetAbilityIdentityString(object arg) {
        string abilityString = arg.ToString();
        string temp = "";

        foreach (char c in abilityString) {      
            if (c == '_') temp += " ";
            else temp += c;
        }

        return temp;
    }

    /*Create an ability object base on the abiliity idenity pass in to the function.*/
    public static Ability ObatainAbilityData(AbilityIdentity value) {
       
        Ability ability;
        float directDamageMod = 1.2f;
        float normalDamageMod = 1.0f;
        float stunDamageMod = 0.8f;
        float directHealingMod = 2.0f;
        int attributeBuffCustomStackCount = 4;
        int stunDurationCustomStackCount = 2;

        ability = new Ability();

        switch (value) {
            case AbilityIdentity.Venomn_Bite: ability.InitData(AbilityIdentity.Venomn_Bite, Elemental.ElementalIdentity.Earth,
                StatusAilment.StatusAilmentType.Poison, 0.2f,true,false);
                break;
            case AbilityIdentity.Flame_Blast: ability.InitData(AbilityIdentity.Flame_Blast,Elemental.ElementalIdentity.Fire,
                StatusAilment.StatusAilmentType.Burning, 0.33f,true,true);
                break;
            case AbilityIdentity.Pollution: ability.InitData(AbilityIdentity.Pollution, Elemental.ElementalIdentity.Water,
                StatusAilment.StatusAilmentType.Poison, 0.33f, true, true);
                break;
            case AbilityIdentity.Carnage: ability.InitData(AbilityIdentity.Carnage, Elemental.ElementalIdentity.Fire,
                StatusAilment.StatusAilmentType.None, normalDamageMod, true,false);
                break;
            case AbilityIdentity.Carnivorous: ability.InitData(AbilityIdentity.Carnivorous, Elemental.ElementalIdentity.Earth,
                StatusAilment.StatusAilmentType.None, normalDamageMod, true, false);
                break;
            case AbilityIdentity.Healing_Wave: ability.InitData(AbilityIdentity.Healing_Wave, Elemental.ElementalIdentity.Water,
                StatusAilment.StatusAilmentType.None, directHealingMod, false,true);
                break;
            case AbilityIdentity.Lifebloom: ability.InitData(AbilityIdentity.Lifebloom, Elemental.ElementalIdentity.Earth,
                StatusAilment.StatusAilmentType.None, directHealingMod, false, true);
                break;
            case AbilityIdentity.Wind_Gust: ability.InitData(AbilityIdentity.Wind_Gust,Elemental.ElementalIdentity.Wind,
                StatusAilment.StatusAilmentType.None, directDamageMod, true,true);
                break;
            case AbilityIdentity.Stun_Breaker: ability.InitData(AbilityIdentity.Stun_Breaker, Elemental.ElementalIdentity.Wind,
                StatusAilment.StatusAilmentType.None, normalDamageMod, true, false);
                break;
            case AbilityIdentity.Thunder_Bolt: ability.InitData(AbilityIdentity.Thunder_Bolt, Elemental.ElementalIdentity.Wind,
                StatusAilment.StatusAilmentType.Burning, 0.5f, true, true);
                break;
            case AbilityIdentity.Ice_Spear: ability.InitData(AbilityIdentity.Ice_Spear, Elemental.ElementalIdentity.Water,
                StatusAilment.StatusAilmentType.None, directDamageMod, true, false);
                break;
            case AbilityIdentity.Boulder_Toss: ability.InitData(AbilityIdentity.Boulder_Toss, Elemental.ElementalIdentity.Earth,
                StatusAilment.StatusAilmentType.None, directDamageMod, true, false);
                break;
            case AbilityIdentity.Self_Immolation: ability.InitData(AbilityIdentity.Self_Immolation, Elemental.ElementalIdentity.Fire,
                StatusAilment.StatusAilmentType.None, 0.25f, false, true); break;
            case AbilityIdentity.Regrowth: ability.InitData(AbilityIdentity.Regrowth, Elemental.ElementalIdentity.Earth,
                StatusAilment.StatusAilmentType.Regen, 0.2f, false, true);
                break;
            case AbilityIdentity.Dash: ability.InitData(AbilityIdentity.Dash, Elemental.ElementalIdentity.Wind,
                StatusAilment.StatusAilmentType.AttributeBuff, normalDamageMod, true, false, attributeBuffCustomStackCount);
                break;
            case AbilityIdentity.Fervour: ability.InitData(AbilityIdentity.Fervour, Elemental.ElementalIdentity.Earth,
                StatusAilment.StatusAilmentType.AttributeBuff, normalDamageMod, true, false, attributeBuffCustomStackCount);
                break;
            case AbilityIdentity.Creeping_Madness: ability.InitData(AbilityIdentity.Creeping_Madness, Elemental.ElementalIdentity.Water,
                StatusAilment.StatusAilmentType.AttributeBuff, normalDamageMod, true, true, attributeBuffCustomStackCount);
                break;
            case AbilityIdentity.Molten_Strike: ability.InitData(AbilityIdentity.Molten_Strike, Elemental.ElementalIdentity.Fire,
                StatusAilment.StatusAilmentType.None, directDamageMod, true, false);
                break;
            case AbilityIdentity.Lava_Lash: ability.InitData(AbilityIdentity.Lava_Lash, Elemental.ElementalIdentity.Fire,
                StatusAilment.StatusAilmentType.Stun, stunDamageMod, true, true, stunDurationCustomStackCount);
                break;
            case AbilityIdentity.Freeze: ability.InitData(AbilityIdentity.Freeze, Elemental.ElementalIdentity.Water,
                StatusAilment.StatusAilmentType.Stun, stunDamageMod, true, true, stunDurationCustomStackCount);
                break;
            case AbilityIdentity.Thunder_Punch: ability.InitData(AbilityIdentity.Thunder_Punch, Elemental.ElementalIdentity.Wind,
                StatusAilment.StatusAilmentType.Stun, stunDamageMod, true, false, stunDurationCustomStackCount);
                break;
            case AbilityIdentity.Magma_Fury: ability.InitData(AbilityIdentity.Magma_Fury, Elemental.ElementalIdentity.Fire,
                StatusAilment.StatusAilmentType.AttributeBuff, normalDamageMod, true, false, attributeBuffCustomStackCount);
                break;
            default: Debug.LogError("Error: " + value + " has not been assigned data."); break;         
        }

        return ability;
    }

    /*Decides how the data associated with an ability is applied the monster that is using the ability and
     the target monster.*/
    void AppplyAbilitySpeicalEffect(MonsterData user, MonsterData target, AbilityEffectData effectData) {

        switch (_myAbilityIdentity) {
            case AbilityIdentity.Venomn_Bite: OffensiveAilment(user, target, effectData); break;
            case AbilityIdentity.Flame_Blast: OffensiveAilment(user, target, effectData); break;
            case AbilityIdentity.Pollution: OffensiveAilment(user, target, effectData); break;
            case AbilityIdentity.Thunder_Bolt: OffensiveAilment(user, target, effectData); break;
            case AbilityIdentity.Wind_Gust: DirectDamage(user, target, effectData); break;
            case AbilityIdentity.Healing_Wave: DirectHealing(user, target, effectData); break;
            case AbilityIdentity.Ice_Spear: DirectDamage(user, target, effectData); break;
            case AbilityIdentity.Self_Immolation: SelfImolation(user, target, effectData); break;
            case AbilityIdentity.Regrowth: HealingOverTime(user, target, effectData); break;
            case AbilityIdentity.Dash: OffensiveAttributeBuff(user, target, effectData); break;
            case AbilityIdentity.Creeping_Madness: OffensiveAttributeBuff(user, target, effectData); break;
            case AbilityIdentity.Carnage: LifeLeech(user, target, effectData); break;
            case AbilityIdentity.Boulder_Toss: DirectDamage(user, target, effectData); break;
            case AbilityIdentity.Molten_Strike: DirectDamage(user, target, effectData); break;
            case AbilityIdentity.Lava_Lash: OffensiveAilment(user, target, effectData); break;
            case AbilityIdentity.Freeze: OffensiveAilment(user, target, effectData); break;
            case AbilityIdentity.Thunder_Punch: OffensiveAilment(user, target, effectData); break;
            case AbilityIdentity.Magma_Fury: OffensiveAttributeBuff(user, target, effectData); break;
            case AbilityIdentity.Fervour: OffensiveAttributeBuff(user, target, effectData); break;
            case AbilityIdentity.Carnivorous: LifeLeech(user, target, effectData); break;
            case AbilityIdentity.Lifebloom: DirectHealing(user, target, effectData); break;
            case AbilityIdentity.Stun_Breaker: StunComboDamage(user, target, effectData); break;
            default: Debug.LogError("Error: Ability speical effect not found for " + _myAbilityIdentity); break;

        }
    }

    /*Each ability get an attribute damage bonus, base on its elemental type.*/
    int FindAttributeEffectBonus(MonsterData user) {
        switch (_abilityElmentalType) {
            case Elemental.ElementalIdentity.Earth: return user.CurAgility;
            case Elemental.ElementalIdentity.Fire: return user.CurStrength;
            case Elemental.ElementalIdentity.Water: return user.CurSpirit;
            case Elemental.ElementalIdentity.Wind: return user.CurSpeed;
            default: Debug.LogError("Error: Attribute effect bonus not define for " + _abilityElmentalType); return 0;
        }
    }

    /*This function calculates how much damage over time an ability should do to a target.
     The main idea behind effect over times abilites is that less damage an ability does 
     immediately, the more damage it does over time. Let's use vemon bite as an example. 
     Vemon bite only does 20% of 30 on it initial attack. However, it does 180% of 30 damage 
     over time from its posin effect. The 180% modifier is derived from taking 20% and subtracting
     it from 100%, giving 80%. We add 80% to 100% to give us the 180% modifier. This way the lower 
     initial damage, the more damage over time there is.*/
    StatusAilment CalculateStatusAilment(float effect) {
        float baseEfectModifer = (float)_maxEffect / (float)_standardEffect;
        float baseEffect = effect / baseEfectModifer;
        float baseOverTimeBonus = 1.0f +  (1.0f - baseEfectModifer);

        //Debug.Log("baseEfectModifer: " + baseEfectModifer + " baseEffect: " + baseEffect + " baseOverTimeBonus: " + baseOverTimeBonus);
        int totalAlimentEffect = Mathf.RoundToInt(baseEffect * baseOverTimeBonus);
        //Debug.Log("Total ailment effect: " + totalAlimentEffect);
        StatusAilment newStatusAilment = new StatusAilment(_abilityAlimentType, _stackCount, totalAlimentEffect);
        return newStatusAilment;
    }


    /*Caculates how ability effect the user and the target.*/
    public void UseAbility(MonsterData user, MonsterData target) {

        AbilityEffectData effectData = new AbilityEffectData();

        effectData.baseEffect = UnityEngine.Random.Range(_minEffect, _maxEffect);
        effectData.baseEffect += FindAttributeEffectBonus(user);

        effectData.elementalEffecteMod = (_isOffensiveAbility) ? Elemental.FindElementalModifier(target.MonsterElementalType, _abilityElmentalType) :
            Elemental.FindElementalModifier(user.MonsterElementalType, _abilityElmentalType);

        effectData.baseEffect *= effectData.elementalEffecteMod;

        effectData.modifiedEffect = effectData.baseEffect;

        /*When a monster uses the ability "Self Imolation" and when it removes buffs/debuffs from monster, the
         monster gets self imolation buff stacks, depending on the number of buffs/debuffs removed from the monster.
         These stacks increase the effect on the next ability used by the monster.*/
        if (user.SelfImolationBuffStacks > 0) {
            float removedAlimentsBonus = 0.25f * user.SelfImolationBuffStacks;
            effectData.modifiedEffect *= 2.4f + removedAlimentsBonus;
            user.SelfImolationBuffStacks = 0;
        }

        float criMod = user.ObatainCriMod();

        if (criMod > 0) {
            effectData.landedCri = true;
            effectData.modifiedEffect *= criMod;
        }

        if (user.IsEnemyMonster == true) effectData.modifiedEffect = CaculateEnemyDamgeMinagation(effectData.modifiedEffect);

        effectData.modifiedEffect = user.PreAbilityUseEvent(user, target, effectData);
        AppplyAbilitySpeicalEffect(user, target, effectData);
        user.AfterAbilityUseEvent();
    }

    /*Enemy monsters do less damage to the player's monster, during the early floors, but
     do more damage at later floors.*/
    float CaculateEnemyDamgeMinagation(float effect) {
        float dungeonCompletion = GameManager.instance.DugeonData.ObtainDungeonCompletionPercent();
        float damageMig = 0.3f;
        damageMig -= damageMig * dungeonCompletion;
        //Debug.Log("Damage mingation: " + damageMig.ToString("F2"));
        //Debug.Log("effect before: " + effect);
        effect -= effect * damageMig;
        //Debug.Log("effect after: " + effect);
        return effect;
    }
    
    //If a monster gets critacal hit, then this function displays message during combat to show that. 
    void AnnounceCriticalHit(MonsterData mosData, bool landedCri) {
        if (landedCri == false) return;

        CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo("Critical!",
           ColorGenerator.GenerateColor(255, 165, 0));

        mosData.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
    }

    //Deal direct damage to the target.
    void DirectDamage(MonsterData user, MonsterData target, AbilityEffectData effectData) {

        AnnounceCriticalHit(target, effectData.landedCri);
        target.MyMosAnimatorRef.abilityEffectAnimator.SetTrigger(_myAbilityIdentity.ToString());
        target.TakeDamage(Mathf.FloorToInt(effectData.modifiedEffect), this, effectData.elementalEffecteMod);
        user.PlayAbilityAnimation(_isSpell, this);
    }

    //Deal direct damage to the target and heal the user for a percentage of the damage.
    void LifeLeech(MonsterData user, MonsterData target, AbilityEffectData effectData) {

        user.ApplyHealing(Mathf.RoundToInt(effectData.modifiedEffect * 0.5f));

        DirectDamage(user, target, effectData);
    }

    //If the target is stuned, deal bonus damage, otherwise deal normal damage.
    void StunComboDamage(MonsterData user, MonsterData target, AbilityEffectData effectData) {

        if (target.CheckForMatchingAilment(StatusAilment.StatusAilmentType.Stun)) {
            //Debug.Log("Damage before stun combo bonus: " + effect);
            effectData.modifiedEffect *= 1.5f;
            //Debug.Log("Damage after stun combo bonus: " + effect);

            CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo("Bonus Damage!",
           ColorGenerator.GenerateColor(255, 165, 0));

            target.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }

        DirectDamage(user, target, effectData);
    }

    //Applies a damage over time effect the target.
    void OffensiveAilment(MonsterData user, MonsterData target, AbilityEffectData effectData) {
        
        target.AddStatusAilment(CalculateStatusAilment(effectData.baseEffect));
        DirectDamage(user, target, effectData);
    }

    //Buffs the user and deals direct damage to the target. 
    void OffensiveAttributeBuff(MonsterData user, MonsterData target, AbilityEffectData effectData) {
        string buffMessage;
        buffMessage = user.GrantTempAttributeBonus(this);
        user.AddStatusAilment(CalculateStatusAilment(effectData.baseEffect));
        user.MyMosAnimatorRef.abilityEffectAnimator.SetTrigger(_myAbilityIdentity.ToString());

        AnnounceCriticalHit(target, effectData.landedCri);
        target.TakeDamage(Mathf.FloorToInt(effectData.modifiedEffect), this, effectData.elementalEffecteMod);
        user.PlayAbilityAnimation(_isSpell, this);

        CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo(buffMessage,
           Elemental.FindElementalColor(_abilityElmentalType));
        user.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
    }

    //Applies a healing over time effect to the user.
    void HealingOverTime(MonsterData user, MonsterData target, AbilityEffectData effectData) {

        user.AddStatusAilment(CalculateStatusAilment(effectData.baseEffect));

        DirectHealing(user, target, effectData);
    }

    //Applies direct healing to the user.
    void DirectHealing(MonsterData user, MonsterData target, AbilityEffectData effectData) {

        user.MyMosAnimatorRef.abilityEffectAnimator.SetTrigger(_myAbilityIdentity.ToString());
        AnnounceCriticalHit(user, effectData.landedCri);
        user.PlayAbilityAnimation(_isSpell, this);
        user.ApplyHealing(Mathf.FloorToInt(effectData.modifiedEffect), true, effectData.elementalEffecteMod);
        user.IncramentHealingReductionStack();
    }

    /*Deals a small amount of fire damage to the user and removes all buff and debuff from the
     user. The user gains a self imolation buff stack for each buff and debuff removed. The user's
     next ability's effect is increased depending on how many stacks they recessive.*/
    void SelfImolation(MonsterData user, MonsterData target, AbilityEffectData effectData) {

        user.MyMosAnimatorRef.abilityEffectAnimator.SetTrigger(_myAbilityIdentity.ToString());
        AnnounceCriticalHit(user, effectData.landedCri);
        CombatAnnouncementText.AnnouncementTextInfo temp = new CombatAnnouncementText.AnnouncementTextInfo(GetAbilityIdentityString(_myAbilityIdentity),
            Elemental.FindElementalColor(_abilityElmentalType));
        user.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);

        user.SelfImolationBuffStacks = user.RemoveAllAilmentEffects();

        temp = new CombatAnnouncementText.AnnouncementTextInfo("Empowered X" + user.SelfImolationBuffStacks, Color.white);
        user.MyMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);

        user.TakeDamage(Mathf.FloorToInt(effectData.modifiedEffect),this, effectData.elementalEffecteMod);     
    }
}
