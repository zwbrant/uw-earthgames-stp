using UnityEngine;
using System.Collections;

public class MonthData {
	public float carbonSaved;
	public float incomeAdded;
	public float moneySpent;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public MonthData(float carbon, float income, float spent) {
		carbonSaved = carbon;
		incomeAdded = income;
		moneySpent = spent;
	}

}
