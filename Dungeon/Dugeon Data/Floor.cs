using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A floor is the space in which the player navigate. It is repersented as a matrix of rooms.  
Note: (0,0) is in the upper left coner. (x,y) x value increaes as you go downward and y value increaes as you go right.*/
public class Floor {

    UniqueBossData.UniqueBossIdentity _uniqueBossType;
    Elemental.ElementalIdentity _elementalType;
    FloorElementalBiases _floorElementalBiase;
    TotemData _floorTotem;
    int _floorLevel;
    int _floorSize;
    Room[,] _roomMatrix;
    Room _startRoom;
    Room _playerRoom;

    /*By having a refference to all active rooms, we can reduce the number of rooms we need loop through
    This can reduce time complixty from O(n^2) to O(n)*/
    List<Room> _activeRooms = new List<Room>();
    List<MonsterData> _beastMasterMonsters;

    public Room[,] RoomMatrix {
        get {
            return _roomMatrix;
        }
    }

    public int FloorSize {
        get {
            return _floorSize;
        }
    }

    public Room PlayerRoom {
        get {
            return _playerRoom;
        }
    }

    public List<MonsterData> BeastMasterMonsters {
        get {
            return _beastMasterMonsters;
        }
    }

    public int FloorLevel {
        get {
            return _floorLevel;
        }
    }

    public FloorElementalBiases FloorElementalBiase {
        get {
            return _floorElementalBiase;
        }
    }

    public Elemental.ElementalIdentity ElementalType {
        get {
            return _elementalType;
        }
    }

    public UniqueBossData.UniqueBossIdentity UniqueBossType {
        get {
            return _uniqueBossType;
        }
    }

    public TotemData FloorTotem {
        get {
            return _floorTotem;
        }
    }

    public void InitData(int floorLevelArg, int floorSizeArg, UniqueBossData.UniqueBossIdentity uniqueBossTypeArg, TotemData floorTotemArg = null) {
        _floorLevel = floorLevelArg;
        _floorSize = floorSizeArg;
        _floorTotem = floorTotemArg;

        _uniqueBossType = uniqueBossTypeArg;

        if (_uniqueBossType == UniqueBossData.UniqueBossIdentity.NONE) _elementalType = Elemental.PickRandomElementalIdentity();
        else _elementalType = UniqueBossData.FindUniqueBosElementalType(_uniqueBossType);

        _floorElementalBiase = FloorElementalBiases.CreatePresetOfType(_elementalType);
    }

    public void CreateFloor() {
        InitFloorData();
        PickStartRoom();
        BuildBranches();
        InitConnectorData();
        RollHallwayRoomTypes();
        RollKeyRooms();
        DecideElmentalRoomBiase();
        InitBeastMasterMonsters();
    }

    //Player's initial soul count is taken as percentage of the floor size so make sure the player
    //dosn't have too much soul for small floors and enough soul for large floors.    
    public int PlayerStartSoul() {
        return Mathf.RoundToInt(_activeRooms.Count * 0.40f);
    }

    //Checks if a vector is widthin the room matrix. 
    bool IsVaildRoomMatrixPosition(Vector2 position) {
        if(position.x >= 0 && position.x < _roomMatrix.GetLength(0) &&
            position.y >= 0 && position.y < _roomMatrix.GetLength(1)){
            return true;
        }

        return false;
    }

    //Checks if a new room reference is already in the active room list. If not, then it adds it to the active room list.   
    bool AddToActiveRooms(Room newRoom, bool addToList = true) {
        foreach (Room roomRef in _activeRooms) {
            if(newRoom == roomRef) {
                return false;
            }
        }

        if(addToList) _activeRooms.Add(newRoom);
        return true;
    }

