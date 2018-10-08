using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Many classes have common functionalities when it come to displaying a set of monsters panels or
displaying a room's elemental type. Instead of rewriteing these functions for each class, 
this class already define them. This greatly reduces redundancy and makes it easier to fix bugs.*/
public static class RoomEventCommonFunctions {

    //Hides the list of monsters panels passed into the function.
    static void HideMonsterInfoPanelSet(List<MonsterInfoPanel> arg) {
        for (int i = 0; i < arg.Count; i++) {
            arg[i].gameObject.SetActive(false);
        }
    }

    //Updates a list of monsters panels with a list of monster data passed into the function.
    public static void UpdateInfoPanelSet(List<MonsterData> monsterDataSet, List<MonsterInfoPanel> infoPanelSet) {

        HideMonsterInfoPanelSet(infoPanelSet);

        for (int i = 0; i < monsterDataSet.Count; i++) {
            monsterDataSet[i].UpdateMonsterInfoPanel(infoPanelSet[i]);
            infoPanelSet[i].gameObject.SetActive(true);
        }
    }

    //Updates room elemental text with the elemental type of the current floor the player is in.
    public static void UpdateRoomElementText(Text roomElementText) {
        Elemental.ElementalIdentity roomElement = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity;
        roomElementText.text = "Room Element: " + roomElement.ToString();
        roomElementText.color = Elemental.FindElementalColor(roomElement);
    }
}
