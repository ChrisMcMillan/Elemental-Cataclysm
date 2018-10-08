using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* A floor's elemental baise determine the elemental type of each individual room, 
 which can have numerous gameplay effects. For example, if a floor's  elemental baise is 10% fire, 60% water, 10% earth
 and 10% wind; the floor will have mostly water rooms.*/
public class FloorElementalBiases{

    static float _biaseIncrement = 0.25f;

    float _fireBiase;
    float _waterBiase;
    float _earthBiase;
    float _windBiase;

    public float FireBiase {
        get {
            return _fireBiase;
        }
    }

    public float WaterBiase {
        get {
            return _waterBiase;
        }
    }

    public float EarthBiase {
        get {
            return _earthBiase;
        }
    }

    public float WindBiase {
        get {
            return _windBiase;
        }
    }

    public FloorElementalBiases(float fireBiaseArg, float waterBiaseArg, float earthBiaseArg, float windBiaseArg) {
        _fireBiase = fireBiaseArg;
        _waterBiase = waterBiaseArg;
        _earthBiase = earthBiaseArg;
        _windBiase = windBiaseArg;

        CorrectBiases();
    }

    /*Creates a preset of the specified element. For example if the agrument passed into this function was fire, then
     it would create a preste of 0.5 fire and 0.17 of the other elements.*/
    public static FloorElementalBiases CreatePresetOfType(Elemental.ElementalIdentity arg) {
        float strongBiase = 0.50f;
        float weakBiase = 0.17f;

        switch (arg) {
            case Elemental.ElementalIdentity.Wind: return new FloorElementalBiases(weakBiase, weakBiase, weakBiase, strongBiase);
            case Elemental.ElementalIdentity.Earth: return new FloorElementalBiases(weakBiase, weakBiase, strongBiase, weakBiase);
            case Elemental.ElementalIdentity.Water: return new FloorElementalBiases(weakBiase, strongBiase, weakBiase, weakBiase);
            case Elemental.ElementalIdentity.Fire: return new FloorElementalBiases(strongBiase, weakBiase, weakBiase, weakBiase);
            default: Debug.LogError("Could not find preset for " + arg); return null;
        }
    }

    //This function makes sure that all the biases always add up to 1.0f or 100%. 
    void CorrectBiases() {
        float biasesSum = _fireBiase + _waterBiase + _earthBiase + _windBiase;
        float scaler = 1.0f / biasesSum;

        _fireBiase *= scaler;
        _waterBiase *= scaler;
        _earthBiase *= scaler;
        _windBiase *= scaler; 
    }

    //Used in the portal scene. This function displays a rooom's elemntal bias on room biase buttons. 
    public void UpdateFloorElementalBiaseButton(List<RoomBiaseButton> buttontGroup) {

        int count = 0;
        string strTemp = "";

        foreach (Elemental.ElementalIdentity item in Elemental.ElementalIdentity.GetValues(typeof(Elemental.ElementalIdentity))) {
            if (item == Elemental.ElementalIdentity.NONE) continue;
            strTemp = item.ToString() + " Rooms: %";

            switch (item) {
                case Elemental.ElementalIdentity.Wind: strTemp += _windBiase.ToString("F2"); break;
                case Elemental.ElementalIdentity.Earth: strTemp += _earthBiase.ToString("F2"); break;
                case Elemental.ElementalIdentity.Water: strTemp += _waterBiase.ToString("F2"); break;
                case Elemental.ElementalIdentity.Fire: strTemp += _fireBiase.ToString("F2"); break;
                default: Debug.LogError("Error: no biase for: " + item); break;
            }

            buttontGroup[count].ButtonText.text = strTemp;
            buttontGroup[count].ButtonText.color = Elemental.FindElementalColor(item);
            buttontGroup[count].ButtonElementalType = item;
            count++;
        }
    }

    //Add more biase of the specified element to the floor elemental biase. 
    public void IndcrementBiase(Elemental.ElementalIdentity arg) {

        switch (arg) {
            case Elemental.ElementalIdentity.Wind: _windBiase += _biaseIncrement; break;
            case Elemental.ElementalIdentity.Earth: _earthBiase += _biaseIncrement; break;
            case Elemental.ElementalIdentity.Water: _waterBiase += _biaseIncrement; break;
            case Elemental.ElementalIdentity.Fire: _fireBiase += _biaseIncrement; break;
            default: Debug.LogError("Error can't find biase for " + arg); break;
        }

        CorrectBiases();
    }
}
