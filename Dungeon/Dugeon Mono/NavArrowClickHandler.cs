using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Use by the navigation arrows, in the floor navigation scene, to let the player move
throughtout the floor.*/ 
public class NavArrowClickHandler : MonoBehaviour {

    public enum ArrowIdentity { Up,Down,Left,Right};

    public ArrowIdentity myArrowIdentity;

    /*The navigation arrows are assigned an arrow identity and their arrow identiy
     is pass into the NavArrowButtonEvent() fuction of the current floor to move them
     to the next room.*/
    public void arrowClickEvent()
    {
        GameManager.instance.DugeonData.CurrentFloor.NavArrowButtonEvent(myArrowIdentity);
    }
}