    /*When a player clicks a navigation arrow, this function is called. Depending on what arrow the player 
      clicks; this function can move the player in different direction, in the floor. If makes sure it is a 
      vaild move the moves the player into the new room.*/ 
    public void NavArrowButtonEvent(NavArrowClickHandler.ArrowIdentity arrowIdentityArg){
        Vector2 newPlayerLocation = _playerRoom.Coordinate;

        switch (arrowIdentityArg){
            case NavArrowClickHandler.ArrowIdentity.Up: newPlayerLocation += new Vector2(-1,0); break;
            case NavArrowClickHandler.ArrowIdentity.Down: newPlayerLocation += new Vector2(1, 0); break;
            case NavArrowClickHandler.ArrowIdentity.Left: newPlayerLocation += new Vector2(0, -1); break;
            case NavArrowClickHandler.ArrowIdentity.Right: newPlayerLocation += new Vector2(0, 1); break;
            default: Debug.LogError("newPlayerLocation not define for " + arrowIdentityArg); break; 
        }

        if(IsVaildRoomMatrixPosition(newPlayerLocation) == false) {
            return;
        }

        else if(_roomMatrix[Mathf.FloorToInt(newPlayerLocation.x), 
            Mathf.FloorToInt(newPlayerLocation.y)].MyRoomStructureType == Room.RoomStructureType.Unused){
            return;
        }

        _playerRoom = _roomMatrix[Mathf.FloorToInt(newPlayerLocation.x), Mathf.FloorToInt(newPlayerLocation.y)];

        MovePlayerToNextRoom();
    }

    /*When a player moves to new room, they lose soul. When this happens, we need to check if their
     soul is at or below 0. If so, they lose the game. We also need to check if there is a rooom encounter 
     in the next room. If so, then go to speical encounter scene. Else just go to the floor navigation scene*/
    void MovePlayerToNextRoom() {
       
        if (GameManager.instance.PlayerInfo.DrainSoul(DetermineDrainSoulAmount())) {
            GameManager.instance.PlayerInfo.SouldHasFaded = true;
            GameManager.instance.ChangeGameState(GameManager.GameState.Defeat);
            return;
        }
        else if(_playerRoom.RoomEncounterCleared == true) {
            GameManager.instance.ChangeGameState(GameManager.GameState.FloorNavigation);
            return;
        }

        GameManager.instance.ChangeGameState(_playerRoom.MyRoomEncounterType);
    }

    /*We need check if they player has the Totem of Grace and the room they are moving into.
     This can change the amount of soul that is drained, when they player moves into a new room.*/
    int DetermineDrainSoulAmount() {
        int drainAmount = 1;

        if (GameManager.instance.PlayerInfo.PlayerHasTotem(TotemData.TotemIdentity.Grace) &&
           _playerRoom.RoomElementalIdentity == Elemental.ElementalIdentity.Wind) {

            drainAmount--;
        }
        else if (GameManager.instance.PlayerInfo.PlayerHasTotem(TotemData.TotemIdentity.Grace) &&
           _playerRoom.RoomElementalIdentity == Elemental.ElementalIdentity.Fire) {

            drainAmount++;
        }
       
        return drainAmount;
    }

    //Creates the matrix of rooms that make up the floor. O(n^2)
    void InitFloorData(){
        _roomMatrix = new Room[_floorSize, _floorSize];
        Vector2 newCoordinate;

		for (int i = 0; i < _floorSize; i++){
			for (int j = 0; j < _floorSize; j++){
                _roomMatrix[i, j] = new Room();
                newCoordinate = new Vector2(i, j);
                _roomMatrix[i, j].InitRoomData(newCoordinate);
            }
        }
    }

    //Determines where the player will start in the floor. Approximates the middle of the matrix and set it there. 
    void PickStartRoom(){

		int row = Mathf.RoundToInt(_roomMatrix.GetLength(0) / 2);
		int column = Mathf.RoundToInt(_roomMatrix.GetLength(1) / 2);

        _roomMatrix[row, column].MyRoomStructureType = Room.RoomStructureType.Start;
        _startRoom = _roomMatrix[row, column];
        _playerRoom = _startRoom;
    }

