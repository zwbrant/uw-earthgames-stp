using UnityEngine;
using System.Collections;

public class RotateSun : MonoBehaviour {
	public float Speed; 
	public static bool pause = false;
    public float startRotation;
    float rotateInterval;
    float dayInterval;
    float dayTimer = 0;
    public MonthTimer monthTimer;

	// Use this for initialization
	void Start () {
        transform.Rotate(0f, 0f, startRotation);
        rotateInterval = (180f / monthTimer._monthLength);
        SetDayInterval();
	}
	
	// Update is called once per frame
	void Update () {
       
		if (!PlayerStatus.paused) {
            if (dayTimer >= dayInterval)
            {
                monthTimer.currDay++;
                monthTimer.SetDateText();
                dayTimer = 0.0f;
            }
            dayTimer += Time.deltaTime;

            transform.Rotate (new Vector3 (0, 0, rotateInterval * Time.deltaTime));
			if (transform.localEulerAngles.z < 270 && transform.eulerAngles.z >= 90) {
				transform.Rotate(new Vector3(0, 0, 180f));
                monthTimer.EndOfMonth();
                SetDayInterval();
            }
		}
	}

    void SetDayInterval()
    {
        dayInterval = monthTimer._monthLength / MonthTimer.monthLengths[MonthTimer.monthIndex];
    }
}
