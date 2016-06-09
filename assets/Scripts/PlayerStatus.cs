using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour {
    public static GameObject uI;
	public static List<Upgrade> upgradeDB = UpgradeDatabase.upgrades;
    public static StatsAndTriggers statsAndTriggers;
    private const float TimerSpeed = 5.0f;
	public static float money; //Current player money
	public static float carbon; //Current player carbon savings
	public static int pikas;
    public static float incomeX; //Multipliers
    public static float priceX;
    public static float carbonX; 
    public static float durationX;

    public static GameObject pikaContainer;
    public GameObject researchArea;
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
    public Text incomeText;
    public Text carbonText;
	public Text pikaText;
	public Text levelText;
    public static GameObject manager;
    public static Slider xPSlider;
    public Button pauseButton;
    public GameObject taskTimer;
	public GameObject errorPopup;
    public GameObject levelPopup;
    public GameObject pika;


    //Lists, with synchronized indexes, that store upgrade meters and their associated values 
    //(there's probably a way better way to do this)
	public static List<Slider> taskMeters = new List<Slider>(); 
	public static List<int> meterUpgradeIDs = new List<int> ();
	public static List<int> oldUpgradeIds = new List<int> (); //Needed to calculate net change
	public static List<BuildingMenu> buildings = new List<BuildingMenu>();

    public int Pikas
    {
        set
        {
            int startCount = Pikas;
            if (value > Pikas)
            {
                for (int i = 0; i < (value - startCount); i++)
                {
                    //Debug.Log("Adding a pika...");
                    GameObject newPika = Instantiate(pika);
                    newPika.transform.SetParent(pikaContainer.transform, false);
                    newPika.transform.localScale = new Vector3(2.8f, 2.8f, 2.8f);
                    newPika.transform.localPosition = new Vector3((startCount * 155 + (155 * i)) + -65, -31.5f, 0);
                }
                
            } else
            {
                for (int i = 0; i < (startCount - value); i++)
                {
                    //Debug.Log("Removing a pika...");
                    Destroy(pikaContainer.transform.GetChild((startCount - 1) - i).gameObject);
                }
            }
           
        }
        get { return pikaContainer.transform.childCount; }
    }

    public float Carbon
    {

        get { return carbon; }
        set
        {
            carbon = value;
            while (carbon >= UpgradeDatabase.levels[Level])
                LevelPlayer();
            if (xPSlider.maxValue != UpgradeDatabase.levels[level])
                xPSlider.maxValue = UpgradeDatabase.levels[level];
            SmogBehavior.SetCutoff();
            
            xPSlider.value = value;
        }
    }

    public int UnlockPoints
    {
        get { return unlockPoints; }
        set
        {
            unlockPoints = value;

            researchArea.transform.GetChild(4).gameObject.GetComponent<Image>().sprite = (unlockPoints == 0) 
                ? Resources.Load<Sprite>("New UI/Research Icon") : Resources.Load<Sprite>("New UI/Research Plus Icon");
        }
    }

    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            researchArea.transform.GetChild(3).gameObject.GetComponent<Text>().text = "lvl " + Level;
            Carbon = Carbon;
        }
    }

	void Awake () {
        uI = GameObject.Find("UI");
        UpgradeDatabase.MakeUpgradeDB();
        pikaContainer = GameObject.Find("PikaContainer");
        manager = GameObject.Find("Manager");
        xPSlider = researchArea.transform.GetChild(1).gameObject.GetComponent<Slider>();
        xPSlider.value = 0;
        statsAndTriggers = GameObject.Find("MessageSequenceManager").GetComponent<StatsAndTriggers>();
        SetPauseButSprites();
    }

	void Start () {
        //UpdateSmog();
        
        // Preliminary attempt at save/load system
		if (PlayerPrefs.HasKey("MONEY")) { //checks for player info
            //Used for continuing a previous game
            GameMgmt.LoadPreviousGame();
		} else {
            //Used to start new game
            GameMgmt.StartNewGame();
		}

        statsAndTriggers.CheckDialogueEvents();
	}

	void Update () {


		if (!paused) {
           // townSmog.transform.Translate(Vector3.right * 10 * Time.deltaTime);

            for (int i = taskMeters.Count - 1; i >= 0; i--) {	                    //Advances all of the active upgrade task meters.					
				if (taskMeters [i].value <= 0) {
                    BuildingMenu building = buildings[i];
					GetComponent<AudioSource>().PlayOneShot (Resources.Load<AudioClip> ("Audio/pika hooray"));
                    Pikas++;

                    float newIncome = upgradeDB[meterUpgradeIDs[i]].monthlyIncome * incomeX - upgradeDB[oldUpgradeIds[i]].monthlyIncome * incomeX;
                    float newCarbon = upgradeDB[meterUpgradeIDs[i]].carbonSavings * carbonX - upgradeDB[oldUpgradeIds[i]].carbonSavings * carbonX;

                    Carbon += newCarbon;
                    income += newIncome;
                    carbonSaved += newCarbon;
                    incomeAdded += newIncome;

                    building.AnalyzeBuild();

                    if (building.currMenu != null)
                        building.CreateSelectors();

                    UIEffects.MoneyReward(building.gameObject, newIncome);

                    DestroyObject(taskMeters[i].gameObject);
					Destroy (taskMeters [i].gameObject);
					CameraDragMove.taskMeterOrigins.RemoveAt (i);
					taskMeters.RemoveAt (i);
					meterUpgradeIDs.RemoveAt (i);
                    oldUpgradeIds.RemoveAt(i);
					buildings.RemoveAt(i);
                    statsAndTriggers.CheckDialogueEvents();
				} else {
					taskMeters [i].value -= Time.deltaTime * TimerSpeed;
				}

                //if (monthTimer.timeMeter.value <= 0) 
                    //PauseGame();
            }

            //if (monthTimer.timeMeter.value <= 0)
            //    PauseGame(true, true);
        }
        UpdateUIStats();
	}
	
    public void UpdateUIStats ()
    {
        moneyText.text = "$" + money;
        carbonText.text = Carbon + " lb/yr";
        incomeText.text = "$" + income;
        levelText.text = "Level " + level.ToString() + " (" + Carbon + "/" + UpgradeDatabase.levels[level].ToString()
            + " till Level " + (level + 1).ToString() + ")";
        pikaText.text = pikas.ToString();
    }

    public void LevelPlayer()
    {
        Debug.Log("leveled up!");
        UnlockPoints++;
        Pikas++;
        Level++;
        CreateMessage("YOU LEVELED UP! \r\n You're Now Level " + level.ToString(), false);
        manager.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika hooray"));
    }

	public bool tryToBuy(int upgradeId, BuildingMenu building, Slider meter, int selectorIndex) {
		if (!upgradeDB[upgradeId].unlocked) {
			CreateMessage("Level " + upgradeDB[upgradeId].levelRequired + " required for this upgrade", true);
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			return false;
		} else if (Pikas <= 0) {
			CreateMessage ("No Pika Workers Available", true);
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			return false;
		} else if (money < upgradeDB [upgradeId].price) {
			CreateMessage ("Not Enough Money For Upgrade", true);
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			return false;
		} else {
            oldUpgradeIds.Add(building.currUpgradeIds[selectorIndex]);
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika yes"));
            //meter.transform.SetParent(uI.transform, false);
			money -= upgradeDB[upgradeId].price * priceX;
			building.income += upgradeDB[upgradeId].monthlyIncome;
			building.carbon += upgradeDB[upgradeId].carbonSavings;
			Pikas--; 
			moneySpent += upgradeDB[upgradeId].price * priceX;
			buildings.Add (building);

			createTaskMeter((upgradeDB[upgradeId].duration * durationX), upgradeId, building.currUpgradeIds[selectorIndex], building.gameObject);

			building.currUpgradeIds[selectorIndex] = upgradeId;   //Updates Building Menu with new upgrade
            UpdateUIStats();
            building.CreateSelectors();
			return true;
		}
	}

	public void CreateMessage(string message, bool isError) {
        if (isError)
        {
            GameObject msg = (GameObject)Instantiate(errorPopup);
            msg.transform.SetParent(GameObject.Find("UI").transform, false);
            //msg.GetComponent<RectTransform>().localPosition = new Vector3(252.0f, -98.0f, 0.0f);
            msg.transform.GetChild(1).GetComponent<Text>().text = message;
        } else
        {
            GameObject msg = (GameObject)Instantiate(levelPopup);
            msg.transform.SetParent(GameObject.Find("UI").transform, false);
            //msg.GetComponent<RectTransform>().localPosition = new Vector3(500.0f, -133.0f, 0.0f);
            string msgText = "";
            string unlocks = "Technologies Unlocked:";
            foreach (Upgrade upgrd in upgradeDB)
            {
                if (upgrd.levelRequired == level)
                {
                    msgText = unlocks;
                    
                    msgText += "\r\n" + upgrd.upgradeName;
                    Debug.Log(msgText);
                }
            }

            if (!msgText.Contains("Technologies"))
                msgText = "Check the Research Tree for new upgrades!";

            msg.transform.GetChild(1).GetComponent<Text>().text = msgText;

        }
	}

	//Creates a timer meter on the location of a upgrade menu, with the duration dependent on the upgrade 
	public void createTaskMeter(float duration, int upgradeId, int oldUpgradeId, GameObject building) {
        Slider newMeter = GameObject.Instantiate(taskTimer).GetComponent<Slider>();
        newMeter.maxValue = duration;
		newMeter.value = newMeter.maxValue;
		newMeter.transform.SetParent (GameObject.Find ("UI").transform);
        newMeter.transform.SetAsFirstSibling();
        Vector3 buildPos = GameObject.Find ("Main Camera").GetComponent<Camera>().WorldToScreenPoint(building.transform.position);
        buildPos.z = 100f;
		newMeter.transform.position = buildPos + new Vector3(0, 20 * taskMeters.Count, 0);
		CameraDragMove.taskMeterOrigins.Add(Camera.main.ScreenToWorldPoint (newMeter.transform.position));
		RectTransform sliderRect = newMeter.GetComponent<RectTransform> ();
		//sliderRect.localPosition = new Vector3 (0, 25, 0);
		sliderRect.sizeDelta = new Vector2 (120, 35);

		taskMeters.Add (newMeter);
		meterUpgradeIDs.Add (upgradeId);
		

	}

    Sprite playLight, pauseLight, playDark, pauseDark, disabledPauseBut;
    internal void SetPauseButSprites()
    {
        playLight = Resources.Load<Sprite>("New UI/Play Light");
        pauseLight = Resources.Load<Sprite>("New UI/Pause Light");
        playDark = Resources.Load<Sprite>("New UI/Play Dark");
        pauseDark = Resources.Load<Sprite>("New UI/Pause Dark");
        disabledPauseBut = Resources.Load<Sprite>("New UI/Pause Disabled");
    }
    public static bool wasMenuPaused = false;
	public void PauseGame(bool pause, bool menuPause) {
		paused = pause;
        wasMenuPaused = menuPause;
        pauseButton.GetComponent<Image>().sprite = (paused) ? playLight : pauseLight;

        SpriteState st = new SpriteState();
        st.pressedSprite = (paused) ? playDark : pauseDark;
        st.disabledSprite = disabledPauseBut;
        pauseButton.GetComponent<Button>().spriteState = st;
        Camera.main.GetComponent<AudioSource>().volume = (paused) ? 0.2f : 0.8f;
		RotateSun.pause = !RotateSun.pause;
		MonthTimer.pause = !MonthTimer.pause;
	}

    public void TogglePause()
    {
        PauseGame(!paused, false);
    }

    /*
    private void UpdateSmog()
    {
        float smogOpacity = 1.2f - (carbon / UpgradeDatabase.levels[2]);
        Debug.Log("Smog opacity: " + smogOpacity);

        Material smogMaterial = townSmog.transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
        Color smogColor = smogMaterial.color;
        smogColor.a = smogOpacity;
        smogMaterial.color = smogColor;
    }
    */
}