    /*To gerenate the floor we frist need to set the branches. Branches are created from picking a random room, that is 
     within a small radius of one the conners of the matrix, and building a path of rooms to that room. These branches are built 
     starting from the start room. We also need to connect these branches to each other. So we need to create list of list to store 
     references to these branches. O(n)*/
    void BuildBranches(){
        int branchCount = UnityEngine.Random.Range(3, 4 + 1);
        int radius = Mathf.CeilToInt(_floorSize* 0.2f);
        List<List<Room>> allBranches = new List<List<Room>>();
        List<Room> currentBranch;
       
        List<Room> roomRefs = PickRandomConerRoomsInRadius(radius, GetConerRooms());
        
        for (int i = 0; i < branchCount; i++){
            currentBranch = new List<Room>();
            CreateRoomsToTarget(_startRoom, roomRefs[i], currentBranch);
            allBranches.Add(currentBranch);
            roomRefs[i].MyRoomStructureType = Room.RoomStructureType.Key;
        }

        AddToActiveRooms(_startRoom);

        RemoveRoomsWithingRadiusOfStart(allBranches);
        ConnectBraches(allBranches);
    }

    /*When creating connection between the branches, we don't want them to be too close to the start room.
     If they are too close to the start room, then rooms will be too bunched together when they are being connected. 
     So calculate a radius multiplying the floor size by 0.5. We loop through all the rooms within the branches.
     If the distance between the a room and the start room is less then or equal to the radius, we remove that room
     from the list. O(n*m)*/
    void RemoveRoomsWithingRadiusOfStart(List<List<Room>> allBranches) {
        int radius = Mathf.RoundToInt(_floorSize * 0.5f);
        Room roomRef;
        int count = allBranches.Count - 1;
        int innerCount;

        while(count >= 0) {
            innerCount = allBranches[count].Count - 1;
            while (innerCount >= 0) {
                roomRef = allBranches[count][innerCount];
                if (FindRoomDistance(_startRoom, roomRef) <= radius) {
                    allBranches[count].RemoveAt(innerCount);
                    if (allBranches[count].Count == 0) {
                        //Debug.LogError("Warning: A branch's count is 0");
                        allBranches.RemoveAt(count);
                    }
                }
                innerCount--;
            }
            count--;
        }
    }

    /*To connect the branches we need to pick random room in one of the branches and a random room in another one.
     We loop through the branches list and pick a room in the first branch list and another room in the next branch list.
     After we pick our two rooms, we create a path of rooms to connect them. O(n)*/
    void ConnectBraches(List<List<Room>> allBranches) {
        int RNG;
        Room randomRoom1;
        Room randomRoom2;

        for (int i = 0; i < allBranches.Count - 1; i++) {
            RNG = UnityEngine.Random.Range(0, allBranches[i].Count);
            //Index out range when allBranches[i].Count = 0
            //Debug.Log("ConnectBraches() " + "RNG: "+ RNG + " allBranches[i].Count: " + allBranches[i].Count);
            randomRoom1 = allBranches[i][RNG];
            RNG = UnityEngine.Random.Range(0, allBranches[i + 1].Count);
            randomRoom2 = allBranches[i + 1][RNG];
            ConnectTwoRooms(randomRoom1, randomRoom2);
        }
    }

