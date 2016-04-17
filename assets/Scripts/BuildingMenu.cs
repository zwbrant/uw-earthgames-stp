using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading;

public class BuildingMenu : MonoBehaviour {
	public PlayerStatus player;
    public int[] currUpgradeIds;
    int[] initialUpgradesIds;

	public Slider TimeMeter; 
	public GameObject typeTextPrefab;
	public GameObject upgradeMenuItemPrefab;
	public GameObject menuPrefab; 
	public GameObject popupPrefab;
    public GameObject solarPanelPrefab;
    internal List<GameObject> solarPanels;
    public string buildName;

    internal GameObject currMenu;
    internal Vector3 mouseDownPos;
    internal float initCarbon;
    internal float initIncome;
    internal float carbon = 0; //Current stats (savings) of this building
	internal float income = 0;

    void Awake()
    {
        BuildingDatabase.buildings.Add(this);
        solarPanels = new List<GameObject>();
    }


    void Start()
    {
        //AnalyzeBuild();
        initCarbon = carbon;
        initIncome = income;
    }

	public void AnalyzeBuild() {
		income = 0;
		carbon = 0;
		for (int i = 0; i < currUpgradeIds.Length; i++) {
			carbon += UpgradeDatabase.upgrades[currUpgradeIds[i]].carbonSavings;
			income += UpgradeDatabase.upgrades[currUpgradeIds[i]].monthlyIncome;
		}

        //Adds or removes solar panels depending on upgrades
        if (solarPanels.Count < 1 && (Contains(currUpgradeIds, 11) || Contains(currUpgradeIds, 10))) 
        {
            AddSolarPanels();
        } else if (solarPanels.Count > 0 && !Contains(currUpgradeIds, 11) && !Contains(currUpgradeIds, 10))
        {
           GameObject.Destroy(solarPanels[0]);
           if (solarPanels.Count > 1)
               GameObject.Destroy(solarPanels[1]);
           solarPanels.Clear();
        }

        UpdateMaterial();
    }

