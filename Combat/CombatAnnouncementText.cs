using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Controls the text that displays during combat. This text is displayed in an animation. We could get multiple text orders that are ressived
 before the text animation is finished, so need to use a queue to store those orders.*/
public class CombatAnnouncementText : MonoBehaviour {

    public Text announcementText;

    Animator _myAnimator;
    Queue announcementQueue = new Queue();

    //Use to store infomation about what text display during combat and what color that text should be. 
    public class AnnouncementTextInfo {
        public string text;
        public Color textColor;

        public AnnouncementTextInfo(string textArg, Color textColorArg) {
            text = textArg;
            textColor = textColorArg;
        }
    }

    // Use this for initialization
    void Start () {
        _myAnimator = GetComponent<Animator>();
    }

    //Add a new AnnouncementTextInfo to the queue. If it is the frist one in the queue, then it begins the animation cycle.  
    public void AddToAnnouncementQueue(AnnouncementTextInfo newTextInfo) {

        announcementQueue.Enqueue(newTextInfo);

        if (announcementQueue.Count == 1) PlayAnnouncementText();
    }

    //Use as animation event. At the end of the animation, if there another AnnouncementTextInfo in the queue, it begin the animation cycle again.
    public void CheckAnnouncementQueue() {

        announcementQueue.Dequeue();

        if (announcementQueue.Count != 0) PlayAnnouncementText();
    }

    //Play the animation with information held by the AnnouncementTextInfo in front of the queue.
    void PlayAnnouncementText() {
        AnnouncementTextInfo textInfoRef = (AnnouncementTextInfo)announcementQueue.Peek();
        announcementText.text = textInfoRef.text;
        announcementText.color = textInfoRef.textColor;
        _myAnimator.SetTrigger("Announcement");
    }
}