    /*To connect two rooms we frist need to find the midpoint between those two rooms. After
     we have the midpoint, we then need move it farther away from the start room, so the rooms are not 
     bunched together, when we preform the connection process. We do this using the ExaggerateVectorDistance()
     function. We then need to preform more checks to make sure the midpoint room is not an or next to already 
     active rooms. If either is true, then we need to use the ExaggerateVectorDistance() function again to move it
     away from those rooms. After that, we connect our two rooms to the midpoint room.*/
    void ConnectTwoRooms(Room room1, Room room2) {

        List<Room> alternativeRooms;
        Room midPointRoom;
        Vector2 midPoint = (room1.Coordinate + room2.Coordinate) / 2;
        //float roomDisatance = FindRoomDistance(room1, room2);
        
        midPoint = new Vector2(Mathf.Round(midPoint.x), Mathf.Round(midPoint.y));
        midPoint = ExaggerateVectorDistance(_startRoom.Coordinate, midPoint, 2);
        midPointRoom = _roomMatrix[Mathf.RoundToInt(midPoint.x), Mathf.RoundToInt(midPoint.y)];

        //If the mid point room is an existing room then pick a random room within the specified radius of the midpoint room
        //that is not an existing room. Next, exaggerate distance from start room. 
        if (AddToActiveRooms(midPointRoom, false) == false){
   
            alternativeRooms = FindRoomsWithinRadius(midPointRoom, 1,true);
            if (alternativeRooms.Count > 0) midPointRoom = alternativeRooms[UnityEngine.Random.Range(0, alternativeRooms.Count)];
            midPoint = midPointRoom.Coordinate;
            midPoint = ExaggerateVectorDistance(_startRoom.Coordinate, midPoint, 2);
            midPointRoom = _roomMatrix[Mathf.RoundToInt(midPoint.x), Mathf.RoundToInt(midPoint.y)];
        }

        //Check if mid point room is next to another existing room
        alternativeRooms = FindRoomsWithinRadius(midPointRoom, 1, false);
        if (alternativeRooms.Count == 1) {
            //Debug.Log("Mid point room is next to existing room. Midpoint Room: " + midPointRoom.Coordinate +
              //  " Existing room: " + alternativeRooms[0].Coordinate);
            midPoint = midPointRoom.Coordinate;
            midPoint = ExaggerateVectorDistance(alternativeRooms[0].Coordinate, midPoint, 2);
            midPointRoom = _roomMatrix[Mathf.RoundToInt(midPoint.x), Mathf.RoundToInt(midPoint.y)];
           // Debug.Log("New midpoint postion: " + midPointRoom.Coordinate);
        }

        //Debug.Log("room1: " + room1.Coordinate + " room2: " + room2.Coordinate + " Room distance: " + roomDisatance +
          //  " midpointRoom: " + midPointRoom.Coordinate);

        CreateRoomsToTarget(room1, midPointRoom);
        CreateRoomsToTarget(room2, midPointRoom);
    }

    /*This function moves vector position away from another vector position. The comapareVector argument is the vector we want move away from.
     The oldVectorPoisiton is orgainal vector position. The exaggeration argument is how much we want to move. The function also makes sure
     that the new vector position is within the bounds of the room matrix.*/
    Vector2 ExaggerateVectorDistance(Vector2 compareVector, Vector2 oldVectorPoisiton, int exaggeration) {
        Vector2 newVectorPoistion = oldVectorPoisiton;

        if (newVectorPoistion.x < compareVector.x) {
            newVectorPoistion.x -= exaggeration;
            if (newVectorPoistion.x < 0) {
                newVectorPoistion.x = 0;
                //newVectorPoistion.x += exaggeration;
            }
        }

        if (newVectorPoistion.x > compareVector.x) {
            newVectorPoistion.x += exaggeration;
            if (newVectorPoistion.x > _roomMatrix.GetLength(0) - 1) {
                newVectorPoistion.x = _roomMatrix.GetLength(0) - 1;
                //newVectorPoistion.x -= exaggeration;
            }
        }

        if (newVectorPoistion.y < compareVector.y) {
            newVectorPoistion.y -= exaggeration;
            if (newVectorPoistion.y < 0) {
                newVectorPoistion.y = 0;
                //newVectorPoistion.y += exaggeration;
            }
        }

        if (newVectorPoistion.y > compareVector.y) {
            newVectorPoistion.y += exaggeration;
            if (newVectorPoistion.y > _roomMatrix.GetLength(1) - 1) {
                newVectorPoistion.y = _roomMatrix.GetLength(1) - 1;
                //newVectorPoistion.y -= exaggeration;
            }
        }
        
        return newVectorPoistion;
    }

