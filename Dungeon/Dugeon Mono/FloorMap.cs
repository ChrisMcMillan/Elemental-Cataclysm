using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*This is a mini map that display information about the floor the player is on.
 Can diplay information like player current location, enemy location and boss location.*/ 
public class FloorMap : MonoBehaviour {

    public GameObject mapNodePrefab;

	// Use this for initialization
	void Start () {
        CreateMap();
    }

    //Take the floor data and gernerte the appropriate amount of map nodes for the mini map
    public void CreateMap(){

        Floor floorData = GameManager.instance.DugeonData.CurrentFloor;

        GridLayoutGroup gridLayoutScript = GetComponent<GridLayoutGroup>();
        gridLayoutScript.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutScript.constraintCount = floorData.RoomMatrix.GetLength(1);

        GameObject mapNodeGameObjectRef;

        for (int i = 0; i < floorData.RoomMatrix.GetLength(0); i++){
            for (int j = 0; j < floorData.RoomMatrix.GetLength(1); j++){
                mapNodeGameObjectRef = Instantiate(mapNodePrefab, Vector2.zero, Quaternion.identity);
                mapNodeGameObjectRef.transform.SetParent(this.transform, false);

                floorData.RoomMatrix[i, j].MyNode = mapNodeGameObjectRef.GetComponent<MapNode>();
                floorData.RoomMatrix[i, j].UpdateApperance();
            }
        }
    }



}
