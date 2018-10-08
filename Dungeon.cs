using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages over all data realted the dungeon, a group of floors. 
public class Dungeon {

    List<Floor> _floorsList;
    int minFloorSize;
    int maxFloorSize;
    int _floorCount;

    Floor _currentFloor;
    Floor _nextFloor;

    public Floor CurrentFloor {
        get {
            return _currentFloor;
        }
    }

    public Floor NextFloor {
        get {
            return _nextFloor;
        }
    }

    public List<Floor> FloorsList {
        get {
            return _floorsList;
        }
    }

    //Initializes the data for the dugeon.  
    public void InitData(int maxFloorCountArg) {
        minFloorSize = 5;
        maxFloorSize = 6;
        _floorCount = 0;

        InitFloorsList(maxFloorCountArg);

        _currentFloor = _floorsList[_floorCount];
        _currentFloor.CreateFloor();
        _nextFloor = _floorsList[_floorCount + 1];
        _nextFloor.CreateFloor();
    }

    //Get the player's current dugeon progress as percent. 
    public float ObtainDungeonCompletionPercent() {
        float dugeonCompletionPercent = (float)_floorCount / (float)_floorsList.Count;
        return dugeonCompletionPercent;
    }

    //Creates list of totems to be rewards after boss fights. 
    List<TotemData> CreateDungeonTotemList() {
        List<TotemData> newTotemList = new List<TotemData>();
        TotemData temp;

        foreach (TotemData.TotemIdentity value in TotemData.TotemIdentity.GetValues(typeof(TotemData.TotemIdentity))) {
            temp = new TotemData(value);
            newTotemList.Add(temp);
        }

        return newTotemList;
    }

    /*Create a list of floors that become the whole dungeon. Evey other floor is given
     a unique boss encouter. This function also gives a different totem reward for each boss fight.*/ 
    void InitFloorsList(int maxFloorCount) {
        _floorsList = new List<Floor>();
        List<UniqueBossData.UniqueBossIdentity> bossList = UniqueBossData.GetUniqueBossIdentityList();
        List<TotemData> dungeonTotemList = CreateDungeonTotemList();
        Floor newFloor;
        int uniqueFloorDis = 2;
        int nextUniqueFloor = uniqueFloorDis - 1;
        int floorSizeRNG;
        int totemPickRNG;
        int bossPicKRNG;

        for (int i = 0; i < maxFloorCount; i++) {
            floorSizeRNG = UnityEngine.Random.Range(minFloorSize, maxFloorSize + 1);

            newFloor = new Floor();
            
            if (i == nextUniqueFloor) {
                   
                totemPickRNG = UnityEngine.Random.Range(0, dungeonTotemList.Count);
                UniqueBossData.UniqueBossIdentity newIden;

                if (nextUniqueFloor == maxFloorCount - 1) {
                    newIden = UniqueBossData.UniqueBossIdentity.Shifter;
                }
                else {
                    bossPicKRNG = UnityEngine.Random.Range(0, bossList.Count);
                    newIden = bossList[bossPicKRNG];
                    bossList.RemoveAt(bossPicKRNG);
                }

                newFloor.InitData(i + 1, floorSizeRNG, newIden, dungeonTotemList[totemPickRNG]);

                dungeonTotemList.RemoveAt(totemPickRNG);
               
                nextUniqueFloor += uniqueFloorDis;
                if (nextUniqueFloor > maxFloorCount - 1) nextUniqueFloor = maxFloorCount - 1;
            }

            else {
                
                newFloor.InitData(i + 1, floorSizeRNG, UniqueBossData.UniqueBossIdentity.NONE);  
            }

            _floorsList.Add(newFloor);
        }
    }

    //Allows the player to go to the next floor in the portal room scene. It also creates the next floor when called. 
    public void GoToNextFloor() {

        _floorCount++;

        _currentFloor = _nextFloor;

        if (_floorCount + 1 < _floorsList.Count) {
            _nextFloor = _floorsList[_floorCount + 1];
            _nextFloor.CreateFloor();
        }
        else {
            _nextFloor = null;
        }

        GameManager.instance.ChangeGameState(GameManager.GameState.FloorNavigation);
      
    }
}
