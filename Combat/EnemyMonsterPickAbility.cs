using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the logic for enemy monsters picking their abilities. 
public static class EnemyMonsterPickAbility {

    /*Picks a random ability for the enemy monster. If that ability passes some checks, then it becomes the
     ability that the monster will use. Otherwise,  GerneralMonsterPickAbility() moves through the monster's 
     abilites until a vaild one is found.*/
    public static void PickAbility(MonsterData enemyMonsterData, MonsterData playerMonsterData) {

        int abilityIndex = UnityEngine.Random.Range(0, enemyMonsterData.Abilities.Length);
        
        abilityIndex = GerneralMonsterPickAbility(enemyMonsterData, playerMonsterData,abilityIndex);
    
        enemyMonsterData.Abilities[abilityIndex].UseAbility(enemyMonsterData, playerMonsterData);
    }

    //Depending on the identity of the ability, this function preforms a check on that ability. If it passes the check
    //then the monster will use that ability. Else this function loops through the monster's abilities until a vaild one is found.  
    static int GerneralMonsterPickAbility(MonsterData enemyMonsterData,MonsterData playerMonsterData, int abilityIndex) {

        bool havePickedAbility = false;
        Ability.AbilityIdentity enemyAbilityIden;

        for (int j = 0; j < enemyMonsterData.Abilities.Length; j++) {

            enemyAbilityIden = (Ability.AbilityIdentity)enemyMonsterData.Abilities[abilityIndex].MyAbilityIdentity;

            if (enemyAbilityIden == Ability.AbilityIdentity.Self_Immolation) {
                havePickedAbility = DecideForSelfImmolation(enemyMonsterData, playerMonsterData, enemyAbilityIden);
            }
            else if (enemyAbilityIden == Ability.AbilityIdentity.Healing_Wave || enemyAbilityIden == Ability.AbilityIdentity.Regrowth ||
                enemyAbilityIden == Ability.AbilityIdentity.Lifebloom) {
                havePickedAbility = DecideForHealingAbility(enemyMonsterData, playerMonsterData, enemyAbilityIden);
            }
            else {
                havePickedAbility = true;
            }

            if (havePickedAbility) break;

            abilityIndex++;
            if (abilityIndex > enemyMonsterData.Abilities.Length - 1) abilityIndex = 0;
        }

        return abilityIndex;
    }

    //Use to debug the enemy artificial intelligence
    static void EnemyMonsterPickAbilityDebugOutput(bool decidedToUse, Ability.AbilityIdentity abilityIdenity, float RNG, float chanceToUse) {
        string temp = "Enemy monster decided ";
        temp += (decidedToUse) ? "to use " : "not to use ";
        temp += abilityIdenity.ToString();
        temp += " with RNG: " + RNG.ToString("F2");
        temp += " chance to use: " + chanceToUse.ToString("F2");
        Debug.Log(temp);
    }

    //This function makes it unlikely the enemy monster will use self imolation when they have low health
    //or when they don't have any debuffs on them.   
    static bool DecideForSelfImmolation(MonsterData enemyMonsterData, MonsterData playerMonsterData, Ability.AbilityIdentity abilityIdenity) {
        float chanceToUse = 0.01f;
        float chanceToUseIncrament = 0.40f;
        float RNG = UnityEngine.Random.Range(0.0f, 1.0f);

        if (enemyMonsterData.Health < enemyMonsterData.MaxHealth * 0.25f) {
            //Debug.Log("Enemy monster decied not to use Self Imolation because of low health.");
            return false;
        }

        if (enemyMonsterData.CheckForMatchingAilment(StatusAilment.StatusAilmentType.Burning)) chanceToUse += chanceToUseIncrament;
        if (enemyMonsterData.CheckForMatchingAilment(StatusAilment.StatusAilmentType.Poison)) chanceToUse += chanceToUseIncrament;

        if(RNG <= chanceToUse) {
            //EnemyMonsterPickAbilityDebugOutput(true, abilityIdenity, RNG, chanceToUse);
            return true;
        }

        //EnemyMonsterPickAbilityDebugOutput(false, abilityIdenity, RNG, chanceToUse);

        return false;
    }

    //This function makes it unlikely the enemy monster will use a healing ability when they have alot of health already. 
    static bool DecideForHealingAbility(MonsterData enemyMonsterData, MonsterData playerMonsterData, Ability.AbilityIdentity abilityIdenity) {

        if(enemyMonsterData.HealingReductionStacks >= 4) {
            Debug.Log("Enemy monster decided not to use heal ability because of high HealingReductionStacks.");
            return false;
        }

        float enemyHealthPercent = (float)enemyMonsterData.Health / (float)enemyMonsterData.MaxHealth;
        float chanceToUse = (1.0f - enemyHealthPercent);
        float RNG = UnityEngine.Random.Range(0.0f, 1.0f);

        if (chanceToUse > 0.8f) chanceToUse = 0.8f;
       
        if (RNG <= chanceToUse) {
            //EnemyMonsterPickAbilityDebugOutput(true, abilityIdenity, RNG, chanceToUse);
            return true;
        }

        //EnemyMonsterPickAbilityDebugOutput(false, abilityIdenity, RNG, chanceToUse);

        return false;
    }

    //This function makes it unlikely the enemy monster will use a stun abiility if the player monster's stun resistance is high.
    static bool DecideForStunAbility(MonsterData enemyMonsterData, MonsterData playerMonsterData, Ability.AbilityIdentity abilityIdenity) {

        if(playerMonsterData.CurStunResistance < 0.75f) {
            
            return true;
        }
        Debug.Log("Enemy monster decied not to use stun ability because of high CurStunResistance.");

        return false;
    }
}
