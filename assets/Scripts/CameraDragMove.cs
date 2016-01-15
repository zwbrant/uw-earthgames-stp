using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDragMove : MonoBehaviour {
	private Vector3 ResetCamera;
	private Vector3 Origin;
	private Vector3 Diference;
	public static List<Vector3> taskMeterOrigins = new List<Vector3>(); 

	GameObject hudElement;
	private bool Drag = false;

	void Start () {
		ResetCamera = Camera.main.transform.position;
	}

	void LateUpdate () {
		//Origin2 = Camera.main.ScreenToWorldPoint (hudElement.transform.position);

		for (int i = 0; i < taskMeterOrigins.Count; i++) {
			PlayerStatus.taskMeters[i].transform.position = Camera.main.WorldToScreenPoint(taskMeterOrigins[i]);
		}
		if (Input.GetMouseButton (0)) {
			Diference = (Camera.main.ScreenToWorldPoint (Input.mousePosition)) - Camera.main.transform.position;
			if (Drag == false){
				Drag = true;
				Origin = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			}
		} else {
			Drag = false;
		}
		if (Drag == true){

			Camera.main.transform.position = Origin-Diference;
		//	hudElement.transform.position = Origin + Diference2;
		}

		//RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
		if (Input.GetMouseButton (1)) {
			Camera.main.transform.position = ResetCamera;
		}
	}
} 