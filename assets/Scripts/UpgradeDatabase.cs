using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public static class UpgradeDatabase {
	public static List<Upgrade> upgrades = new List<Upgrade>();
	public static List<Upgrade> modifiers = new List<Upgrade>();
    //Delineates the available levels and how much XP is necessary for each one
	public static int[] levels = new int[] {0, 400, 1500, 3000, 6100, 12000, 20000, 48000, 66000, 80000, 100000}; 

	// Initialization of all upgrades in the game

	public static void MakeUpgradeDB () {
		// Upgrade Format: Name, ID, Description, Icon, Price, Income Added, Carbon Savings, Upgrade Type, Time Required, Level Required
		upgrades.Add (new Upgrade ("Alder", 0, "Trees no longer needed", Resources.Load<Sprite>("Alder"), 0.0f, 0.0f, 0.0f, Upgrade.UpgradeType.Trees, 85.0f, 0));
		upgrades.Add (new Upgrade ("Maple", 1, "Trees no longer needed", Resources.Load<Sprite>("Maple"), 0.0f, 0.0f, 0.0f, Upgrade.UpgradeType.Trees, 90.0f, 0));
		upgrades.Add (new Upgrade ("Optimize PSI (Handpump)", 2, "Use your left hand if you get bored", Resources.Load<Sprite>("vehicle manual pump"), 0.0f, 1.25f, 100.0f, Upgrade.UpgradeType.Tires, 30.0f, 0));
		upgrades.Add (new Upgrade ("Incandescent", 3, "Don't use these", Resources.Load<Sprite>("Incandescent"), 3.0f, 0.0f, 0.0f, Upgrade.UpgradeType.Lightbulbs, 40.0f, 2));
		upgrades.Add (new Upgrade ("CFL", 4, "It's white, it's bright, it's a kind of light", Resources.Load<Sprite>("CFL"), 2.0f, 1.25f, 5.5f, Upgrade.UpgradeType.Lightbulbs, 20.0f, 2));
		upgrades.Add (new Upgrade ("LED", 5, "High-tech circa 2002", Resources.Load<Sprite>("LED"), 4.0f, 2.0f, 6.0f, Upgrade.UpgradeType.Lightbulbs, 10.0f, 0));
		upgrades.Add (new Upgrade ("Hybrid Car", 6, "Buy a hybrid instead of a non hybrid!", Resources.Load<Sprite>("Hybrid"), 1500.0f, 42.0f, 3300.0f, Upgrade.UpgradeType.Vehicles, 90.0f, 0));
		upgrades.Add (new Upgrade ("Old Car", 7, "Carbureted inefficiency", Resources.Load<Sprite>("OldCar"), 20.0f, -3.0f, -5.0f, Upgrade.UpgradeType.Vehicles, 50.0f, 1));
        upgrades.Add(new Upgrade("Poorly Inflated Tire", 8, "Too high, too low - neither goes", Resources.Load<Sprite>("vehicle flat tire"), 0.0f, 0.0f, 0.0f, Upgrade.UpgradeType.Tires, 5.0f, 1));
        upgrades.Add(new Upgrade("Road Bike", 9, "Bikes are the best thing ever -Every Biker Ever", Resources.Load<Sprite>("RoadBike"), 200.0f, 12.5f, 1000.0f, Upgrade.UpgradeType.Vehicles, 50.0f, 3));
		upgrades.Add(new Upgrade("Full House Solar", 10, "Save the climate while feeling awesomely high tech", Resources.Load<Sprite>("FullHouseSolar"), 23000.0f, 113.0f, 23500.0f, Upgrade.UpgradeType.Electricity, 100.0f, 9));
		upgrades.Add(new Upgrade("1kw Solar", 11, "Save the climate while feeling awesomely high tech", Resources.Load<Sprite>("1kwSolar"), 3500.0f, 17.25f, 3600.0f, Upgrade.UpgradeType.Electricity, 50.0f, 6));
        upgrades.Add(new Upgrade("Electric Thermostat Adjustment", 12, "High tech free home improvement", Resources.Load<Sprite>("70degree"), 0.0f, 7.0f, 1500.0f, Upgrade.UpgradeType.Thermostats, 75.0f, 1));
        upgrades.Add(new Upgrade("Gas Thermostat Adjustment", 13, "Free home improvement", Resources.Load<Sprite>("70degree"), 0.0f, 2.0f, 350.0f, Upgrade.UpgradeType.Thermostats, 50.0f, 0));
        upgrades.Add(new Upgrade("Limit Meat Consumption", 14, "Not quite a vegetarian", Resources.Load<Sprite>("LimitMeat"), 0.0f, 10.0f, 660.0f, Upgrade.UpgradeType.Diets, 65.0f, 0));
        upgrades.Add(new Upgrade("Become Vegetarian", 15, "Is quite a vegetarian", Resources.Load<Sprite>("Vegetarian"), 0.0f, 22.5f, 1760.0f, Upgrade.UpgradeType.Diets, 90.0f, 2));
        upgrades.Add(new Upgrade("Eat Lots Of Meat", 16, "Beef, bacon and the American way", Resources.Load<Sprite>("Meatwad"), 0.0f, 0.0f, 0.0f, Upgrade.UpgradeType.Diets, 70.0f, 1));
        upgrades.Add(new Upgrade("Unadjusted Thermostat", 17, "Salmon", Resources.Load<Sprite>("72degree"), 0.0f, 0.0f, 0.0f, Upgrade.UpgradeType.Thermostats, 50.0f, 1));
		upgrades.Add (new Upgrade ("Optimize PSI (Electric Pump)", 18, "Use your left hand if you get bored", Resources.Load<Sprite>("ElectricPump"), 0.0f, 1.25f, 100.0f, Upgrade.UpgradeType.Tires, 10.0f, 0));
		upgrades.Add(new Upgrade("Coal", 19, "Save climate change while feeling awesomely high tech", Resources.Load<Sprite>("Coal"), 100.0f, 0.0f, 0.0f, Upgrade.UpgradeType.Electricity, 20.0f, 1));

		// Upgrade Format: Name, ID, Description, Icon, PriceX, IncomeX, CarbonX, DurationX, Upgrade Type
		modifiers.Add (new Upgrade("2x Income, .5x Duration Bonus", 0, "2x Money, .5x Duration Multiplier", Resources.Load<Sprite>("2x Cash"), 0, 2, 0, 0.5f, Upgrade.UpgradeType.Modifier));

		updateUnlocks ();
		//UpgradeTree.BuildTree ();
	}

	public static List<Upgrade> RelevantMods(Upgrade.UpgradeType kind) {
		return modifiers.FindAll(item => item.upgradeType == kind || item.upgradeType == Upgrade.UpgradeType.All && item.unlocked);
	}

	public static void updateUnlocks() {
		List<Upgrade> subDB = upgrades.FindAll(item => item.levelRequired <= PlayerStatus.level && item.levelRequired != 0);
		foreach (Upgrade upgrade in subDB) 
			upgrade.unlocked = true;
	}


}
