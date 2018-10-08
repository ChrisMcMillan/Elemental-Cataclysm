using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


//Responsible for displaying ads to user and rewarding them.
public class AdManager : MonoBehaviour {

    //If the add is ready, this function will show an add to the player.
    public void ShowAd() {

        if (Advertisement.IsReady("rewardedVideo")) {
            Advertisement.Show("rewardedVideo", new ShowOptions() {resultCallback = HandleAdResult});
        }
    }

    /*Display an message to console depending how what happends with the ad.
    If player watches the ad, then they will get 3 souls.*/
    void HandleAdResult(ShowResult result) {
        switch (result) {
            case ShowResult.Failed:
                Debug.LogError("Player failed to launch ad.");
                break;
            case ShowResult.Skipped:
                Debug.Log("Player has skipped ad.");
                break;
            case ShowResult.Finished:
                Debug.Log("Player has finished watching ad.");
                GameManager.instance.PlayerInfo.Soul += 3;       
                break;
        }

        GameManager.instance.DugeonData.GoToNextFloor();
    }

    //Allows the player to go the next floor.
    public void ContinueButtonEvent() {
        GameManager.instance.DugeonData.GoToNextFloor();
    }
}
