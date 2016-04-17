using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingDatabase : MonoBehaviour {
    public static List<BuildingMenu> buildings = new List<BuildingMenu>();

	// Use this for initialization
	void Start () {
        for (int i = 0; i < buildings.Count; i++)
        {
            Debug.Log(buildings[i].name);
        }

	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
