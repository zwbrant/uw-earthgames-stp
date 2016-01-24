using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeTree : MonoBehaviour {
	public static List<Upgrade> upgradeDB = UpgradeDatabase.upgrades;
	public static List<Upgrade> modDB = UpgradeDatabase.modifiers;
	public GameObject upgradeTreePanel;
	public GameObject upgradeItem;
	public GameObject treeBranch;
	public GameObject popupPrefab;
	public PlayerStatus player;
	private GameObject currMenu;
	private Text pointsText; 

	private Vector3 leftSpacing = new Vector3 (-90, -90, 0);
	private Vector3 midSpacing = new Vector3 (0, -80, 0);
	private Vector3 rightSpacing = new Vector3 (90, -90, 0);

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void BuildTree(GameObject currItem, GameObject newMenu, Upgrade currNode) {
		if (currNode.leftNode != null) {
			GameObject newMenuItem = BuildItem (currNode.leftNode);
			newMenuItem.transform.SetParent (currItem.transform);
			newMenuItem.transform.localPosition = leftSpacing;
			buildBranch(0, currItem, currNode.leftNode.unlocked);
			BuildTree(newMenuItem, newMenu, currNode.leftNode);
		}
		if (currNode.midNode != null) {
			GameObject newMenuItem = BuildItem (currNode.midNode);
			newMenuItem.transform.SetParent (currItem.transform);
			newMenuItem.transform.localPosition = midSpacing;
			buildBranch(1, currItem, currNode.midNode.unlocked);
			BuildTree(newMenuItem, newMenu, currNode.midNode);
		}
		if (currNode.rightNode != null) {
			GameObject newMenuItem = BuildItem (currNode.rightNode);
			newMenuItem.transform.SetParent (currItem.transform);
			newMenuItem.transform.localPosition = rightSpacing;
			buildBranch(2, currItem, currNode.rightNode.unlocked);
			BuildTree(newMenuItem, newMenu, currNode.rightNode);
		}
	}

	private void buildBranch(int nodeIndex, GameObject currItem, bool unlocked) {
		GameObject currBranch = (GameObject)Instantiate (treeBranch);
		if (!unlocked) {
			Image branchImage = currBranch.gameObject.GetComponent<Image> ();
			branchImage.sprite = Resources.Load<Sprite>("Menu Elements/lockedBranch");
		} 

		currBranch.transform.SetParent(currItem.transform);
		currBranch.transform.SetSiblingIndex(0);
		if (nodeIndex == 0) {
			currBranch.transform.localPosition = new Vector3 (-45, -45, 0);
		} else if (nodeIndex == 2) {
			currBranch.transform.localPosition = new Vector3 (45, -45, 0);
			currBranch.transform.Rotate(0, 0, 90);
		} else {
			currBranch.transform.localPosition = new Vector3 (0, -40, 0);
			currBranch.transform.Rotate(0, 0, 45);
		}
	}

	public void OpenUpgradeTree() {
        SetOrder();

		currMenu = (GameObject)Instantiate (upgradeTreePanel);
		pointsText = currMenu.transform.GetChild (1).gameObject.GetComponent<Text>();
		pointsText.text = PlayerStatus.unlockPoints.ToString();
		currMenu.transform.SetParent (GameObject.Find("HUDCanvas").transform);
		currMenu.transform.localPosition = new Vector3(0,0,0);

		GameObject newMenuItem = BuildItem (upgradeDB [2]);
		newMenuItem.transform.SetParent (currMenu.transform);
		newMenuItem.transform.localPosition = new Vector3 (15, 120, 0);

		BuildTree (newMenuItem, currMenu, upgradeDB[2]);
	}

    void SetOrder()
    {
        upgradeDB[2].setChildren(upgradeDB[5], null, upgradeDB[6]);

        upgradeDB[5].setChildren(modDB[0], null, null);
        upgradeDB[6].setChildren(null, null, upgradeDB[18]);

        upgradeDB[18].setChildren(null, upgradeDB[14], null);
        modDB[0].setChildren(null, upgradeDB[13], null);
    }

	public GameObject BuildItem(Upgrade upgrade) {
		GameObject newMenuItem = (GameObject)Instantiate (upgradeItem);

		Image itemBg = newMenuItem.transform.GetChild (0).gameObject.GetComponent<Image> ();
		Image itemIcon = newMenuItem.transform.GetChild (1).gameObject.GetComponent<Image> ();
		Button buttons = newMenuItem.GetComponentInChildren<Button>();

		buttons.onClick.AddListener(() => ConfirmPopup(upgrade, newMenuItem));
		itemIcon.sprite = upgrade.icon;
		Destroy (newMenuItem.GetComponent<Image> ());
		if (!upgrade.unlocked) {
			itemBg.sprite = Resources.Load<Sprite>("lockedIcon");
		} 

		return newMenuItem;
	}

	public void ConfirmPopup(Upgrade upgrade, GameObject currItem) {
		GameObject popup = (GameObject)Instantiate (popupPrefab);
		popup.transform.SetParent (GameObject.Find("HUDCanvas").transform);
		popup.transform.localPosition = new Vector3 (0, 0, 0);
		Button buyButton = popup.GetComponentInChildren<Button>();
		Image icon = popup.transform.GetChild (1).gameObject.GetComponent<Image> ();
		Text stats = popup.transform.GetChild (3).gameObject.GetComponent<Text>(); 
		Text carbonComp = popup.transform.GetChild (4).gameObject.GetComponent<Text>(); 
		Text incomeComp = popup.transform.GetChild (5).gameObject.GetComponent<Text>(); 
		Text name = popup.transform.GetChild (7).gameObject.GetComponent<Text>(); 
		
		name.text = upgrade.upgradeName;
		icon.sprite = upgrade.icon;
		stats.text = upgrade.carbonSavings + "lb / year\r\n" + "$" + upgrade.monthlyIncome + " / month\r\n" 
			+ upgrade.duration + " sec\r\n" + "$" + upgrade.price;
		carbonComp.text = "";
		incomeComp.text = "";
		
		buyButton.onClick.AddListener(() => TryToUnlock(upgrade, currItem));
		buyButton.onClick.AddListener(() => Destroy(popup));
	}

	public void TryToUnlock(Upgrade upgrade, GameObject currItem) {
		if (PlayerStatus.unlockPoints > 0 && upgrade.unlocked == false && (upgrade.root == null || upgrade.root.unlocked == true)) {
			Debug.Log ("Unlocked " + upgrade.upgradeName);
			upgrade.unlocked = true;
			upgrade.levelRequired = 1;
			PlayerStatus.unlockPoints--;
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika yes"));
			Destroy (currMenu);
			OpenUpgradeTree ();
		} else if (PlayerStatus.unlockPoints == 0) {
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			player.CreateMessage ("You Have No Unlock Points");
		} else if (upgrade.root.unlocked == false) {
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			player.CreateMessage ("You Need To Unlock The Upgrade Before This");
		} else {
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			player.CreateMessage ("You Already Unlocked This");
		}
	}
}
