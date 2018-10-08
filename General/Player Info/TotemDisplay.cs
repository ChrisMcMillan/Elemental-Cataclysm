using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Manages the totem's display in the user interface. 
public class TotemDisplay : MonoBehaviour {

    public Text totemName;
    public Image totemPicture;
    public Text totemDescription;


    //Update a totem's display in the user interface base on its data. 
    public void UpdateTotemDisplay(TotemData arg) {

        totemName.text = "Totem of " + arg.MyIdentity.ToString();
        totemDescription.text = arg.Description;
        totemPicture.sprite = GameManager.instance.GameResorcesRef.FindTotemIcon(arg.MyIdentity);
    }

}
