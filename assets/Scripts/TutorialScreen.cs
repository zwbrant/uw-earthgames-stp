using UnityEngine;
using System.Collections;

public class TutorialScreen {
    public string heading;
    public string message;
    public TutorialManager.PikaSprites pikaSprite;
    public GameObject requiredClick;

    //Base objects (prefabs) required for every screen
    public GameObject tScreen;
    public GameObject messageBox;
    public GameObject pikaSpritePrefab;
    

    public TutorialScreen(string heading, string message, TutorialManager.PikaSprites pikaSprite, GameObject requiredClick)
    {
        this.heading = heading;
        this.message = message;
        this.pikaSprite = pikaSprite;
        this.requiredClick = requiredClick;
    }

}
