using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls runes images display in the user interface. 
public class RuneDisplay : MonoBehaviour {

    public List<GameObject> runeImages;

	// Use this for initialization
	void Start () {
        UpdateRuneDisplay();
    }

    //Gets reffereces to the rune display's object runes images and updates them with the player's data.    
    public void UpdateRuneDisplay() {
        List<Image> imageGroup = new List<Image>();
        List<Text> textGroup = new List<Text>();

        for (int i = 0; i < runeImages.Count; i++) {
            imageGroup.Add(runeImages[i].GetComponent<Image>());
            textGroup.Add(runeImages[i].GetComponentInChildren<Text>());
        }

        GameManager.instance.PlayerInfo.UpdatePlayerRuneImages(textGroup, imageGroup);
    }
}
