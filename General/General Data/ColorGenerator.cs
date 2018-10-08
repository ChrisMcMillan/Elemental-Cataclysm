using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generates a object color base on RGB value
public static class ColorGenerator {

    static float maxRange = 255;

    public static Color GenerateColor(float r, float g, float b) {
        Color c;
        c = new Color(r / maxRange, g / maxRange, b / maxRange);
        return c;
    }
}
