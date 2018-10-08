using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages which music is played at certain scenes.
public class MusicManager : MonoBehaviour {

    public AudioClip combatClip;
    public AudioClip floorClip;

    AudioSource audioRef;

    public AudioSource AudioRef {
        get {
            return audioRef;
        }
    }

    void Awake() {
        audioRef = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        
        UpdateMusic();
    }

    //Changes the music during combat, otherwise plays the default music. 
    public void UpdateMusic() {

        Room playerRoom = GameManager.instance.DugeonData.CurrentFloor.PlayerRoom;

        if((playerRoom.MyRoomEncounterType == Room.RoomEncounterType.Combat || playerRoom.MyRoomEncounterType == Room.RoomEncounterType.Boss) && 
            playerRoom.RoomEncounterCleared == false) {

            audioRef.clip = combatClip;
        }
        else {
            if (audioRef.clip == floorClip) return;
            audioRef.clip = floorClip;
        }

        audioRef.Play();
    }
}
