using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeSceneTo (string desiredScene) {

		SaveInfo.SaveAll ();
		Application.LoadLevel (desiredScene);
	}

}
