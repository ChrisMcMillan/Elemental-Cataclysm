using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Shows icons to indicate what status ailments are currently 
 affecting the monster.*/
public class StatusAilmentDisplay : MonoBehaviour {

    public GameObject buringIcon;
    public GameObject posinIcon;
    public GameObject regenIcon;
    public GameObject attributeBuffIcon;
    public GameObject stunIcon;

    void Awake() {
        HideAllIcons();
    }

    // Use this for initialization
    void Start () {
       
    }
	
	void HideAllIcons() {
        buringIcon.SetActive(false);
        posinIcon.SetActive(false);
        regenIcon.SetActive(false);
        attributeBuffIcon.SetActive(false);
        stunIcon.SetActive(false);
    }

    /*Loops through a monster's ailments and displays the icon that
     corresponds with that ailment.*/
    public void UpdateStatusAilmentDisplayIcons(List<StatusAilment> arg) {
        HideAllIcons();

        for (int i = 0; i < arg.Count; i++) {
            switch (arg[i].AlimentType) {
                case StatusAilment.StatusAilmentType.Poison: posinIcon.SetActive(true); break;
                case StatusAilment.StatusAilmentType.Burning: buringIcon.SetActive(true); break;
                case StatusAilment.StatusAilmentType.Regen: regenIcon.SetActive(true); break;
                case StatusAilment.StatusAilmentType.AttributeBuff: attributeBuffIcon.SetActive(true); break;
                case StatusAilment.StatusAilmentType.Stun: stunIcon.SetActive(true); break;
                default: Debug.LogError("Error: Status ailment icon display has not been define for " + arg[i].AlimentType); break;
            }
        }
    }
}