    //Gets list of rooms within a radius of the centerRoom agrument. O(nm)
    List<Room> FindRoomsWithinRadius(Room centerRoom, int radius, bool addNonExistingRooms) {
        List<Room> roomList = new List<Room>();

        int startRow = Mathf.RoundToInt(centerRoom.Coordinate.x - radius);
        if (startRow < 0) startRow = 0;

        int endRow = Mathf.RoundToInt(centerRoom.Coordinate.x + radius);
        if (endRow > _roomMatrix.GetLength(0) - 1) endRow = _roomMatrix.GetLength(0) - 1;

        int startColum = Mathf.RoundToInt(centerRoom.Coordinate.y - radius);
        if (startColum < 0) startColum = 0;

        int endColum = Mathf.RoundToInt(centerRoom.Coordinate.y + radius);
        if (endColum > _roomMatrix.GetLength(1) - 1) endColum = _roomMatrix.GetLength(1) - 1;

        for (int i = startRow; i < endRow + 1; i++) {
            for (int y = startColum; y < endColum + 1; y++) {
                //If they room is not already an existing room, then add it to the list.
                if (AddToActiveRooms(_roomMatrix[i, y], false) == true && addNonExistingRooms) {
                    roomList.Add(_roomMatrix[i, y]);
                }
                //If the room is an existing room, then add it to the list.
                else if(AddToActiveRooms(_roomMatrix[i, y], false) == false && !addNonExistingRooms) {
                    roomList.Add(_roomMatrix[i, y]);
                }
            }
        }

        return roomList;
    }
    /*Using a recursive algorithm, this function creates a path of rooms to specfied target room.
     It does this by obataining the rooms adjacent to the current path room and then find the room with the smallest
     magnitude, from the target room. That room's room type is then set to empty and is pass in as the new start 
     room, in the recursive call. The recursion ends when the start room matches the target room. O(n)*/
    void CreateRoomsToTarget(Room startRoom,Room targetRoom, List<Room> currentBranch = null){
  
        if (startRoom == targetRoom) return;
        
        Room[] adjacentRooms = ObtainAdjacentRooms(startRoom);
        Room curPathRoom = null;
        Vector2 offSet;
        float shortestSqrLen = 1000;

        for (int i = 0; i < adjacentRooms.Length; i++){
            if (adjacentRooms[i] == null) continue;

            offSet = targetRoom.Coordinate - adjacentRooms[i].Coordinate;

            if(offSet.sqrMagnitude < shortestSqrLen){
                shortestSqrLen = offSet.sqrMagnitude;
                curPathRoom = adjacentRooms[i];
            }
        }

        if(curPathRoom.MyRoomStructureType == Room.RoomStructureType.Unused) curPathRoom.MyRoomStructureType = Room.RoomStructureType.Empty;
        AddToActiveRooms(curPathRoom);
        if (currentBranch != null) currentBranch.Add(curPathRoom);
        CreateRoomsToTarget(curPathRoom, targetRoom, currentBranch);
    }

    //Set bool data of if rooms are connected to each other or not. O(n)
    void InitConnectorData() {

        for (int z = 0; z < _activeRooms.Count; z++){
            _activeRooms[z].InitConectorData(ObtainAdjacentRooms(_activeRooms[z]));
        }
    }

    /*  Obtains references to the rooms that are north, east, south and west of the center room.
      By using the the center's room corrdinate and adding it to a basic vector, like (1,0) or (0,1), 
      we can obtain a vector, whose corrdinates can be used to index the correct room, in the roomMatrix array.
      If the vector is out of bounds of the matrix, it will set that reference to null. O(n)*/
    Room[] ObtainAdjacentRooms(Room centerRoom){
        int adjacentRoomCount = 4;
        Room[] adjacentRooms = new Room[adjacentRoomCount];
        Vector2 adjacentRoomCord = new Vector2();
        
        for (int i = 0; i < adjacentRoomCount; i++){
            switch (i){
                case 0: adjacentRoomCord = centerRoom.Coordinate + Vector2.up; break;
                case 1: adjacentRoomCord = centerRoom.Coordinate + Vector2.down; break;
                case 2: adjacentRoomCord = centerRoom.Coordinate + Vector2.left; break;
                case 3: adjacentRoomCord = centerRoom.Coordinate + Vector2.right; break;
                default: Debug.LogError("Error invaild index."); break;
            }

            if (IsVaildRoomMatrixPosition(adjacentRoomCord) == true) {

                adjacentRooms[i] = _roomMatrix[Mathf.RoundToInt(adjacentRoomCord.x), Mathf.RoundToInt(adjacentRoomCord.y)];
            }
            else{
                adjacentRooms[i] = null;
            }
        }

        return adjacentRooms;
    }

