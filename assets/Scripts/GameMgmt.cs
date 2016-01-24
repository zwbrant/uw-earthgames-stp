using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMgmt : MonoBehaviour {

    //New game starting stats
    public const float initCarbon = 0;
    public const float initMoney = 20000;
    public const float initIncome = 0;
    public const int initPikas = 2;
    public const int initLevel = 1;
    public const int initUnlockPoints = 2;

    public void QuitGame()
    {
        Debug.Log("Saving & quitting game...");

        SaveGame();
        Application.Quit();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetFloat("CARBON", PlayerStatus.carbon);
        PlayerPrefs.SetFloat("MONEY", PlayerStatus.money);
        PlayerPrefs.SetInt("PIKAS", PlayerStatus.pikas);
        PlayerPrefs.SetFloat("INCOME", PlayerStatus.income);
        PlayerPrefs.SetInt("LEVEL", PlayerStatus.level);
        PlayerPrefs.SetInt("UNLOCKPOINTS", PlayerStatus.unlockPoints);
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
        PlayerStatus.money = PlayerPrefs.GetFloat("MONEY");
        PlayerStatus.income = PlayerPrefs.GetFloat("INCOME");
        PlayerStatus.carbon = PlayerPrefs.GetFloat("CARBON");
        PlayerStatus.pikas = PlayerPrefs.GetInt("PIKAS");
        PlayerStatus.level = PlayerPrefs.GetInt("LEVEL");
        PlayerStatus.unlockPoints = PlayerPrefs.GetInt("UNLOCKPOINTS");
        LoadBuildings();

        Debug.Log("Loaded previous game");
    }

    public static void StartNewGame()
    {
        PlayerStatus.money = initMoney;
        PlayerStatus.carbon = initCarbon;
        PlayerStatus.income = initIncome;
        PlayerStatus.pikas = initPikas;
        PlayerStatus.level = initLevel;
        PlayerStatus.unlockPoints = initUnlockPoints;

        Debug.Log("New game started");
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
