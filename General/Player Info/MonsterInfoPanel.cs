using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contain reference to the monster panel's parts    
public class MonsterInfoPanel : MonoBehaviour {
    public Image monsterPicture;
    public Text monsterName;
    public List<Image> abilityIcons;
    public Text monsterLevel;
    public Slider healthSlider;
    public Slider expSlider;
    public Text strText;
    public Text agiText;
    public Text spriText;
    public Text speedText;
    public Image panelBackgroundImage;
}
