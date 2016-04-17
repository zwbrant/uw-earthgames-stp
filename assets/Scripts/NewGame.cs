using UnityEngine;
using System.Collections;

//This script should be used for any functions nessecary to starting a new game
//like clearing old game info 
public class NewGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ClearPlayerPrefs() {
		PlayerPrefs.DeleteAll ();
		Debug.Log ("Prefs Cleared");
	}
}
