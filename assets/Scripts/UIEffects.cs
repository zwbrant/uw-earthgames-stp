using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEffects : MonoBehaviour {
    public GameObject coinSpriteTemp;
    public static GameObject coinSprite;
    public static GameObject UI;
    public static Dictionary<string, int> spawnHeights = new Dictionary<string, int>
    {
        {"apartment", 15}, {"house", 10}, {"office", 17}, {"store", 9}, {"car", 3}, {"van", 4},
        { "truck", 4}, {"ute", 4}, {"cop", 4}
    };

	// Use this for initialization
	void Start () {
        coinSprite = coinSpriteTemp;
        UI = GameObject.Find("UI");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void MoneyReward(GameObject building, float income)
    {
        int coinCount = ((int)income / 2);

        Vector3 position = building.transform.position;

        if (spawnHeights.ContainsKey(building.name.Split('_')[0]))
            position.y += spawnHeights[building.name.Split('_')[0]];
        else
            position.y += 10f;

        for (int i = 0; i < coinCount; i++)
        {
            GameObject newCoin = (GameObject)Instantiate(coinSprite);
            newCoin.transform.position = position;
        }

    }
}
