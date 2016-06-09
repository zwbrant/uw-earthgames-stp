using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DestroyMenu(){
        CameraDragMove.inMenu = false;

        string goName = gameObject.name;
        if (PlayerStatus.wasMenuPaused && 
            (goName.Contains("Progress Report") || goName.Contains("UpgradeMenu") || goName.Contains("TreeWindow")))
        {
            MonthTimer.player.PauseGame(false, false);
        }
		Destroy (this.gameObject);
	}
}
