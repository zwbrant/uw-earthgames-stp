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
		Destroy (this.gameObject);
	}
}
