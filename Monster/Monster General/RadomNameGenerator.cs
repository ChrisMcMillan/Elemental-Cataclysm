using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//List of possible names for monsters.
public static class RadomNameGenerator{

    static List<string> _randomNames = new List<string>() {
        "Shadewing", "Fetidstrike", "Brulvorok", "Bor'anuth","Zikumith","Gagdrech","Argarin","Marumath","Ral'gauz","Vazrannon","Dror'gamith",
        "Marquez", "Maw","Mog'drith", "Elguzok", "Ulgen", "Gog'thannur", "Xal'gonneg", "Tolrez", "Xornaan", "Sith'tuuth", "Zalgudon", "Xustroren",
        "Vith'toroch", "Ozroth", "Mag'thaxal", "Doth'tazan", "Maronoch", "Joz'gerag", "Mazrath", "Zekallath", "Broglemur", "Ruzrenod", "Uriks",
        "Urshath", "Monshobiph", "Taelkizox", "Gelodkoth", "Kokroks", "Bocnaek", "Ocdriz", "Strugekthez", "Nitrax", "Hodorphu", "Strugekthez"
    };

    public static List<string> RandomNames {
        get {
            return _randomNames;
        }
    }
}
