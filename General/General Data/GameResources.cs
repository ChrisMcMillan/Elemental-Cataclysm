using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/*
This class acts as a data base for the game. There are certain types of common information that need  to be shared amoung multiple classes and this
class allows us to share that information. 

 Questains to think about:
 If we were able to load the icons directly to the MonsterData class, how would be able to know which icons go with which monster?
 If loaded the icons into a array, then how would be be able to distinguish between the diffent icons?
 If we were able to identify the diffent icons, then that would make this class an unnecessary middle man. 
 */
public class GameResources : MonoBehaviour {

    [Serializable]
    public class MonsterResources {
        public MonsterData.MonsterIdentity identity;
        public Sprite icon;
        public GameObject skeleton;
    }

    [Serializable]
    public class UniqueBossResources {
        public UniqueBossData.UniqueBossIdentity identity;
        public GameObject skeleton;
    }

    [Serializable]
    public class ElementalResources {
        public Elemental.ElementalIdentity identity;
        public Sprite background;
        public Sprite monsterInfoPanelBackgroundImage;
        public Sprite runeImage;
    }

    [Serializable]
    public class AbilityIcons {
        public Ability.AbilityIdentity identity;
        public Sprite icon;
    }

    [Serializable]
    public class RoomEncounterResources {
        public Room.RoomEncounterType encounter;
        public Sprite roomEncounterIcon;
    }

    [Serializable]
    public class TotemResources {
        public TotemData.TotemIdentity identity;
        public Sprite icon;
    }

    public Sprite playerLocationIcon;
    public Sprite noneIcon;
    public MonsterResources[] monsterResources;
    public AbilityIcons[] abilityIcons;
    public RoomEncounterResources[] roomEncounterResources;
    public ElementalResources[] elementalResources;
    public UniqueBossResources[] uniqueBossResources;
    public TotemResources[] totemResorces;

    public Sprite LoadNoneIcon() {
        return noneIcon;
    }

    //Returns a totem icon associted with a totem identity.
    public Sprite FindTotemIcon(TotemData.TotemIdentity arg) {

        for (int i = 0; i < totemResorces.Length; i++) {
            if (totemResorces[i].identity == arg) return totemResorces[i].icon;
        }

        Debug.LogError("Error: Could not find totem icon for: " + arg);
        return null;
    }

    //Returns a monster icon associted with a monster identity.
    public Sprite FindMonsterIcon(MonsterData.MonsterIdentity arg) {

        for (int i = 0; i < monsterResources.Length; i++) {
            if (monsterResources[i].identity == arg) return monsterResources[i].icon;
        }

        Debug.LogError("Error: Could not find monster icon " + arg);
        return null;
    }

    /*Returns a monster's skeleton associted with a monster identity.
     Also checks for unique boss identity.*/
    public GameObject FindSkeleton(object arg) {

        if (arg.GetType() == typeof(MonsterData.MonsterIdentity)) {
            for (int i = 0; i < monsterResources.Length; i++) {
                if (monsterResources[i].identity == (MonsterData.MonsterIdentity)arg) return monsterResources[i].skeleton;
            }
        }
        else {
            for (int i = 0; i < uniqueBossResources.Length; i++) {
                if (uniqueBossResources[i].identity == (UniqueBossData.UniqueBossIdentity)arg) return uniqueBossResources[i].skeleton;
            }
        }

        Debug.LogError("Error: Could not find skeleton for " + arg);
        return null;
    }

    //Returns ability icon associted with ability identity. 
    public Sprite FindAbilityIcons(Ability.AbilityIdentity arg) {

        for (int i = 0; i < abilityIcons.Length; i++) {
            if (abilityIcons[i].identity == arg) return abilityIcons[i].icon;
        }

        Debug.LogError("Error: Cound not find ability icon " + arg);
        return noneIcon;
    }

    //Returns icon associated with an room encounter.
    public Sprite FindRoomEncounterIcon(Room.RoomEncounterType arg) {
        for (int i = 0; i < roomEncounterResources.Length; i++) {
            if (roomEncounterResources[i].encounter == arg) return roomEncounterResources[i].roomEncounterIcon;
        }

        //Debug.LogError("Error: Cound not find roomEncounterIcon " + arg);
        return null;
    }

    //Return a combat background associated with an elemental identity.
    public Sprite FindElementalBackground(Elemental.ElementalIdentity arg) {
        for (int i = 0; i < elementalResources.Length; i++) {
            if (elementalResources[i].identity == arg) return elementalResources[i].background;
        }

        Debug.LogError("Error: Cound not find Elemental background " + arg);
        return null;
    }

    //Return a monster info panel associated with an elemental identity.
    public Sprite FindMonsterInfoPanelBackgroundImage(Elemental.ElementalIdentity arg) {
        for (int i = 0; i < elementalResources.Length; i++) {
            if (elementalResources[i].identity == arg) return elementalResources[i].monsterInfoPanelBackgroundImage;
        }

        Debug.LogError("Error: Cound not find Monster Info Panel Background Image for" + arg);
        return null;
    }

    //Returns a rune image associated with an elemental identity.
    public Sprite FindRuneImage(Elemental.ElementalIdentity arg) {
        for (int i = 0; i < elementalResources.Length; i++) {
            if (elementalResources[i].identity == arg) return elementalResources[i].runeImage;
        }

        Debug.LogError("Error: Could not fine rune image for " + arg);
        return null;
    }
}
