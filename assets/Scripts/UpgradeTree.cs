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
    private Text carbXText;
    private Text incomeXText;
    private Text durationXText;



    private Vector3 leftSpacing = new Vector3 (327, -65, 0);
	private Vector3 midSpacing = new Vector3 (327, 0, 0);
	private Vector3 rightSpacing = new Vector3 (327, 65, 0);

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OpenUpgradeTree()
    {
        GameMgmt.status.PauseGame(true, true);
        SetOrder();

        currMenu = (GameObject)Instantiate(upgradeTreePanel);
        currMenu.transform.SetParent(GameObject.Find("UI").transform, false);
        currMenu.transform.localPosition = new Vector3(0, 0, 0);

        pointsText = currMenu.transform.GetChild(1).gameObject.GetComponent<Text>();
        pointsText.text = PlayerStatus.unlockPoints.ToString();
        carbXText = currMenu.transform.GetChild(3).gameObject.GetComponent<Text>();
        carbXText.text = PlayerStatus.carbonX.ToString() + "x";
        durationXText = currMenu.transform.GetChild(4).gameObject.GetComponent<Text>();
        durationXText.text = PlayerStatus.durationX.ToString() + "x";
        incomeXText = currMenu.transform.GetChild(5).gameObject.GetComponent<Text>();
        incomeXText.text = PlayerStatus.incomeX.ToString() + "x";

        GameObject newMenuItem = BuildItem(upgradeDB[2]);
        newMenuItem.transform.SetParent(currMenu.transform.GetChild(2).GetChild(0).GetChild(0).transform);
        newMenuItem.transform.localPosition = new Vector3(-185, -20, 0);

        BuildTree(newMenuItem, currMenu, upgradeDB[2]);
    }

    public void BuildTree(GameObject currItem, GameObject newMenu, Upgrade currNode) {
		if (currNode.leftNode != null) {
			GameObject newMenuItem = BuildItem (currNode.leftNode);
			newMenuItem.transform.SetParent (currItem.transform);
			newMenuItem.transform.localPosition = leftSpacing;
			//buildBranch(0, currItem, currNode.leftNode.unlocked);
			BuildTree(newMenuItem, newMenu, currNode.leftNode);
		}
		if (currNode.midNode != null) {
			GameObject newMenuItem = BuildItem (currNode.midNode);
			newMenuItem.transform.SetParent (currItem.transform);
			newMenuItem.transform.localPosition = midSpacing;
			//buildBranch(1, currItem, currNode.midNode.unlocked);
			BuildTree(newMenuItem, newMenu, currNode.midNode);
		}
		if (currNode.rightNode != null) {
			GameObject newMenuItem = BuildItem (currNode.rightNode);
			newMenuItem.transform.SetParent (currItem.transform);
			newMenuItem.transform.localPosition = rightSpacing;
			//buildBranch(2, currItem, currNode.rightNode.unlocked);
			BuildTree(newMenuItem, newMenu, currNode.rightNode);
		}
	}

    /*
	private void buildBranch(int nodeIndex, GameObject currItem, bool unlocked) {
		GameObject currBranch = (GameObject)Instantiate (treeBranch);
		if (!unlocked) {
			Image branchImage = currBranch.gameObject.GetComponent<Image> ();
			branchImage.sprite = Resources.Load<Sprite>("Menu Elements/lockedBranch");
		} 

		currBranch.transform.SetParent(currItem.transform);
		currBranch.transform.SetSiblingIndex(0);
		if (nodeIndex == 0) {
			currBranch.transform.localPosition = new Vector3 (45, -45, 0);
            currBranch.transform.Rotate(0, 0, 90);
        } else if (nodeIndex == 2) {
			currBranch.transform.localPosition = new Vector3 (45, 45, 0);
			currBranch.transform.Rotate(0, 0, 180);
		} else {
			currBranch.transform.localPosition = new Vector3 (40, 0, 0);
			currBranch.transform.Rotate(0, 0, 135);
		}
	} */



    void SetOrder()
    {
        upgradeDB[2].setChildren(upgradeDB[5], null, upgradeDB[6]);

        upgradeDB[5].setChildren(modDB[0], null, null);
        upgradeDB[6].setChildren(null, null, upgradeDB[18]);

        upgradeDB[18].setChildren(null, upgradeDB[14], null);
        modDB[0].setChildren(null, upgradeDB[13], null);
    }

    public void calculateModifiers(Upgrade upgrade)
    {

        if (upgrade.carbonX > PlayerStatus.carbonX)
            PlayerStatus.carbonX = upgrade.carbonX;
        if (upgrade.incomeX > PlayerStatus.incomeX)
            PlayerStatus.incomeX = upgrade.incomeX;
        if (upgrade.durationX < PlayerStatus.durationX)
            PlayerStatus.durationX = upgrade.durationX;
        if (upgrade.priceX < PlayerStatus.priceX)
            PlayerStatus.priceX = upgrade.priceX;

        Debug.Log("Multipliers: income: " + PlayerStatus.incomeX 
            + " carbon: " + PlayerStatus.carbonX + " speed: " + PlayerStatus.durationX);
    }

	public GameObject BuildItem(Upgrade upgrade) {

		GameObject newMenuItem = (GameObject)Instantiate (upgradeItem);

		Image itemBg = newMenuItem.transform.GetChild (0).gameObject.GetComponent<Image> ();
		Image itemIcon = newMenuItem.transform.GetChild (1).gameObject.GetComponent<Image> ();
		Button buttons = newMenuItem.GetComponentInChildren<Button>();

		buttons.onClick.AddListener(() => ConfirmPopup(upgrade, newMenuItem));
		itemIcon.sprite = upgrade.icon;
		Destroy (newMenuItem.GetComponent<Image> ());
		if (upgrade.unlocked) {
			itemBg.sprite = Resources.Load<Sprite>("Menu Elements/unlockedIcon");
            Destroy(newMenuItem.transform.GetChild(2).gameObject);
        }

        return newMenuItem;
	}

	public void ConfirmPopup(Upgrade upgrade, GameObject currItem) {
        GameObject popup = (GameObject)Instantiate(popupPrefab);

        popup.transform.SetParent(currMenu.transform, false);
        popup.transform.localPosition = new Vector3 (0, 0, 0);
        Button buyButton = popup.transform.GetChild(0).gameObject.GetComponent<Button>();
        Button closeButton = popup.transform.GetChild(8).gameObject.GetComponent<Button>();
        Image icon = popup.transform.GetChild(1).gameObject.GetComponent<Image>();
        Text incomeStat = popup.transform.GetChild(2).gameObject.GetComponent<Text>();
        Text carbStat = popup.transform.GetChild(3).gameObject.GetComponent<Text>();
        Text durationStat = popup.transform.GetChild(4).gameObject.GetComponent<Text>();
        Text pikaStat = popup.transform.GetChild(5).gameObject.GetComponent<Text>();
        Text carbonComp = popup.transform.GetChild(6).gameObject.GetComponent<Text>();
        Text incomeComp = popup.transform.GetChild(7).gameObject.GetComponent<Text>();
        Text name = popup.transform.GetChild(9).gameObject.GetComponent<Text>();
        Text priceStat = popup.transform.GetChild(10).gameObject.GetComponent<Text>();

        name.text = upgrade.upgradeName;
        icon.sprite = upgrade.icon;
        carbStat.text = upgrade.carbonSavings + "lb /year";
        incomeStat.text = "$" + upgrade.monthlyIncome + " /month";
        durationStat.text = upgrade.duration + " sec";
        pikaStat.text = 1.ToString();
        priceStat.text = "$" + upgrade.price.ToString();

        carbonComp.text = "";
        incomeComp.text = "";

        buyButton.onClick.AddListener(() => TryToUnlock(upgrade, currItem));
        buyButton.onClick.AddListener(() => GameObject.Destroy(popup));
        closeButton.onClick.AddListener(() => GameObject.Destroy(popup));

		buyButton.onClick.AddListener(() => Destroy(popup));
	}

	public void TryToUnlock(Upgrade upgrade, GameObject currItem) {
		if (GameMgmt.status.UnlockPoints > 0 && upgrade.unlocked == false && (upgrade.root == null || upgrade.root.unlocked == true)) {
			Debug.Log ("Unlocked " + upgrade.upgradeName);
            if (upgrade.upgradeType == Upgrade.UpgradeType.Modifier) { calculateModifiers(upgrade); }

            upgrade.unlocked = true;
			upgrade.levelRequired = 1;
            GameMgmt.status.UnlockPoints--;
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika yes"));
			Destroy (currMenu);
			OpenUpgradeTree ();
		} else if (GameMgmt.status.UnlockPoints == 0) {
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			player.CreateMessage ("No Unlock Points", true);
		} else if (upgrade.root != null && upgrade.root.unlocked == false) {
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			player.CreateMessage ("Requires prior upgrade", true);
		} else {
			GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/pika no"));
			player.CreateMessage ("You Already Unlocked This", true);
		}
	}
}
