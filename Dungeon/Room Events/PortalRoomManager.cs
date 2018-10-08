using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*The portal room is where the player goes before heading to the next floor. The player
 can use their resources to change the properties of the next floor.*/
public class PortalRoomManager : MonoBehaviour {

    public List<RoomBiaseButton> floorElemeentalBiaseButtons;
    public List<Image> keyRoomsIcons;
    public TotemDisplay totemDisplayRef;
    public GameObject takeTotemButton;
    public GameObject discardTotemButton;
    public Text soulCountText;
    public Text keyRoomRerollSoulCostText;
    public RuneDisplay playerRuneDisplay;

    int _keyRoomRerollSoulCost = 3;
    bool _takeFloorTotem = false;

	// Use this for initialization
	void Start () {
        GameManager.instance.DugeonData.NextFloor.FloorElementalBiase.UpdateFloorElementalBiaseButton(floorElemeentalBiaseButtons);
        UpdateKeyRoomIcons();
        UpdateFloorTotemDisplay();
        UpdateTotemAcquireButtons();
        UpdateSoulCountText();
        UpdateKeyRoomRerollSoulCostText();
    }

    //Updates the soul cost to reroll key rooms.
    void UpdateKeyRoomRerollSoulCostText() {
        keyRoomRerollSoulCostText.text = _keyRoomRerollSoulCost.ToString();
    }

    //Updates how much soul the player has.
    void UpdateSoulCountText() {
        soulCountText.text = "Soul: " + GameManager.instance.PlayerInfo.Soul.ToString();
    }

    //Show the current totem reward, if available, in the user interface. 
    void UpdateFloorTotemDisplay() {
        if(GameManager.instance.DugeonData.CurrentFloor.FloorTotem != null) {
            totemDisplayRef.UpdateTotemDisplay(GameManager.instance.DugeonData.CurrentFloor.FloorTotem);
        }
        else {
            totemDisplayRef.gameObject.SetActive(false);
        }
    }

    /*Fades the take and discard totem buttons, depending on which button is currently active.*/
    void UpdateTotemAcquireButtons() {
       
        if(GameManager.instance.DugeonData.CurrentFloor.FloorTotem == null) {
            takeTotemButton.SetActive(false);
            discardTotemButton.SetActive(false);
        }
        else if(_takeFloorTotem == false) {
            UpdateTotemAcquireButtonAlpha(discardTotemButton, 1.0f);
            UpdateTotemAcquireButtonAlpha(takeTotemButton, 0.5f);
        }
        else {
            UpdateTotemAcquireButtonAlpha(discardTotemButton, 0.5f);
            UpdateTotemAcquireButtonAlpha(takeTotemButton, 1.0f);
        }
    }

    //Sets alpha of the button that is passed into the function.
    void UpdateTotemAcquireButtonAlpha(GameObject acquireButton, float alpha) {
        Image imageTemp;
        Color c;
        Text textTemp;

        imageTemp = acquireButton.GetComponent<Image>();
        c = imageTemp.color;
        c.a = alpha;
        imageTemp.color = c;

        textTemp = acquireButton.GetComponentInChildren<Text>();
        c = textTemp.color;
        c.a = alpha;
        textTemp.color = c;
    }

    //Error:Should be called AddToFloorBiaseEvent
    //Checks if the player has enough runes, then modifies the next floor's elemental bias.
    public void AddToRoomBiaseEvent(Elemental.ElementalIdentity arg) {
        if (GameManager.instance.PlayerInfo.SpendRunes(1, arg) == false) return;

        GameManager.instance.DugeonData.NextFloor.AddToRoomBiase(arg);

        GameManager.instance.DugeonData.NextFloor.FloorElementalBiase.UpdateFloorElementalBiaseButton(floorElemeentalBiaseButtons);
        playerRuneDisplay.UpdateRuneDisplay();
    }

    //Spends the player's soul to reroll they key rooms of the next floor.
    public void RerollKeyRoomEvent() {

        if (GameManager.instance.PlayerInfo.Soul - _keyRoomRerollSoulCost < 1) return;

        GameManager.instance.PlayerInfo.Soul -= _keyRoomRerollSoulCost;
        GameManager.instance.DugeonData.NextFloor.RollKeyRooms();

        UpdateKeyRoomIcons();
        UpdateSoulCountText();
    }

    //Updates the icons displayed to repersent the keys room of the next floor.
    void UpdateKeyRoomIcons() {
        List<Room> keyRooms = GameManager.instance.DugeonData.NextFloor.ObtainKeyRooms();

        for (int i = 0; i < keyRoomsIcons.Count; i++) {
            keyRoomsIcons[i].gameObject.SetActive(false);
        }

        for (int j = 0; j < keyRooms.Count; j++) {
            if (keyRooms[j].MyRoomEncounterType == Room.RoomEncounterType.None) continue;
            keyRoomsIcons[j].sprite = GameManager.instance.GameResorcesRef.FindRoomEncounterIcon(keyRooms[j].MyRoomEncounterType);
            keyRoomsIcons[j].gameObject.SetActive(true);
        }
    }

    //Takes the player to the ad scene. If they have the take totem button active, then they will add the totem to their inventory.
    public void NextFloorButtonEvent() {

        if (GameManager.instance.DugeonData.CurrentFloor.FloorTotem != null && _takeFloorTotem == true) {
            GameManager.instance.PlayerInfo.PlayerTotems.Add(GameManager.instance.DugeonData.CurrentFloor.FloorTotem);
        }

        GameManager.instance.ChangeGameState(GameManager.GameState.Ad);
    }

    //Allows player to leave the portal room.
    public void ExitButtonEvent() {
        GameManager.instance.ChangeGameState(GameManager.GameState.FloorNavigation);
    }

    //Allows the player to go the player status scene.
    public void StatusButtonEvent() {
        GameManager.instance.PreviousGameState = Room.RoomEncounterType.Portal;
        GameManager.instance.ChangeGameState(GameManager.GameState.PlayerStatus);
    }

    //Updates the _takeFloorTotem bool, then updates the take and discard buttons.
    public void TotemAcquireButtonEvemt(bool isTakeTotemButton) {
      
        _takeFloorTotem = isTakeTotemButton;
        UpdateTotemAcquireButtons();
    }
}
