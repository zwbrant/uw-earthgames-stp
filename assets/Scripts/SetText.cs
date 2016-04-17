using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetText : MonoBehaviour {
		
		
		Text text;                      // Reference to the Text component.
		
		
		void Awake ()
		{
			// Set up the reference.
			text = GetComponent <Text> ();
		}
		
		
		void Update ()
		{
			// Set the displayed text to be the word "Score" followed by the score value.
			text.text = "$ " + PlayerStatus.money.ToString();
		}

		public void ChangeMoney(int moneyChange) {
				PlayerStatus.money += moneyChange;
		}



	
}