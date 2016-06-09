using UnityEngine;
using System.Collections;

public class Upgrade {
	public string upgradeName; //Can't just call it "name" becuase that's inherited from 
							   //Object class
	public int iD; 
	public string description; 
	public Sprite icon;
	public float price;
	public float monthlyIncome; 
	public float carbonSavings;
	public float duration; 
	public UpgradeType upgradeType; //Specifies what kind of upgrade it is
	public int levelRequired; 
	public bool unlocked;

	public Upgrade root;
	public Upgrade leftNode;
	public Upgrade midNode;
	public Upgrade rightNode;
	public float priceX; 
	public float incomeX;
	public float carbonX;
	public float durationX;



	//Specifies the kinds of upgrades available
	public enum UpgradeType {
		Appliances,
		Tires,
		Lightbulbs,
		Trees,
		Vehicles,
        Modifier,
		Thermostats,
		Diets,
		Electricity,
		All 
	}

    //For normal, building/vehicle upgrades
	public Upgrade(string newName,int newID, string newDescription, Sprite newIcon, float newPrice, 
	               float newMonthlyIncome, float newCarbonSavings, UpgradeType newUpgradeType, float newDuration, int newLevelRequired){
		upgradeName = newName;
		iD = newID;
		description = newDescription;
		icon = Resources.Load<Sprite>(newName);
		price = newPrice;
		monthlyIncome = newMonthlyIncome;
		carbonSavings = newCarbonSavings;
		upgradeType = newUpgradeType;
		duration = newDuration;
		levelRequired = newLevelRequired;
		unlocked = false;

        if (!UpgradeDatabase.bestCarbUpgrds.ContainsKey(newUpgradeType) || !UpgradeDatabase.bestIncomeUpgrds.ContainsKey(newUpgradeType))
        {
            UpgradeDatabase.bestCarbUpgrds.Add(newUpgradeType, this);
            UpgradeDatabase.bestIncomeUpgrds.Add(newUpgradeType, this);

        }
        else if (UpgradeDatabase.bestCarbUpgrds[newUpgradeType].carbonSavings < newCarbonSavings) //New upgrade has better carbon savings
        {
            UpgradeDatabase.bestCarbUpgrds[newUpgradeType] = this;
        }
        else if (UpgradeDatabase.bestCarbUpgrds[newUpgradeType].monthlyIncome < newMonthlyIncome) //New upgrade has better monthly income
        {
            UpgradeDatabase.bestIncomeUpgrds[newUpgradeType] = this;
        }
    }
	
	public bool isUnlocked() {
        Debug.Log("Checking... " + upgradeName);
		return (levelRequired <= PlayerStatus.level);
	}

    //For multiplier/bonus upgrades
	public Upgrade(string newName, int newID, string newDescription, Sprite newIcon, float newPriceX, float newIncomeX, float newCarbonX, float newDurationX, UpgradeType newUpgradeType) {
		upgradeName = newName;
		iD = newID;
		description = newDescription;
		icon = newIcon;
		priceX = newPriceX; 
		incomeX = newIncomeX;
		carbonX = newCarbonX;
		durationX = newDurationX;
		upgradeType = newUpgradeType;
		unlocked = false;
	}

	public void setChildren(Upgrade newLeftNode, Upgrade newMidNode, Upgrade newRightNode) {
		leftNode = newLeftNode;
		if (leftNode != null) 
			leftNode.root = this; 
		midNode = newMidNode;
		if (midNode != null)
			midNode.root = this;
		rightNode = newRightNode;
		if (rightNode != null)
			rightNode.root = this;
	}
}
