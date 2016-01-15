using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MonthTimer : MonoBehaviour {
	public static bool pause = false;
	private static float timer;
	public Slider timeMeter; //UI timer meter
	private static int monthIndex;
	private static int year = 2034;
	private string[] month = new string[] {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
	public static float incomeDue; 
	public float monthLength; 
	public Dictionary<string, MonthData> history = new Dictionary<string, MonthData>(); 

	public GameObject progReport;
	public GameObject Month;

	// Use this for initialization
	void Awake () {
		StartTimer ();
	}
	
	// Update is called once per frame
	void Update () {
		if (timer == monthLength) {
			Month.GetComponent<Text>().text = month[monthIndex] + " " + year;
		}

		if(timer > 0){
			if (!pause) {
				timeMeter.value -= Time.deltaTime; 
				timer -= Time.deltaTime;
			}
		}
		else {
			MonthData curr = new MonthData(PlayerStatus.carbonSaved, PlayerStatus.incomeAdded, PlayerStatus.moneySpent);
			history.Add(month[monthIndex] + year.ToString(), curr); 
			ProduceReport();
			PlayerStatus.incomeAdded = 0;
			PlayerStatus.carbonSaved = 0;
			PlayerStatus.moneySpent = 0;

			monthIndex = (monthIndex < 11) ? monthIndex + 1 : 0;
			year = (monthIndex == 0) ? year + 1 : year;

			timer = monthLength;
			timeMeter.value = monthLength;
		}
	}

	public void StartTimer() {
		timer = monthLength;
		timeMeter.maxValue = monthLength;
		monthIndex = 0;
	}

	public void ProduceReport() {
		GameObject report = (GameObject)Instantiate (progReport);
		report.transform.SetParent(GameObject.Find("HUDCanvas").transform);
		report.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		Text carbonText = report.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
		Text incomeText = report.transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();

		int i = 0;
		foreach(KeyValuePair<string, MonthData> pair in history) {
			carbonText.text += month[i] + " " + year.ToString() + ": " + pair.Value.carbonSaved + "lbs of C02 Saved\r\n";
			incomeText.text += month[i] + " " + year.ToString() + ": $" + pair.Value.incomeAdded + " added to income\r\n";
			i++;
		}
	
	}
}
