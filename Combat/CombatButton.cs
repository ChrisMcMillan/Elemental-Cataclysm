using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A combat button can either display infomation about a monster or display a monster's abilities. 
public class CombatButton : MonoBehaviour {

    public Text monsterName;
    public Slider monsterHealthSilder;
    public List<Image> abilityIcons;

    Image imageRef;

    public Image ImageRef {
        get {
            return imageRef;
        }
    }

    // Use this for initialization
    void Start () {
        imageRef = GetComponent<Image>();
    }

    //If the player does not have a full party or if a monster dies during combat, then this function dispalys the none icon.
    public void LoadNoneIcon() {
        imageRef.sprite = GameManager.instance.GameResorcesRef.LoadNoneIcon();
        monsterName.gameObject.SetActive(false);
        monsterHealthSilder.gameObject.SetActive(false);
        for (int i = 0; i < abilityIcons.Count; i++) {
            abilityIcons[i].gameObject.SetActive(false);
        }
    }

    //Displays a monster's abilities. 
    public void LoadAbilityIcon(Ability.AbilityIdentity arg) {
        monsterName.gameObject.SetActive(false);
        monsterHealthSilder.gameObject.SetActive(false);

        for (int i = 0; i < abilityIcons.Count; i++) {
            abilityIcons[i].gameObject.SetActive(false);
        }

        imageRef.sprite = GameManager.instance.GameResorcesRef.FindAbilityIcons(arg);
    }

}
