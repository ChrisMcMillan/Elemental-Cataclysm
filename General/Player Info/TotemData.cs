using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages data associated with totems 
public class TotemData{

    public enum TotemIdentity { Grace, Bloodlust, Greed, Runes, Reclamation};

    TotemIdentity _myIdentity;
    string _description;

    public TotemIdentity MyIdentity {
        get {
            return _myIdentity;
        }
    }

    public string Description {
        get {
            return _description;
        }
    }

    //Initialize totem data 
    public TotemData(TotemIdentity arg) {
        _myIdentity = arg;
        InitDescription(_myIdentity);
    }

    //Set the totem description associated with a totem identity.
    void InitDescription(TotemIdentity arg) {
        switch (arg) {
            case TotemIdentity.Grace: _description = "Moving into wind rooms cost no soul but moving into fire rooms cost 2 soul."; break;
            case TotemIdentity.Bloodlust: _description = "All monsters get healed after a battle."; break;
            case TotemIdentity.Greed: _description = "Every battle won with the same monster increases the exp reward."; break;
            case TotemIdentity.Runes: _description = "Doubles rune rewards from battles but can only get 2 soul from battles."; break;
            case TotemIdentity.Reclamation: _description = "Gain soul when your monsters die in battle."; break;
            default: Debug.LogError("Errror: Description not define for " + arg); break;
        }
    }
}