    //Finds the magnitude between two rooms in the room matrix.
    float FindRoomDistance(Room room1, Room room2){
        Vector2 offSet = room1.Coordinate - room2.Coordinate;
        return offSet.sqrMagnitude;
    }


    //Gets list of rooms that are the conners of the room matrix.
    List<Room> GetConerRooms(){
        List<Room> roomRefs = new List<Room>();
        
        roomRefs.Add(_roomMatrix[0, 0]);
        roomRefs.Add(_roomMatrix[0, _roomMatrix.GetLength(1) - 1]);
        roomRefs.Add(_roomMatrix[_roomMatrix.GetLength(0) - 1, 0]);
        roomRefs.Add(_roomMatrix[_roomMatrix.GetLength(0) - 1, _roomMatrix.GetLength(1) - 1]);
        return roomRefs;
    }

    /*Using the conners of the room matrix as starting points, this function picks a random rooms within
     the specified radius of the conner rooms. To determine if the radius value should subtract or added, this 
     fucntion cacaulates the center of the matrix to be used as a reference point. By comparing the center matrix value to
     the coordinate of the current conner room being evaulated for, we can determine which conner it is and thus
     if we need to add or subtact the radius value. O(n)*/
    List<Room> PickRandomConerRoomsInRadius(int radius,List<Room> connerRooms){
        List<Room> roomRefs = new List<Room>();
        Vector2 roomMatrixCenter = new Vector2(Mathf.RoundToInt(_roomMatrix.GetLength(0) / 2),
            Mathf.RoundToInt(_roomMatrix.GetLength(1) / 2));
        int randomX;
        int randomY;

        for (int i = 0; i < connerRooms.Count; i++)
        {
            //By using Mathf.Abs we can guarantee that we are seting radius to postive or negative,
            //regardless of the current state it is in.
            if (connerRooms[i].Coordinate.x < roomMatrixCenter.x) radius = Mathf.Abs(radius);
            else radius = -Mathf.Abs(radius);

            randomX = UnityEngine.Random.Range(Mathf.RoundToInt(connerRooms[i].Coordinate.x),
                Mathf.RoundToInt(connerRooms[i].Coordinate.x + radius) + 1);

            if (connerRooms[i].Coordinate.y < roomMatrixCenter.y) radius = Mathf.Abs(radius);
            else radius = -Mathf.Abs(radius);

            randomY = UnityEngine.Random.Range(Mathf.RoundToInt(connerRooms[i].Coordinate.y),
                Mathf.RoundToInt(connerRooms[i].Coordinate.y + radius) + 1);

            roomRefs.Add(_roomMatrix[randomX, randomY]);
        }

        return roomRefs;
    }

    /*This function determines the room enocunters for the hallway rooms. A hallway room is a room between the start room
     and key rooms.  When create the hallway rooms, we need to make sure that we don't pick the same rooms twice.
     In order to do this, we create a copy of refferences from the active rooms. Once a room encounter
     type has been chosen, we remove that room from roomPickPool. O(n)*/
    void RollHallwayRoomTypes(){
        List<Room> roomPickPool = new List<Room>();

        for (int i = 0; i < _activeRooms.Count; i++){
            if (_activeRooms[i].MyRoomStructureType != Room.RoomStructureType.Key &&
                _activeRooms[i].MyRoomStructureType != Room.RoomStructureType.Start) {

                roomPickPool.Add(_activeRooms[i]);
            }
        }
        
        RollRoomEncounterType(Room.RoomEncounterType.Combat, 
            Mathf.RoundToInt(_activeRooms.Count * 0.25f), roomPickPool);
        RollRoomEncounterType(Room.RoomEncounterType.Healing,
            Mathf.RoundToInt(_activeRooms.Count * 0.05f), roomPickPool);
    }

