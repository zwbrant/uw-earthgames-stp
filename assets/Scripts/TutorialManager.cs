using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
    public bool showTutorial;
    //Tutorial screen prefab
    public GameObject tutorialScreen;
    public GameObject tutorialArrow;
    public GameObject veil;

    //Everything below is used to build the tutorialScreens
    public List<string> headings;
    public List<string> messages;
    public List<PikaSprites> pikaSprites;
    public List<GameObject> targetObjs;
    public List<bool> pauseGame;

    //The index which specifies which element of all the lists applies to the
    //current screen
    internal int currIndex;
    internal GameObject currArrow;
    internal GameObject currScreen;
    internal GameObject currVeil;
    internal GameObject currTargetObj;
    // Use this for initialization
    void Start () {
        currIndex = 0;
        if (showTutorial)
            CreateTScreen();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void CreateTScreen()
    {
        currScreen = GameObject.Instantiate(tutorialScreen);
        currScreen.transform.SetParent(PlayerStatus.uI.transform, false);

        Text heading = currScreen.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        Text body = currScreen.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        heading.text = headings[currIndex];
        body.text = messages[currIndex];

        if (pauseGame[currIndex] && !PlayerStatus.paused)
            GameMgmt.status.PauseGame();


        SetPikaSprite();
        CreateButton();
    }

    internal void SetPikaSprite()
    {
        GameObject gameObj = currScreen.transform.GetChild(1).gameObject;
        gameObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tutorial Pikas/" + pikaSprites[currIndex].ToString());
    }

    internal void CreateButton()
    {

        if (targetObjs[currIndex] == null)
        {
            currScreen.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => NextScreen()); //Default next button
        }
        else //Non-default next buttons
        {
            currTargetObj = targetObjs[currIndex].gameObject;
            if (currTargetObj.gameObject.GetComponent<Button>() != null) //if the object is a button
            {
                currTargetObj.GetComponent<Button>().onClick.AddListener(() => NextScreen());
                SpawnArrow();
                SetOrdering();
            }
            else
            {
 


            }
        }
    }

    public void SetOrdering()
    {

        currTargetObj.transform.SetSiblingIndex(currScreen.transform.GetSiblingIndex() - 1);
        currVeil.transform.SetSiblingIndex(currTargetObj.transform.GetSiblingIndex() - 1);

    }

    public void SpawnArrow()
    {
        currVeil = Instantiate(veil);
        currVeil.transform.SetParent(PlayerStatus.uI.transform, false);
        currArrow = Instantiate(tutorialArrow);
        currArrow.transform.SetParent(PlayerStatus.uI.transform, false);
        

        if (currTargetObj.transform.position.x > 0)
        {
            currArrow.transform.position = currTargetObj.transform.position + new Vector3(-140f, 0f, 0f);
            currArrow.transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
        } else
        {
            currArrow.transform.position = currTargetObj.transform.position + new Vector3(140f, 0f, 0f);
        }

    }

    public void NextScreen()
    {

 
            Debug.Log("Going to next screen (index: " + currIndex + ")");


            if (pauseGame[currIndex] && PlayerStatus.paused)
                GameMgmt.status.PauseGame();

            Destroy(currScreen.gameObject);
            if (currArrow != null)
                Destroy(currArrow);
            if (currVeil != null)
                Destroy(currVeil);

        if (currIndex < messages.Count - 1)
        {
            currIndex++;
            CreateTScreen();
        }


    }

    public enum PikaSprites
    {
        Default,
        DefaultHappy,
        DefaultSad,
        Female,
        Intro,
        Wise,
        Baby
    }
}
