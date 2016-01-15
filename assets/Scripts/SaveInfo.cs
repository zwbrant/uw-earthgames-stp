using UnityEngine;
using System.Collections;

public class SaveInfo : MonoBehaviour {

	//Saves all the player's info so that it can be accessed in other scenes or play sessions
	public static void SaveAll() {
		PlayerPrefs.SetFloat ("MONEY", PlayerStatus.money); 
		PlayerPrefs.SetFloat ("CARBON", PlayerStatus.carbon); 
	}
}
