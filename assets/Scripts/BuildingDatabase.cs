using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingDatabase : MonoBehaviour {
    public static List<GameObject> buildings;

	// Use this for initialization
	void Start () {
        buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag("Building")); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
