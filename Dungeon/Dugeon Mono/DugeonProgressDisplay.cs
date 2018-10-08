using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use in the player status to show the player's progress through the dugeon.
public class DugeonProgressDisplay : MonoBehaviour {

    public GameObject mapNodePrefab;

    // Use this for initialization
    void Start () {
        DisplayDugeonProgress();
    }

    //Takes the list of floors that makes up the dugeon and uses them to create icons to repersent those floors.
    void DisplayDugeonProgress() {

        List<Floor> floorList = GameManager.instance.DugeonData.FloorsList;
        GameObject mapNodeGameObjectRef;
        MapNode mapNodeRef;

        for (int i = 0; i < floorList.Count; i++) {
            mapNodeGameObjectRef = Instantiate(mapNodePrefab, Vector2.zero, Quaternion.identity);
            mapNodeGameObjectRef.transform.SetParent(this.transform, false);
            mapNodeRef = mapNodeGameObjectRef.GetComponent<MapNode>();
            mapNodeRef.UpdateDugeonProgressNode(floorList[i]);
        }
    
    }
}
