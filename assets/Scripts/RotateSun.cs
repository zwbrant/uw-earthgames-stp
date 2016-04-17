using UnityEngine;
using System.Collections;

public class RotateSun : MonoBehaviour {
	public float Speed; 
	public static bool pause = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!pause) {
			transform.Rotate (new Vector3 (0, 0, (-1 * Speed)) * Time.deltaTime);
			//Debug.Log (transform.eulerAngles.z);
			if (transform.eulerAngles.z < 270 && transform.eulerAngles.z > 90) {
				transform.localEulerAngles = new Vector3 (0, 300, 90);
			}
		}
	}
}
