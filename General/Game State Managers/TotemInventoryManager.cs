using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Display information about totems the player has acquired.
public class TotemInventoryManager : MonoBehaviour {

    public TotemDisplay totemDisplayRef;
    public GameObject rightArrow;
    public GameObject leftArrow;
    public Text noTotemText;

    int _playerTotemsIndex;
    List<TotemData> _playerTotemsRef;

    //If the player has no totems this function display message to inform the player.
    //Otherwise, it displays the frist totem in their inventory. 
    void Start () {

        _playerTotemsIndex = 0;
        _playerTotemsRef = GameManager.instance.PlayerInfo.PlayerTotems;

        if(_playerTotemsRef.Count > 0) {
            totemDisplayRef.UpdateTotemDisplay(_playerTotemsRef[_playerTotemsIndex]);
            noTotemText.gameObject.SetActive(false);
        }
        else {
            totemDisplayRef.gameObject.SetActive(false);
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            noTotemText.gameObject.SetActive(true);
        }
	}

    //Cyclces through the player's totems.
    public void RightArrowEvent() {
        _playerTotemsIndex++;
        if (_playerTotemsIndex > _playerTotemsRef.Count - 1) _playerTotemsIndex = 0;
        totemDisplayRef.UpdateTotemDisplay(_playerTotemsRef[_playerTotemsIndex]);
    }

    //Cyclces through the player's totems.
    public void LeftArrowEvent() {
        _playerTotemsIndex--;
        if (_playerTotemsIndex < 0) _playerTotemsIndex = _playerTotemsRef.Count - 1;
        totemDisplayRef.UpdateTotemDisplay(_playerTotemsRef[_playerTotemsIndex]);
    }

    //Returns the player to the player status scene.
    public void ReturnButtonEvent() {
        GameManager.instance.ChangeGameState(GameManager.GameState.PlayerStatus);
    }
}
