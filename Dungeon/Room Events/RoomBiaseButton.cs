using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Error: This class should be called FloorBiaseButton
This class is necessary because each button must have its own elemental identity. The 
button's elemental idenity is pass into portalRoomManger script's AddToRoomBiaseEvent() and
this let it know which floor elemental biase to add. For example, if this button has earth
as its elemental identity and the player clicked on it, then next floor would have more earth rooms.*/
public class RoomBiaseButton : MonoBehaviour {

    public PortalRoomManager portalRoomMangerRef;

    Text buttonText;
    Elemental.ElementalIdentity _buttonElementalType;

    public Elemental.ElementalIdentity ButtonElementalType {
        get {
            return _buttonElementalType;
        }

        set {
            _buttonElementalType = value;
        }
    }

    public Text ButtonText {
        get {
            return buttonText;
        }
    }

    // Use this for initialization
    void Start () {
        buttonText = GetComponentInChildren<Text>();
	}

    public void RoomBiaseButtonEvent() {
        
        portalRoomMangerRef.AddToRoomBiaseEvent(_buttonElementalType);
    }
}
