using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMgmt : MonoBehaviour {
    public static PlayerStatus status;

    //New game starting stats
    public const float initCarbon = 0;
    public const float initMoney = 50000;
    public const float initIncome = 0;
    public const int initPikas = 3;
    public const int initLevel = 6;
    public const int initUnlockPoints = 4;


    void Awake ()
    {
        status = this.GetComponent<PlayerStatus>();
    }

    public void QuitGame()
    {
        Debug.Log("Saving & quitting game...");

        SaveGame();
        Application.Quit();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("LEVEL", status.Level);
        PlayerPrefs.SetFloat("CARBON", status.Carbon);
        PlayerPrefs.SetFloat("MONEY", PlayerStatus.money);
        PlayerPrefs.SetInt("PIKAS", status.Pikas);
        PlayerPrefs.SetFloat("INCOME", PlayerStatus.income);
        PlayerPrefs.SetInt("UNLOCKPOINTS", status.UnlockPoints);
        PlayerPrefs.SetFloat("CARBONX", PlayerStatus.carbonX);
        PlayerPrefs.SetFloat("DURATIONX", PlayerStatus.durationX);
        PlayerPrefs.SetFloat("PRICEX", PlayerStatus.priceX);
        PlayerPrefs.SetFloat("INCOMEX", PlayerStatus.incomeX);

        SaveUpgradeTree();
        SaveBuildings();

        Debug.Log("Game saved");
    }

    public void ClearSaveInfo()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Save info cleared");
    }

    public static void LoadPreviousGame()
    {
        status.Level = PlayerPrefs.GetInt("LEVEL");
        PlayerStatus.money = PlayerPrefs.GetFloat("MONEY");
        PlayerStatus.income = PlayerPrefs.GetFloat("INCOME");
        status.Carbon = PlayerPrefs.GetFloat("CARBON");
        status.Pikas = PlayerPrefs.GetInt("PIKAS");
        status.UnlockPoints = PlayerPrefs.GetInt("UNLOCKPOINTS");
        PlayerStatus.carbonX = PlayerPrefs.GetFloat("CARBONX");
        PlayerStatus.durationX = PlayerPrefs.GetFloat("DURATIONX");
        PlayerStatus.priceX = PlayerPrefs.GetFloat("PRICEX");
        PlayerStatus.incomeX = PlayerPrefs.GetFloat("INCOMEX");

        LoadUpgradeTree();
        LoadBuildings();

        Debug.Log("Loaded previous game");
    }

    public static void StartNewGame()
    {
        status.Level = initLevel;
        PlayerStatus.money = initMoney;
        status.Carbon = initCarbon;
        PlayerStatus.income = initIncome;
        status.Pikas = initPikas;
        status.UnlockPoints = initUnlockPoints;
        PlayerStatus.carbonX = 1;
        PlayerStatus.durationX = 1;
        PlayerStatus.priceX = 1;
        PlayerStatus.incomeX = 1;

        Debug.Log("New game started");
    }

    public static void SaveUpgradeTree()
    {
        foreach (Upgrade curr in UpgradeDatabase.upgrades)
            if (curr.unlocked)
                PlayerPrefs.SetInt(curr.upgradeName, 1);
        foreach (Upgrade curr in UpgradeDatabase.modifiers)
            if (curr.unlocked)
                PlayerPrefs.SetInt(curr.upgradeName, 1);
    }

    public static void LoadUpgradeTree()
    {
        foreach (Upgrade curr in UpgradeDatabase.upgrades)
        {
            if (PlayerPrefs.GetInt(curr.upgradeName) == 1)
                curr.unlocked = true;
            else
                curr.unlocked = false;
        }
        foreach (Upgrade curr in UpgradeDatabase.modifiers)
        {
            if (PlayerPrefs.GetInt(curr.upgradeName) == 1)
                curr.unlocked = true;
            else
                curr.unlocked = false;
        }
    }

    public static void SaveBuildings()
    {
        List<BuildingMenu> db = BuildingDatabase.buildings;
        for (int i = 0; i < db.Count; i++)
        {
            for (int z = 0; z < db[i].currUpgradeIds.Length; z++)
            {
                PlayerPrefs.SetInt(db[i].name + "_" + z.ToString(), db[i].currUpgradeIds[z]);
            }
        }
    }

    public static void LoadBuildings()
    {
        List<BuildingMenu> db = BuildingDatabase.buildings;
        for (int i = 0; i < db.Count; i++)
        {
            for (int z = 0; z < db[i].currUpgradeIds.Length; z++)
            {
                db[i].currUpgradeIds[z] = PlayerPrefs.GetInt(db[i].name + "_" + z.ToString());
                db[i].AnalyzeBuild();
            }
        }
    }
}
