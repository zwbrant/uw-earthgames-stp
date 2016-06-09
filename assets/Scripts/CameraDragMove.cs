using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraDragMove : MonoBehaviour {
	private Vector3 resetCamera;
	private Vector3 origin;
	private Vector3 diference;
	public static List<Vector3> taskMeterOrigins = new List<Vector3>();

    public float cameraDistanceMax = 60f;
    public float cameraDistanceMin = 12f;
    private float cameraDistance;
    public float scrollSpeed = 6f;

    GameObject hudElement;
	private bool drag = false;
    public static bool inMenu = false;

    public Text testText;

	void Start () {
		resetCamera = Camera.main.transform.position;
        cameraDistance = this.gameObject.GetComponent<Camera>().orthographicSize;
    }


    void Update() {
        if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            cameraDistance = Vector2.Distance(touch0, touch1) * .1f;
            testText.text = cameraDistance.ToString();
        }

        
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);

        this.gameObject.GetComponent<Camera>().orthographicSize = cameraDistance;
    }

	void LateUpdate () {
		//Origin2 = Camera.main.ScreenToWorldPoint (hudElement.transform.position);

		for (int i = 0; i < taskMeterOrigins.Count; i++) {
			PlayerStatus.taskMeters[i].transform.position = Camera.main.WorldToScreenPoint(taskMeterOrigins[i]);
		}
		if (Input.GetMouseButton (0)) {
			diference = (Camera.main.ScreenToWorldPoint (Input.mousePosition)) - Camera.main.transform.position;
			if (drag == false){
				drag = true;
				origin = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			}
		} else {
			drag = false;
		}

		if (drag == true && !inMenu){

			Camera.main.transform.position = origin-diference;
		//	hudElement.transform.position = Origin + Diference2;
		}

		//RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
		if (Input.GetMouseButton (1)) {
			Camera.main.transform.position = resetCamera;
		}
	}


} 