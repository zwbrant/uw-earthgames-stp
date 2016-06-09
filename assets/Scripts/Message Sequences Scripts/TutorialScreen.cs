using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialScreen : MonoBehaviour{
    [SerializeField]
    public string heading;
    public string message;
    public TutorialManager.PikaSprites pikaSprite;
    public GameObject targetObj;
    public bool pauseGame;


    //Base objects (prefabs) required for every screen
    public GameObject tScreen;
    public GameObject messageBox;
    public GameObject pikaSpritePrefab;
    

    public TutorialScreen(string heading, string message, TutorialManager.PikaSprites pikaSprite, GameObject targetObj)
    {
        this.heading = heading;
        this.message = message;
        this.pikaSprite = pikaSprite;
        this.targetObj = targetObj;
    }

}
