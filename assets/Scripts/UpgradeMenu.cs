using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour {
	public int MENU_HEIGHT = 280;
	public int MENU_WIDTH = 230;
	public int ITEM_HEIGHT = 100;
	public int ITEM_WIDTH = 230;
	public Upgrade.UpgradeType upgradeType;
	public PlayerStatus player;

	public GameObject upgradeMenuItemPrefab;
	public GameObject menuPrefab; 
	public Slider TimeMeter; 

	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void createMenu() {

		UpgradeDatabase.updateUnlocks();
		List<Upgrade> subDB = UpgradeDatabase.upgrades.FindAll(item => item.upgradeType == upgradeType && item.levelRequired != 0); 
		GameObject newMenu = (GameObject)Instantiate (menuPrefab);
		RectTransform menuRect = newMenu.GetComponent<RectTransform> ();
		newMenu.transform.SetParent (GameObject.Find("HUDCanvas").transform);
		int menuHeight = subDB.Count * (105) + 15;
		menuRect.localPosition = new Vector3 (150, (menuHeight / 2) + 10, 0);
		menuRect.sizeDelta = new Vector2(ITEM_WIDTH + 10, menuHeight); 

		for (int i = 0; i < subDB.Count; i++) {        //Populates menu
			GameObject newMenuItem = (GameObject)Instantiate (upgradeMenuItemPrefab);
			Text itemText = newMenuItem.transform.GetChild (0).gameObject.GetComponent<Text>();    
			if (!subDB[i].unlocked) {
				Color itemColor = Color.red;
				itemColor.a = .5f;
				newMenuItem.GetComponent<Image>().color = itemColor;
				itemText.text = subDB[i].description + "\r\n\r\nLEVEL " + subDB[i].levelRequired + " REQUIRED";
			} else {
				itemText.text = subDB[i].description + "\r\nPrice: $" + subDB[i].price + "\r\nReward: $" + subDB[i].monthlyIncome + 
					"\r\n" + subDB[i].carbonSavings + "lb C02";
			}
			Image itemImage = newMenuItem.transform.GetChild (1).gameObject.GetComponent<Image> ();
			Button itemButton = newMenuItem.transform.GetChild (1).gameObject.GetComponent<Button>();

			itemImage.sprite = subDB[i].icon;


			//int index = subDB[i].iD;
			//itemButton.onClick.AddListener(() => player.tryToBuy(index, this.gameObject, (Slider) Instantiate(TimeMeter)));
			itemButton.onClick.AddListener(() => Destroy (newMenu)); 


			RectTransform menuItemRect = newMenuItem.GetComponent<RectTransform> ();
			newMenuItem.transform.SetParent (newMenu.transform);
			menuItemRect.localPosition = new Vector3 (0, ((menuHeight / 2 - 60) - (105 *  (i))), 0); //Sets item spacing in menu
		}

	}

}