    /*This function determines the encounters for they key rooms. A key room is a room in outer perimeter of the floor.
     They are reserved for special room enounters, like the boss encounter. We remove a room from the roomPickPool list
     every time a new room encounter has been picked to avoid assigning a new room encounter multiple times. O(n)*/
    public void RollKeyRooms() {
        List<Room> roomPickPool = ObtainKeyRooms();
        List<Room.RoomEncounterType> keyRoomEncounters = ObatainKeyRoomEncounters();
        int count = keyRoomEncounters.Count - 1;
        int RNG;

        //Need to insure we have a boss room.
        RollRoomEncounterType(Room.RoomEncounterType.Boss, 1, roomPickPool);

        while(count >= 0) {
            RNG = UnityEngine.Random.Range(0, keyRoomEncounters.Count);

            RollRoomEncounterType(keyRoomEncounters[RNG], 1, roomPickPool);
            keyRoomEncounters.RemoveAt(RNG);
            count--;
        }
    }

    //Obatains a list of key room encounters. O(n)
    List<Room.RoomEncounterType> ObatainKeyRoomEncounters() {
        List<Room.RoomEncounterType> keyRoomEcounters = new List<Room.RoomEncounterType>();

        foreach (Room.RoomEncounterType value in Room.RoomEncounterType.GetValues(typeof(Room.RoomEncounterType))) {
            if(value != Room.RoomEncounterType.None && value != Room.RoomEncounterType.Boss &&
                value != Room.RoomEncounterType.Combat && value != Room.RoomEncounterType.Healing &&
                value != Room.RoomEncounterType.Portal) {
     
                keyRoomEcounters.Add(value);
            }           
        }

        return keyRoomEcounters;
    }

    //Gets a list of all key rooms in the floor. O(n)
    public List<Room> ObtainKeyRooms() {
        List<Room> keyRooms = new List<Room>();

        for (int i = 0; i < _activeRooms.Count; i++) {
            if (_activeRooms[i].MyRoomStructureType == Room.RoomStructureType.Key) keyRooms.Add(_activeRooms[i]);
        }

        return keyRooms;
    }

    //Randomly assign an encounter to a room then removes that room from the roompick pool. O(n)
    void RollRoomEncounterType(Room.RoomEncounterType newRoomEncounterType, int newRoomCount, List<Room> roomPickPool){
        if (roomPickPool.Count == 0) return;
        int randomIndex;

        for (int i = 0; i < newRoomCount; i++){
            randomIndex = UnityEngine.Random.Range(0, roomPickPool.Count);
            roomPickPool[randomIndex].MyRoomEncounterType = newRoomEncounterType;
            roomPickPool.RemoveAt(randomIndex);
            if (roomPickPool.Count == 0) return;
        }
    }

