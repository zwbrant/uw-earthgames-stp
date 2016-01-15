using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading;

public class BuildingMenu : MonoBehaviour {
	public PlayerStatus player;
    public VisualUpgrades visualUpgrades;
	public int[] currUpgradeIds;
    public int[] initialUpgradesIds;

	public GameObject currMenu;
	public Slider TimeMeter; 

	public GameObject typeTextPrefab;
	public GameObject upgradeMenuItemPrefab;
	public GameObject menuPrefab; 
	public GameObject popupPrefab;
    public GameObject solarPanelPrefab;
    public List<GameObject> solarPanels = null;
	public string buildName;


    public float initCarbon = 0;
    public float initIncome = 0;
    public float carbon = 0;
	public float income = 0;

    void Start()
    {
        
        AnalyzeBuild();
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

        if (Array.IndexOf(currUpgradeIds, 10) != -1 || Array.IndexOf(currUpgradeIds, 11) != -1) {
            AddSolarPanels();
        } else if (solarPanels.Count > 0)
        {
           GameObject.Destroy(solarPanels[0]);
           if (solarPanels.Count > 1)
               GameObject.Destroy(solarPanels[1]);
           solarPanels.Clear();
        }
    }

    public double CarbonImprovement()
    {
        return ((carbon - initCarbon) / carbon) * 100;
    }

	public void OnMouseDown() {
		if (!EventSystem.current.IsPointerOverGameObject ()) {

            Debug.Log("Carbon = " + carbon.ToString() + " Initial Carbon = " + initCarbon.ToString() + 
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
		currMenu.transform.SetParent (GameObject.Find ("HUDCanvas").transform);
		currMenu.transform.localPosition = new Vector3 (0, 0, 0);
        UpdateBuildStats();

		for (int i = 0; i < currUpgradeIds.Length; i++) {
			GameObject currText = (GameObject)Instantiate(typeTextPrefab);
			currText.GetComponent<Text>().text = "Upgrade" + " " + UpgradeDatabase.upgrades[currUpgradeIds[i]].upgradeType;
			currText.transform.SetParent(currMenu.transform);
			currText.transform.localPosition = new Vector3(-240, 190 - (100 * i), 0);
			GameObject installedUpgrade = CreateUpgrade (currUpgradeIds[i]);
			installedUpgrade.transform.SetParent (currMenu.transform);
			installedUpgrade.transform.localPosition = new Vector3(-280, 140 - (100 * i),0);
			CreateSelector (currMenu, i);
		}
	}

    public void UpdateBuildStats()
    {
        if (currMenu != null)
            currMenu.transform.GetChild(2).transform.GetComponentInChildren<Text>().text = 
                "Carbon Saved: " + (carbon - initCarbon) + "lbs\r\nMoney Saved: $" + (income - initIncome);
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

			newMenuItem.transform.SetParent (menu.transform);
			newMenuItem.transform.localPosition = new Vector3 (-205 + (75 * i), 140 - (100 * selectorIndex), 0); //Sets item spacing in menu
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
		popup.transform.SetParent (currMenu.transform);
		popup.transform.localPosition = new Vector3 (0, 0, 0);
		Button buyButton = popup.transform.GetChild (0).gameObject.GetComponent<Button>();
		Image icon = popup.transform.GetChild (1).gameObject.GetComponent<Image> ();
		Text stats = popup.transform.GetChild (3).gameObject.GetComponent<Text>(); 
		Text carbonComp = popup.transform.GetChild (4).gameObject.GetComponent<Text>(); 
		Text incomeComp = popup.transform.GetChild (5).gameObject.GetComponent<Text>(); 
		Text name = popup.transform.GetChild (7).gameObject.GetComponent<Text>(); 

		name.text = upgrade.upgradeName;
		float carbonDiff = upgrade.carbonSavings - currUpgrade.carbonSavings;
		float incomeDiff = upgrade.monthlyIncome - currUpgrade.monthlyIncome;
		icon.sprite = upgrade.icon;
		stats.text = upgrade.carbonSavings + "lb / year\r\n" + "$" + upgrade.monthlyIncome + " / month\r\n" 
			+ upgrade.duration + " sec\r\n" + "$" + upgrade.price;
		carbonComp.text = (carbonDiff >= 0) ? "(+" + (carbonDiff) + ")" : "(" + (carbonDiff) + ")"; 
		incomeComp.text = (incomeDiff >= 0) ? "(+" + (incomeDiff) + ")" : "(" + (incomeDiff) + ")"; 
		carbonComp.color = (carbonDiff > 0) ? Color.green : Color.red;
		incomeComp.color = (incomeDiff > 0) ? Color.green : Color.red;

		buyButton.onClick.AddListener(() => player.tryToBuy(upgrade.iD, this, (Slider) Instantiate(TimeMeter), selectorIndex));
        buyButton.onClick.AddListener(() => GameObject.Destroy(popup));
    }
}
