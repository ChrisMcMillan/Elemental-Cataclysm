using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls a monster's animations during combat. 
public class MonsterAnimator : MonoBehaviour {

    public Slider healthSlider;
    public Transform feetPosition;
    public RectTransform canvasRectTransform;
    public CombatAnnouncementText combatAnnouncementText;
    public Animator abilityEffectAnimator;
    public StatusAilmentDisplay ailementDisplay;

    Animator _myAnimatorRef;

    public Animator MyAnimatorRef {
        get {
            return _myAnimatorRef;
        }
    }

    // Use this for initialization
    void Start () {
     
        _myAnimatorRef = GetComponent<Animator>();
    }

    /*The enemy monster needs to be flip so it is facing the right direction. But we also
    need to flip it's canvas again so the health bar is going in the right direction.*/ 
    public void FlipSkeleton() {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;

        newScale = canvasRectTransform.localScale;
        newScale.x *= -1;
        canvasRectTransform.localScale = newScale;

        newScale = ailementDisplay.gameObject.transform.localScale;
        newScale.x *= -1;
        ailementDisplay.gameObject.transform.localScale = newScale;
    }

    //Can increase or decrease a skeleton's size base on size sacaler.
    public void ChangeSkeletonSize(float sizeScaler) {
        Vector3 newScale = transform.localScale;
        newScale.x *= sizeScaler;
        newScale.y *= sizeScaler;
        transform.localScale = newScale;
    }
        
    /*When monster is moved to a target position, the fuction moves the monster
     so its feet are at center of the target position.*/
    public void CorrectPosition() {
        Vector3 offSet = transform.position - feetPosition.position;
        transform.position += offSet;
    }
}
