using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is attached to a image to make it fill the entire screen.
public class BackgroundResize : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GetComponent<SpriteRenderer>().sprite = GameManager.instance.GameResorcesRef.
            FindElementalBackground(GameManager.instance.DugeonData.CurrentFloor.PlayerRoom.RoomElementalIdentity);
        Resize();
    }

    // Resizes background image to screen size. 
    void Resize() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 xWidth = transform.localScale;
        xWidth.x = worldScreenWidth / width;
        transform.localScale = xWidth;
     
        Vector3 yHeight = transform.localScale;
        yHeight.y = worldScreenHeight / height;
        transform.localScale = yHeight;
        transform.position = Vector2.zero;
    }
}
