/*
The game manger hold all the game data and communicates the data to the diffent systems of the game.  The singleton design pattern
makes it easy communicate that data because you only need to make a static call to the one instance of the game manager and obtain any data you
need from the instance.  
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //These enum is use to represent the current scene of the game.
    public enum GameState { FloorNavigation, PlayerStatus,  MainMenu, Defeat, Credits, TotemInventory, Ad, HowToPlay};

    //This class is use to associate a game state with a index. SceneManager.LoadScene() requires a intger to change game scences and this class
    //makes sure the correct intger is associate with the correct game scence.    
    public class GameStateSceneData {
        public object gameState;
        public int index;
    }

    List<GameStateSceneData> _gameStateSceneDataList;

    public static GameManager instance = null;
    // We also use RoomEncounterType enum to repersent game scences. So, we set _currentGameState to object data type
    // so it can be a GameState enum or a RoomEncounterType enum.
    object _currentGameState;
    object _previousGameState;

    MusicManager musicManagerRef;
    SceneFadeAnimation _sceneFadeScript;
    GameResources _gameResorcesRef;

    PlayerData _playerInfo = new PlayerData();
    Dungeon _dugeonData = new Dungeon();

    public object CurrentGameState {
        get {
            return _currentGameState;
        }
    }

    public PlayerData PlayerInfo {
        get {
            return _playerInfo;
        }
    }

    public GameResources GameResorcesRef {
        get {
            return _gameResorcesRef;
        }
    }

    public SceneFadeAnimation SceneFadeScript {
        get {
            return _sceneFadeScript;
        }
    }

    public Dungeon DugeonData {
        get {
            return _dugeonData;
        }
    }

    public object PreviousGameState {
        get {
            return _previousGameState;
        }
        set {
            _previousGameState = value;
        }
    }

    public MusicManager MusicManagerRef {
        get {
            return musicManagerRef;
        }
    }

    //If the static instance of GameManager is null then set it this script. Else, destory the duplicate instance of GameManager.   
    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitGameData();
        }

        else if (instance != this) {
            Destroy(gameObject);
        }     
    }

    //Initializes game data. 
    void InitGameData() {
        Debug.Log("InitGameData() called");
        musicManagerRef = GetComponentInChildren<MusicManager>();
        _sceneFadeScript = GetComponentInChildren<SceneFadeAnimation>();
        _gameResorcesRef = GetComponentInChildren<GameResources>();

        InitGameStateSceneDataList();

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        _currentGameState = FindGameStateForIndex(sceneIndex);
        _previousGameState = GameState.FloorNavigation;

        StartGame(true);
    }

    //  Initlize player and dugeon data. For easier debuging, we want to use fuction no matter what scene we start in. So, 
    // use a bool to control when we change to the FloorNavigation game state. 
    public void StartGame(bool testingMode) {
        Debug.Log("StartGame() called");
        _dugeonData.InitData(6);
        _playerInfo.InitData(_dugeonData.CurrentFloor.PlayerStartSoul());
        if(testingMode == false) ChangeGameState(GameState.FloorNavigation);
    }

    //Makes sure no matter what game scene we start in, the correct index is associate the corresponding game scene
    void InitGameStateSceneDataList() {
        _gameStateSceneDataList = new List<GameStateSceneData>();
        GameStateSceneData temp;
     
        foreach (GameState value in GameState.GetValues(typeof(GameState))) {
            temp = new GameStateSceneData();

            switch (value) {
                case GameState.MainMenu: temp.index = 0; break;
                case GameState.Defeat: temp.index = 11; break;
                case GameState.FloorNavigation: temp.index = 1; break;
                case GameState.PlayerStatus: temp.index = 3; break;
                case GameState.Credits: temp.index = 12; break;
                case GameState.TotemInventory: temp.index = 13; break;
                case GameState.Ad: temp.index = 14; break;
                case GameState.HowToPlay: temp.index = 15; break;
                default: Debug.LogError("Index not defined for " + value); break;
            }

            temp.gameState = value;
            _gameStateSceneDataList.Add(temp);
        }

        foreach (Room.RoomEncounterType value in Room.RoomEncounterType.GetValues(typeof(Room.RoomEncounterType))) {
            if (value == Room.RoomEncounterType.None) continue;

            temp = new GameStateSceneData();

            switch (value) {
                case Room.RoomEncounterType.Combat: temp.index = 2; break;
                case Room.RoomEncounterType.Boss: temp.index = 2; break;
                case Room.RoomEncounterType.BeastMaster: temp.index = 4; break;
                case Room.RoomEncounterType.Healing: temp.index = 5; break;
                case Room.RoomEncounterType.Portal: temp.index = 6; break;
                case Room.RoomEncounterType.ShrineOfWisdom: temp.index = 7; break;
                case Room.RoomEncounterType.ShrineOfSacrifice: temp.index = 8; break;
                case Room.RoomEncounterType.ShrineOfPower: temp.index = 9; break;
                case Room.RoomEncounterType.ShrineOfKnowledge: temp.index = 10; break;
                default: Debug.LogError("Index not defined for " + value); break;
            }

            temp.gameState = value;
            _gameStateSceneDataList.Add(temp);
        }
    }

    //Finds the index associated with an game state. 
    int FindndexForGameState(object arg) {

        for (int i = 0; i < _gameStateSceneDataList.Count; i++) {

            if(arg.GetType() == typeof(GameState)) {
                if (_gameStateSceneDataList[i].gameState.GetType() != typeof(GameState)) continue;
                if ((GameState)_gameStateSceneDataList[i].gameState == (GameState)arg) return _gameStateSceneDataList[i].index;
            }
            else {
                if (_gameStateSceneDataList[i].gameState.GetType() != typeof(Room.RoomEncounterType)) continue;
                if ((Room.RoomEncounterType)_gameStateSceneDataList[i].gameState == (Room.RoomEncounterType)arg) return _gameStateSceneDataList[i].index;
            }
        }

        Debug.LogError("Game state index not define for " + arg);
        return -1;
    }

    //Finds the game state associated with an index.
    object FindGameStateForIndex(int arg) {

        for (int i = 0; i < _gameStateSceneDataList.Count; i++) {
            if (_gameStateSceneDataList[i].index == arg) return _gameStateSceneDataList[i].gameState;
        }

        Debug.LogError("Game state index not define for " + arg);
        return null;
    }

    //Actives a fade animation when changing game scences and updates the music. 
    public void ChangeGameState(object newGameState) {

        _currentGameState = newGameState;
       
        musicManagerRef.UpdateMusic();

        _sceneFadeScript.fadeTolevel(FindndexForGameState(_currentGameState));
    }
}
