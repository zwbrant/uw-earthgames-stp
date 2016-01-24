using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour {
	public static List<Upgrade> upgradeDB = UpgradeDatabase.upgrades;
    private const float TimerSpeed = 5.0f;
	public static float money; //Current player money
	public static float carbon; //Current player carbon savings
	public static int pikas;
	public static int unlockPoints;
	public static float incomeAdded;
	public static float carbonSaved;
	public static float moneySpent;
	public static int level;
	public static bool paused = false;

	public static float income; //Monthly income
	//public static float carbonDue; //Current player carbon savings
	public MonthTimer monthTimer;
    public GameObject townSmog;

	public Text moneyText;                      // References to the Text components.
	public Text carbonText;
	public Text pikaText;
	public Text levelText;
	public GameObject messagePanel;

    //Lists, with synchronized indexes, that store upgrade meters and their associated values 
    //(there's probably a way better way to do this)
	public static List<Slider> taskMeters = new List<Slider>(); 
	public static List<int> meterIDs = new List<int> ();
	public static List<int> oldUpgradeIds = new List<int> (); //Needed to calculate net change
	public static List<BuildingMenu> buildings = new List<BuildingMenu>();


	// Use this for initialization
	void Awake () {
        UpgradeDatabase.MakeUpgradeDB();
    }

	void Start () {
        UpdateSmog();
        
        // Preliminary attempt at save/load system
		if (PlayerPrefs.HasKey("MONEY")) { //checks for player info
            //Used for continuing a previous game
            GameMgmt.LoadPreviousGame();
		} else {
            //Used to start new game
            GameMgmt.StartNewGame();
		} 
	}

	// Update is called once per frame
	void Update () {
        townSmog.transform.Translate(Vector3.right * 10 * Time.deltaTime);

        if (monthTimer.timeMeter.value <= 0) 
			money += income;

		if (!paused) {
			for (int i = taskMeters.Count - 1; i >= 0; i--) {	                    //Advances all of the active upgrade task meters.					
				if (taskMeters [i].value <= 0) {
                    BuildingMenu building = buildings[i];
					GetComponent<AudioSource> ().PlayOneShot (Resources.Load<AudioClip> ("Audio/pika hooray"));

					float incomeX = 1; 
					float carbonX = 1;
					List<Upgrade> mods = UpgradeDatabase.RelevantMods (upgradeDB [meterIDs [i]].upgradeType);
					foreach (Upgrade mod in mods) {
						incomeX += mod.incomeX;
						carbonX += mod.carbonX;
					}
					carbon += upgradeDB [meterIDs [i]].carbonSavings * carbonX - upgradeDB [oldUpgradeIds [i]].carbonSavings * carbonX;
					income += upgradeDB [meterIDs [i]].monthlyIncome * incomeX - upgradeDB [oldUpgradeIds [i]].monthlyIncome * incomeX;
					carbonSaved += upgradeDB [meterIDs [i]].carbonSavings * carbonX - upgradeDB [oldUpgradeIds [i]].carbonSavings * carbonX;
					incomeAdded += upgradeDB [meterIDs [i]].monthlyIncome * incomeX - upgradeDB [oldUpgradeIds [i]].monthlyIncome * incomeX;

                    building.AnalyzeBuild();
					if (building.buildName != "" && building.CarbonImprovement() >= 20.0)
						building.gameObject.GetComponent<Renderer>().material = 
							Resources.Load<Material>("Materials/Materials/" + building.buildName);

                    if (building.currMenu != null)
                        building.CreateSelectors();

                    if (carbon >= UpgradeDatabase.levels [level]) {                 //Levels the player
                        LevelPlayer();
					}

					pikas++;
					Destroy (taskMeters [i].gameObject);
					CameraDragMove.taskMeterOrigins.RemoveAt (i);
					taskMeters.RemoveAt (i);
					meterIDs.RemoveAt (i);
					buildings.RemoveAt(i);
                    UpdateSmog();
				} else {
					taskMeters [i].value -= Time.deltaTime * TimerSpeed;
				}

                if (monthTimer.timeMeter.value <= 0) 
                    PauseGame();
            }

            if (monthTimer.timeMeter.value <= 0)
                PauseGame();
        }
        UpdateUIStats();
	}
	
    public void UpdateUIStats ()
    {
        moneyText.text = (income > 0) ? "$" + money + " (+" + income + ")" : "$" + money + " (" + income + ")";
        carbonText.text = carbon + " lb/yr";
        levelText.text = "Level " + level.ToString() + " (" + carbon + "/" + UpgradeDatabase.levels[level].ToString()
            + " till Level " + (level + 1).ToString() + ")";
        pikaText.text = pikas.ToString();
    }

    public void LevelPlayer()
    {
        Debug.Log("leveled up!");
        unlockPoints++;
        pikas++;
        level++;
        CreateMessage("YOU LEVELED UP! \r\n You're Now Level " + level.ToString());
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika hooray"));
    }

	public bool tryToBuy(int upgradeId, BuildingMenu building, Slider meter, int selectorIndex) {
		if (!upgradeDB[upgradeId].unlocked) {
			CreateMessage("You Need To Level-Up For This Upgrade\r\n" + 
			            "(Level " + upgradeDB[upgradeId].levelRequired.ToString() + " Required)");
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			return false;
		} else if (pikas <= 0) {
			CreateMessage ("No Pika Workers Available");
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			return false;
		} else if (money < upgradeDB [upgradeId].price) {
			CreateMessage ("Not Enough Money For Upgrade");
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			return false;
		} else {
			float priceX = 1; 
			float durationX = 1;
			List<Upgrade> mods = UpgradeDatabase.RelevantMods(upgradeDB[upgradeId].upgradeType);
			foreach (Upgrade mod in mods) {
				priceX += mod.priceX;
				durationX += mod.durationX;
			}

			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika yes"));

			money -= upgradeDB[upgradeId].price * priceX;
			building.income += upgradeDB[upgradeId].monthlyIncome;
			building.carbon += upgradeDB[upgradeId].carbonSavings;
			pikas--; 
			moneySpent += upgradeDB[upgradeId].price * priceX;
			buildings.Add (building);
			createTaskMeter((upgradeDB[upgradeId].duration * durationX), upgradeId, building.currUpgradeIds[selectorIndex], building.gameObject, meter);

			building.currUpgradeIds[selectorIndex] = upgradeId;   //Updates Building Menu with new upgrade
			
			return true;
		}
	}

	public void CreateMessage(string message) {
		GameObject moneyError = (GameObject)Instantiate (messagePanel);
		moneyError.transform.SetParent(GameObject.Find("HUDCanvas").transform);
		moneyError.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		moneyError.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = message;
	}

	//Creates a timer meter on the location of a upgrade menu, with the duration dependent on the upgrade 
	public static void createTaskMeter(float duration, int upgradeId, int oldUpgradeId, GameObject building, Slider meter) {
		Slider newMeter = meter;
		newMeter.maxValue = duration;
		newMeter.value = newMeter.maxValue;
		newMeter.transform.SetParent (GameObject.Find ("HUDCanvas").transform);
        newMeter.transform.SetAsFirstSibling();
        Vector3 buildPos = GameObject.Find ("Main Camera").GetComponent<Camera>().WorldToScreenPoint(building.transform.position);
		newMeter.transform.position = buildPos + new Vector3(0, 20 * taskMeters.Count, 0);
		CameraDragMove.taskMeterOrigins.Add(Camera.main.ScreenToWorldPoint (newMeter.transform.position));
		RectTransform sliderRect = newMeter.GetComponent<RectTransform> ();
		//sliderRect.localPosition = new Vector3 (0, 25, 0);
		sliderRect.sizeDelta = new Vector2 (120, 35);

		taskMeters.Add (newMeter);
		meterIDs.Add (upgradeId);
		oldUpgradeIds.Add (oldUpgradeId);

	}

	public void PauseGame() {
		paused = !paused;
		Camera.main.GetComponent<AudioSource>().volume = (paused) ? 0.05f : 0.3f;
		RotateSun.pause = !RotateSun.pause;
		MonthTimer.pause = !MonthTimer.pause;
	}

    private void UpdateSmog()
    {
        float smogOpacity = 1.2f - (carbon / UpgradeDatabase.levels[2]);
        Debug.Log("Smog opacity: " + smogOpacity);

        Material smogMaterial = townSmog.transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
        Color smogColor = smogMaterial.color;
        smogColor.a = smogOpacity;
        smogMaterial.color = smogColor;
    }
}