    /*This function assign the rooms their elmental type. It does this by frist finding out how many rooms of each element
     we want. This is calculated by multiplying the total active rooms by a floor elemental biase. For example, if we have
     12 total active room and a fire biase of 0.25f then there would be 4 fire rooms. 12 * 0.25f = 4. We then loop through each
     element and randomly pick a room from the room pick pool and give that room an elemental type. Once that room has an elemental
     type, we then remove it from the room pick pool. Sometimes, there are less elemetal rooms then the total active rooms. This is
     cause by the rounding of values produced by the Mathf.RoundToInt() function. We fix this by counting the total elemental rooms created
     and storing it in the sumOfNewRoomsElementalType variable. If sumOfNewRoomsElementalType is less then total active rooms, then
     we take the rooms remaining in the room pick pool and assign them a random element. O(n + m)*/
    void DecideElmentalRoomBiase() {

        List<Room> roomPickPool = new List<Room>();
        int fireRoomCount = Mathf.RoundToInt(_activeRooms.Count * _floorElementalBiase.FireBiase);
        int waterRoomCount = Mathf.RoundToInt(_activeRooms.Count * _floorElementalBiase.WaterBiase);
        int earthRoomCount = Mathf.RoundToInt(_activeRooms.Count * _floorElementalBiase.EarthBiase);
        int windRoomCount = Mathf.RoundToInt(_activeRooms.Count * _floorElementalBiase.WindBiase);

        int sumOfNewRoomsElementalType = fireRoomCount + waterRoomCount + earthRoomCount + windRoomCount;
        //Debug.Log("sumOfNewRoomsElementalType: " + sumOfNewRoomsElementalType + "  _activeRooms.Count: " + _activeRooms.Count);

        for (int i = 0; i < _activeRooms.Count; i++) {roomPickPool.Add(_activeRooms[i]);}

        foreach (var value in Elemental.ElementalIdentity.GetValues(typeof(Elemental.ElementalIdentity))) {
            switch ((Elemental.ElementalIdentity)value) {
                case Elemental.ElementalIdentity.Fire: RollRoomElementalType((Elemental.ElementalIdentity)value, fireRoomCount, roomPickPool); break;
                case Elemental.ElementalIdentity.Earth: RollRoomElementalType((Elemental.ElementalIdentity)value, earthRoomCount, roomPickPool); break;
                case Elemental.ElementalIdentity.Water: RollRoomElementalType((Elemental.ElementalIdentity)value, waterRoomCount, roomPickPool); break;
                case Elemental.ElementalIdentity.Wind: RollRoomElementalType((Elemental.ElementalIdentity)value, windRoomCount, roomPickPool); break;
            }
        }

        if (sumOfNewRoomsElementalType < _activeRooms.Count) {
            int count = roomPickPool.Count - 1;
            int randomIndex;
            //Debug.Log("Not enought element bias rooms active. roomPickPool.Count: " + roomPickPool.Count);
            while (count >= 0) {
                randomIndex = UnityEngine.Random.Range(0, roomPickPool.Count);
                roomPickPool[randomIndex].RoomElementalIdentity = Elemental.PickRandomElementalIdentity();
                roomPickPool.RemoveAt(randomIndex);
                count--;
            }
        }
    }

    /*Error:Shoud be called AddToFloorBiase.
      Used during the portal room scene when altering the next floor's attributes. Adds more elemental biase, 
      of a particular type , to the next floor.*/
    public void AddToRoomBiase(Elemental.ElementalIdentity arg) {
        _floorElementalBiase.IndcrementBiase(arg);
        DecideElmentalRoomBiase();
    }

    //Assign a room an elemental type and then removes that room from the room pick pool. 
    void RollRoomElementalType(Elemental.ElementalIdentity newRoomElementalType, int newRoomCount, List<Room> roomPickPool) {
        if (roomPickPool.Count == 0) return;
        int randomIndex;

        for (int i = 0; i < newRoomCount; i++) {
            randomIndex = UnityEngine.Random.Range(0, roomPickPool.Count);
            roomPickPool[randomIndex].RoomElementalIdentity = newRoomElementalType;
            roomPickPool.RemoveAt(randomIndex);
            if (roomPickPool.Count == 0) return;
        }
    }

    //Create a list of monsters for the beast master room encounter. 
    void InitBeastMasterMonsters() {
        _beastMasterMonsters = new List<MonsterData>();
        MonsterData newMonsterData;

        for (int i = 0; i < PlayerData.MAX_MONSTER_EQUIP; i++) {
            newMonsterData = new MonsterData();
            newMonsterData.InitData(MonsterData.PickRandomMonsterIdenity());
            newMonsterData.ControlledLevelUp(_floorLevel);
            _beastMasterMonsters.Add(newMonsterData);
        }
    }
}
