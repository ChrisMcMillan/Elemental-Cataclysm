using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Display infomation about individual a room on the mini map, as a icon.*/
public class MapNode : MonoBehaviour {

    public GameObject conRight;
    public GameObject conLeft;
    public GameObject conUp;
    public GameObject conDown;
    public GameObject centerBox;
    public Text numberDisplay;
    public Image icon;


    /*Update the node's icon color base on what elemental type of the room.
    Fades out the icon if the room encounter has veen cleared.*/  
    public void UpdateNodeColor(Room roomData){
        Image imageRef = centerBox.GetComponent<Image>();

        imageRef.color = Elemental.FindElementalColor(roomData.RoomElementalIdentity);

        if (roomData.RoomEncounterCleared && roomData.MyRoomEncounterType != Room.RoomEncounterType.None) {
            Color c = imageRef.color;
            c.a = 0.5f;
            imageRef.color = c;
        }
    }

    /*This class can also be use to display information about a floor, as an icon. 
     This fuction is used by dugeon progress display class to display information about a floor.*/
    public void UpdateDugeonProgressNode(Floor floorData) {
        conUp.SetActive(false);
        conDown.SetActive(false);
        numberDisplay.gameObject.SetActive(false);

        Image imageRef = centerBox.GetComponent<Image>();
        imageRef.color = Elemental.FindElementalColor(floorData.ElementalType);

        if(floorData == GameManager.instance.DugeonData.CurrentFloor) {
            icon.sprite = GameManager.instance.GameResorcesRef.playerLocationIcon;
        }
        else if(floorData.UniqueBossType != UniqueBossData.UniqueBossIdentity.NONE) {
            icon.sprite = GameManager.instance.GameResorcesRef.FindRoomEncounterIcon(Room.RoomEncounterType.Boss);
        }
        else {
            icon.gameObject.SetActive(false);
        }
    }
}
