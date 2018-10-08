using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Lots of diffent game mechanics use the ElementalIdentity enum. For example, 
 a monster of fire elemental type is weak to abilities of elemental type water. This class 
 defines primary data associated with each element.*/
public static class Elemental {

	public enum ElementalIdentity {NONE, Wind, Earth, Water, Fire};
    static float _neutralMod = 1.0f;
    static float _weakMod = 1.50f;
    static float _resistanceMod = 0.50f;

    public static float NeutralMod {
        get {
            return _neutralMod;
        }
    }

    public static float WeakMod {
        get {
            return _weakMod;
        }
    }

    public static float ResistanceMod {
        get {
            return _resistanceMod;
        }
    }

    //Creates a list of element identities and returns a random one. 
    public static ElementalIdentity PickRandomElementalIdentity() {
        int RNG;
        List<ElementalIdentity> elementalIdenityList = new List<ElementalIdentity>();

        foreach (ElementalIdentity value in ElementalIdentity.GetValues(typeof(ElementalIdentity))) {
            if(value != ElementalIdentity.NONE) elementalIdenityList.Add((ElementalIdentity)value);
        }

        RNG = UnityEngine.Random.Range(0, elementalIdenityList.Count);

        return elementalIdenityList[RNG];
    }

    //Returns the color associated with an element.
    public static Color FindElementalColor(ElementalIdentity arg) {
        Color temp = new Color();

        switch (arg) {
            case ElementalIdentity.Earth: temp = Color.green; break;
            case ElementalIdentity.Fire: temp = Color.red; break;
            case ElementalIdentity.Water: temp = Color.blue; break;
            case ElementalIdentity.Wind: temp = Color.yellow; break;
            default: temp = Color.white; break;
        }

        return temp;
    }

    /*When a monster takes damage from an ability, they can be vurnerable, resistance or
     netural to it, depending on the monster's elemental type and the ability elemental type.
     Each of these interactions has a diffent elemental modifier associated with it. This 
     function will return a diffent string value depending on the modifier.*/
    public static string FindElementalModAnnouncementTextInfo(float elementalMod) {
        if (elementalMod == _weakMod) return "Vulnerable!";
        else if (elementalMod == _resistanceMod) return "Resistance!";
        return "";
    }

    /*This function defines the elemental damage modifier for each elemental interaction.
     For example, a fire monster would take 1.5 more damage from a water attack.*/
    public static float FindElementalModifier(ElementalIdentity monsterElement, ElementalIdentity abilityElement) {
        float damageMod = 0.0f;
        string errorMessage = "Error: " + monsterElement + " does not have a interaction for " + abilityElement;

        if (monsterElement == ElementalIdentity.Earth) {
            switch (abilityElement) {
                case ElementalIdentity.Earth: damageMod = _neutralMod; break;
                case ElementalIdentity.Fire: damageMod = _neutralMod; break;
                case ElementalIdentity.Water: damageMod = _resistanceMod; break;
                case ElementalIdentity.Wind: damageMod = _weakMod; break;
                default: Debug.LogError(errorMessage); break;
            }
        }

        else if (monsterElement == ElementalIdentity.Fire) {
            switch (abilityElement) {
                case ElementalIdentity.Earth: damageMod = _neutralMod; break;
                case ElementalIdentity.Fire: damageMod = _neutralMod; break;
                case ElementalIdentity.Water: damageMod = _weakMod; break;
                case ElementalIdentity.Wind: damageMod = _resistanceMod; break;
                default: Debug.LogError(errorMessage); break;
            }
        }

        else if (monsterElement == ElementalIdentity.Water) {
            switch (abilityElement) {
                case ElementalIdentity.Earth: damageMod = _weakMod; break;
                case ElementalIdentity.Fire: damageMod = _resistanceMod; break;
                case ElementalIdentity.Water: damageMod = _neutralMod; break;
                case ElementalIdentity.Wind: damageMod = _neutralMod; break;
                default: Debug.LogError(errorMessage); break;
            }
        }

        else if (monsterElement == ElementalIdentity.Wind) {
            switch (abilityElement) {
                case ElementalIdentity.Earth: damageMod = _resistanceMod; break;
                case ElementalIdentity.Fire: damageMod = _weakMod; break;
                case ElementalIdentity.Water: damageMod = _neutralMod; break;
                case ElementalIdentity.Wind: damageMod = _neutralMod; break;
                default: Debug.LogError(errorMessage); break;
            }
        }

        else {
            Debug.LogError("Error: " + monsterElement + " has not been implemented.");
        }

        return damageMod;
    }
}
