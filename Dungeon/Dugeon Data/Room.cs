using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*A room is a space, which the player explores. It can have many diffent encounter, which changes how player plays the game.*/
public class Room {

    public enum RoomStructureType {Unused, Empty, Start, Key};
    public enum RoomEncounterType {None, Combat, Healing, Portal, Boss, BeastMaster, ShrineOfWisdom, ShrineOfSacrifice,
        ShrineOfPower, ShrineOfKnowledge};

    Elemental.ElementalIdentity _roomElementalIdentity;
    RoomStructureType _myRoomStructureType;
    RoomEncounterType _myRoomEncounterType;
    bool _roomEncounterCleared;
    Vector2 _coordinate;
	MapNode _myNode;
    bool _hasUpCon;
    bool _hasDownCon;
    bool _hasLeftCon;
    bool _hasRightCon;

	public MapNode MyNode {
		get{
			return _myNode;
		}
		set{
			_myNode = value;
		}
	}
    public Vector2 Coordinate {
        get {
            return _coordinate;
        }
    }

    public RoomStructureType MyRoomStructureType {
        get {
            return _myRoomStructureType;
        }

        set {
            _myRoomStructureType = value;
        }
    }

    public RoomEncounterType MyRoomEncounterType {
        get {
            return _myRoomEncounterType;
        }

        set {
            _myRoomEncounterType = value;
            _roomEncounterCleared = false;
        }
    }

    public bool RoomEncounterCleared {
        get {
            return _roomEncounterCleared;
        }

        set {
            _roomEncounterCleared = value;
        }
    }

    public Elemental.ElementalIdentity RoomElementalIdentity {
        get {
            return _roomElementalIdentity;
        }

        set {
            _roomElementalIdentity = value;
        }
    }

    public void InitRoomData(Vector2 newCoordinate){
        _coordinate = newCoordinate;
        _myRoomStructureType = RoomStructureType.Unused;
        _myRoomEncounterType = RoomEncounterType.None;
        _roomEncounterCleared = true;
    }

    //Changes the room encounter type. 
    public void ChangeRoomEncounterType(RoomEncounterType arg) {
        _myRoomEncounterType = arg;
        _roomEncounterCleared = false;
    }


    //Initializes the data that shows if a room has rooms adjacent to it.   
    public void InitConectorData(Room[] adjacentRooms){

        _hasUpCon = false;
        _hasDownCon = false;
        _hasLeftCon = false;
        _hasRightCon = false;

        for (int i = 0; i < adjacentRooms.Length; i++){

            if (adjacentRooms[i] == null || adjacentRooms[i].MyRoomStructureType == RoomStructureType.Unused) continue;

            if (adjacentRooms[i].Coordinate == _coordinate + new Vector2(-1,0)) _hasUpCon = true;
            
            else if (adjacentRooms[i].Coordinate == _coordinate + new Vector2(1, 0)) _hasDownCon = true;

            else if (adjacentRooms[i].Coordinate == _coordinate + new Vector2(0, 1)) _hasRightCon = true;

            else if (adjacentRooms[i].Coordinate == _coordinate + new Vector2(0, -1)) _hasLeftCon = true;

            //Debug.Log("C: " + _myRoomType + " " + _coordinate + " " +
                  //"A: " + adjacentRooms[i]._myRoomType + " " + adjacentRooms[i].Coordinate);
        }
    }

    //Update room icon and its connectors, on the mini map.
    public void UpdateApperance(){

        UpdateConnectors();

        _myNode.numberDisplay.text = _coordinate.x.ToString() + "-" + _coordinate.y.ToString();
        _myNode.numberDisplay.gameObject.SetActive(false);

        UpdateIcon();

        _myNode.UpdateNodeColor(this);
    }

    //If room is next to other rooms, this function displays the connections between them, on the mini map.
    void UpdateConnectors() {
        if (_myRoomStructureType == RoomStructureType.Unused) {

            _myNode.centerBox.SetActive(false);
            _myNode.conUp.SetActive(false);
            _myNode.conDown.SetActive(false);
            _myNode.conLeft.SetActive(false);
            _myNode.conRight.SetActive(false);
        }
        else {
            _myNode.centerBox.SetActive(true);

            if (_hasUpCon) _myNode.conUp.SetActive(true);
            else _myNode.conUp.SetActive(false);

            if (_hasDownCon) _myNode.conDown.SetActive(true);
            else _myNode.conDown.SetActive(false);

            if (_hasLeftCon) _myNode.conLeft.SetActive(true);
            else _myNode.conLeft.SetActive(false);

            if (_hasRightCon) _myNode.conRight.SetActive(true);
            else _myNode.conRight.SetActive(false);
        }
    }

    //Updates the appearance of the room icon on the mini map.
    void UpdateIcon() {
        Sprite newIcon = GameManager.instance.GameResorcesRef.FindRoomEncounterIcon(_myRoomEncounterType);
        bool isPlayerRoom = false;

        if(this == GameManager.instance.DugeonData.CurrentFloor.PlayerRoom) {
            newIcon = GameManager.instance.GameResorcesRef.playerLocationIcon;
            isPlayerRoom = true;
        }

        if (newIcon != null) {
            _myNode.icon.sprite = newIcon;
            _myNode.icon.gameObject.SetActive(true);

            Color c = _myNode.icon.color;
            c.a = (_roomEncounterCleared) ? 0.5f : 1.0f;
            if (isPlayerRoom) c.a = 1.0f;
            _myNode.icon.color = c;
        }
        else {
            _myNode.icon.gameObject.SetActive(false);
        }
    }
}