    bool Contains(int[] array, int value)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i].Equals(value))
                return true;
        return false;
    }

    private void UpdateMaterial()
    {
        Material material = gameObject.GetComponent<Renderer>().material;

        if (!material.name.Contains("Vehicle"))
        {
            if (CarbonImprovement() >= 1.0 && material.name.Contains("dirty"))
            {
                gameObject.GetComponent<Renderer>().material =
                    Resources.Load<Material>("Materials/City/" + material.name.Substring(0, material.name.Length - 17));

            }
            else if (CarbonImprovement() <= 20.0 && !material.name.Contains("dirty"))
            {
                gameObject.GetComponent<Renderer>().material =
                    Resources.Load<Material>("Materials/City/" + material.name.Substring(0, material.name.Length - 11) + "_dirty");
            }
        }
    }

    public double CarbonImprovement()
    {
        if (initCarbon == 0)
            return 0;
        return ((carbon - initCarbon) / carbon) * 100;
    }

    public void OnMouseDown()
    {
        mouseDownPos = Camera.main.transform.position;
    }

	public void OnMouseUp() {
		if (mouseDownPos == Camera.main.transform.position && !EventSystem.current.IsPointerOverGameObject())
        {

            Debug.Log(name + ": Carbon = " + carbon.ToString() + " Initial Carbon = " + initCarbon.ToString() + 
                " Income = " + income.ToString() + " Initial Income = " + initIncome.ToString());
			CreateSelectors();
            
        }
	}
	
	public void CreateSelectors(){
		Debug.Log ("Making Menu");
		if (currMenu != null) 
			Destroy(currMenu);
		UpgradeDatabase.updateUnlocks ();
		currMenu = (GameObject)Instantiate (menuPrefab);
		currMenu.transform.SetParent (GameObject.Find ("UI").transform, false);
		//currMenu.transform.localPosition = new Vector3 (0, 0, 0);
        //UpdateBuildStats();

		for (int i = 0; i < currUpgradeIds.Length; i++) {
			GameObject currText = (GameObject)Instantiate(typeTextPrefab);
			currText.GetComponent<Text>().text = "Upgrade" + " " + UpgradeDatabase.upgrades[currUpgradeIds[i]].upgradeType;
			currText.transform.SetParent(currMenu.transform, false);
			currText.transform.localPosition = new Vector3(-162, 180 - (112 * i), 0);
			GameObject installedUpgrade = CreateUpgrade (currUpgradeIds[i]);
			installedUpgrade.transform.SetParent (currMenu.transform, false);
			installedUpgrade.transform.localPosition = new Vector3(-215, 130 - (112 * i),0);
			CreateSelector (currMenu, i);
		}
	}

	public void CreateSelector(GameObject menu, int selectorIndex){
        List<Upgrade> subDB = UpgradeDatabase.upgrades.FindAll(item =>
            item.upgradeType == UpgradeDatabase.upgrades[currUpgradeIds[selectorIndex]].upgradeType &&
                item.levelRequired != 0 && item.iD != currUpgradeIds[selectorIndex]);

		for (int i = 0; i < subDB.Count; i++) {        //Populates menu
			GameObject newMenuItem = CreateUpgrade(subDB[i].iD);
			Image itemImage = newMenuItem.transform.GetChild (0).gameObject.GetComponent<Image> ();
			Button itemButton = newMenuItem.transform.GetChild (1).gameObject.GetComponent<Button>();
			Color itemColor = Color.gray;
			itemColor.a = .2f;
			newMenuItem.GetComponent<Image>().color = itemColor;
			if (!subDB[i].unlocked) {
				itemImage.sprite = Resources.Load<Sprite>("lockedIcon");
			} 

			Upgrade currUpgrade = subDB[i];
			itemButton.onClick.AddListener(() => ConfirmPopup(currUpgrade, selectorIndex));

			newMenuItem.transform.SetParent (menu.transform, false);
			newMenuItem.transform.localPosition = new Vector3 (-125 + (90 * i), 130 - (112 * selectorIndex), 0); //Sets item spacing in menu
		}

	}

	public GameObject CreateUpgrade(int upgradeId){
		GameObject newMenuItem = (GameObject)Instantiate (upgradeMenuItemPrefab);
		Image itemImage = newMenuItem.transform.GetChild (1).gameObject.GetComponent<Image> ();
		itemImage.sprite = UpgradeDatabase.upgrades.Find(item => item.iD == upgradeId).icon;
		return newMenuItem; 
	}

    public void AddSolarPanels()
    {
        String buildName = gameObject.name;
        solarPanels.Add(Instantiate(solarPanelPrefab));
        solarPanels[0].transform.SetParent(this.transform);
        float panelRotation = transform.rotation.eulerAngles.y;
        if (buildName.Contains("house"))
        {
            solarPanels[0].transform.localPosition = new Vector3(2.14f, 3.76234f, -5.16f);
            solarPanels[0].transform.Rotate(0.0f, panelRotation, 0.0f);
        } else
        {
            solarPanels.Add(Instantiate(solarPanelPrefab));
            solarPanels[1].transform.SetParent(this.transform);

            solarPanels[0].transform.Rotate(47.8f, panelRotation, 0.0f);
            solarPanels[1].transform.Rotate(47.8f, panelRotation, 0.0f);
        } 

        if (buildName.Contains("store"))
        {
            solarPanels[0].transform.localPosition = new Vector3(2.21f, 0.8f, -2.08f);
            solarPanels[1].transform.localPosition = new Vector3(2.21f, 0.8f, 1.1f);
        }
        else if (buildName.Contains("apartment"))
        {
            solarPanels[0].transform.localPosition = new Vector3(2.21f, 4.1f, 3.24f);
            solarPanels[1].transform.localPosition = new Vector3(2.21f, 4.1f, -4.14f);
        }
        else if (buildName.Contains("office"))
        {
            solarPanels[0].transform.localPosition = new Vector3(2.21f, 6.71f, 0.95f);
            solarPanels[1].transform.localPosition = new Vector3(2.21f, 6.59f, -2.6f);
        }
    }

    public void ConfirmPopup(Upgrade upgrade, int selectorIndex) {
		GameObject popup = (GameObject)Instantiate (popupPrefab);
		Upgrade currUpgrade = UpgradeDatabase.upgrades [currUpgradeIds [selectorIndex]];
		popup.transform.SetParent (currMenu.transform, false);
		//popup.transform.localPosition = new Vector3 (0, 0, 0);
		Button buyButton = popup.transform.GetChild (0).gameObject.GetComponent<Button>();
        Button closeButton = popup.transform.GetChild(8).gameObject.GetComponent<Button>();
        Image icon = popup.transform.GetChild (1).gameObject.GetComponent<Image> ();
		Text incomeStat = popup.transform.GetChild (2).gameObject.GetComponent<Text>();
        Text carbStat = popup.transform.GetChild(3).gameObject.GetComponent<Text>();
        Text durationStat = popup.transform.GetChild(4).gameObject.GetComponent<Text>();
        Text pikaStat = popup.transform.GetChild(5).gameObject.GetComponent<Text>();
        Text priceStat = popup.transform.GetChild(10).gameObject.GetComponent<Text>();
        Text carbonComp = popup.transform.GetChild (6).gameObject.GetComponent<Text>(); 
		Text incomeComp = popup.transform.GetChild (7).gameObject.GetComponent<Text>(); 
		Text name = popup.transform.GetChild (9).gameObject.GetComponent<Text>(); 

		name.text = upgrade.upgradeName;
		float carbonDiff = upgrade.carbonSavings - currUpgrade.carbonSavings;
		float incomeDiff = upgrade.monthlyIncome - currUpgrade.monthlyIncome;
		icon.sprite = upgrade.icon;
        carbStat.text = upgrade.carbonSavings + "lb";
        incomeStat.text = "$" + upgrade.monthlyIncome;
        durationStat.text = upgrade.duration + " sec";
        priceStat.text = "$" + upgrade.price.ToString();
        pikaStat.text = 1.ToString();

        //NEED PRICE

        carbonComp.text = (carbonDiff >= 0) ? "(+" + (carbonDiff) + ")" : "(" + (carbonDiff) + ")"; 
		incomeComp.text = (incomeDiff >= 0) ? "(+" + (incomeDiff) + ")" : "(" + (incomeDiff) + ")";

        Color red = new Color(0.89f, .37f, .37f);
        Color green = new Color(.61f, .79f, .31f);
        carbonComp.color = (carbonDiff > 0) ? green : red;
		incomeComp.color = (incomeDiff > 0) ? green : red;

        buyButton.onClick.AddListener(() => player.tryToBuy(upgrade.iD, this, (Slider) Instantiate(TimeMeter), selectorIndex));
        buyButton.onClick.AddListener(() => GameObject.Destroy(popup));
        closeButton.onClick.AddListener(() => GameObject.Destroy(popup));
    }
}
