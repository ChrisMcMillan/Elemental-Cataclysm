using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Unique bosses are extra tough monsters with unique mechanics.*/
public class UniqueBossData : MonsterData {

    public enum UniqueBossIdentity {NONE, Ignis, EverLasting, Shifter};

    /*We don't include the shifer when picking a random unique boss because 
     it should always be at the end of the dungeon.*/
    public static UniqueBossIdentity PickRandomUniqueBossIdentity() {
        int RNG;
        List<UniqueBossIdentity> elementalIdenityList = new List<UniqueBossIdentity>();

        foreach (UniqueBossIdentity value in UniqueBossIdentity.GetValues(typeof(UniqueBossIdentity))) {
            if (value != UniqueBossIdentity.NONE && value != UniqueBossIdentity.Shifter) elementalIdenityList.Add(value);
        }

        RNG = UnityEngine.Random.Range(0, elementalIdenityList.Count);

        return elementalIdenityList[RNG];
    }

    /*When deciding which unique bosses should go where in the dugeon, we want
     to create a list. We do this so we can remove from the list when we pick a unique
     boss. This guarantees there are no repeats.*/
    public static List<UniqueBossIdentity> GetUniqueBossIdentityList() {
        List<UniqueBossIdentity> bossList = new List<UniqueBossIdentity>();

        foreach (UniqueBossIdentity value in UniqueBossIdentity.GetValues(typeof(UniqueBossIdentity))) {
            if (value != UniqueBossIdentity.NONE && value != UniqueBossIdentity.Shifter) bossList.Add(value);
        }

        return bossList;
    }

    //Find the elemental identity associated with a unique boss identity.   
    public static Elemental.ElementalIdentity FindUniqueBosElementalType(object arg) {

        string errorMessage = "Error: " + arg + " has not been assigned a elemental type.";
       
        switch ((UniqueBossIdentity)arg) {
            case UniqueBossIdentity.Ignis: return Elemental.ElementalIdentity.Fire;
            case UniqueBossIdentity.EverLasting: return Elemental.ElementalIdentity.Earth;
            case UniqueBossIdentity.Shifter: return Elemental.PickRandomElementalIdentity();
            default: Debug.LogError(errorMessage); return Elemental.ElementalIdentity.NONE;
        }
    }

    //Gives a unique boss their attributes.
    protected override void SetBaseIdentityAttributes(object arg) {
        //stregth: 5, agility: 5, spirit: 5, speed: 5;
        switch ((UniqueBossIdentity)arg) {
            case UniqueBossIdentity.Ignis: ApplyAttributes(10, 2, 6, 2); break;
            case UniqueBossIdentity.EverLasting: ApplyAttributes(6, 10, 2, 2); break;
            case UniqueBossIdentity.Shifter: ApplyAttributes(5, 5, 5, 5); break;
            default: Debug.LogError("Error: " + arg + " has not been assigned attributes."); break;
        }
    }

    //Find the class associated with a unique boss identity. 
    public static MonsterData FindUniqueBossDataType(UniqueBossIdentity uniqueBossType) {
        MonsterData newMonsterData = new MonsterData();

        switch (uniqueBossType) {
            case UniqueBossIdentity.NONE: break;
            case UniqueBossIdentity.Ignis: newMonsterData = new IgnisData(); break;
            case UniqueBossIdentity.EverLasting: newMonsterData = new EverLastingData(); break;
            case UniqueBossIdentity.Shifter: newMonsterData = new ShifterData(); break;
            default: Debug.LogError("Error: Data type allocation has not been define for " + uniqueBossType); break;
        }

        return newMonsterData;
    }
}
