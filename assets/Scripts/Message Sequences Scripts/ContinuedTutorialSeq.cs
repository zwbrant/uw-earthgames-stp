using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ContinuedTutorialSeq : MessageSequence {
//    internal override void CreateTScreen()
//    {
//        currScreen = GameObject.Instantiate(tutorialScreen);
//        currScreen.transform.SetParent(PlayerStatus.uI.transform, false);
//        initialPos = currScreen.transform.position;
//        targetPos = initialPos;
//        bottomPos = new Vector3(initialPos.x, initialPos.y - 420f);
////        compactPos = new Vector3(initialPos.x, initialPos.y - 200f);

//        //if (index == 0 && this.GetType().Name != "ContinuedTutorialSeq")
//        //{
//        //    currScreen.transform.position = hiddenPos;
//        //    targetPos = initialPos;
//        //}
//        //else if (index != 0 && screens[index - 1].isCompact && screens[index].isCompact)
//        //{
//        //    currScreen.transform.position = compactPos;
//        //    targetPos = compactPos;
//        //}
//        //else if (screens[index].isCompact)
//        //{
//        //    targetPos = compactPos;
//        //}
//        //else if (index != 0 && screens[index - 1].isCompact)
//        //{
//        //    currScreen.transform.position = compactPos;
//        //}

//        Text heading = currScreen.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
//        Text body = currScreen.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
//        heading.text = screens[index].heading;
//        body.text = screens[index].body;

//        screenStartTime = Time.time;
//        if (index == 18)
//            screenTimeLimit = 2f;
//        else if (index == 21)
//            screenTimeLimit = 3.5f;
//        else
//            screenTimeLimit = 1.5f + body.text.Length / 14f;
//        SetPikaSprite();
//        CreateButton();
//    }

    public override void NextScreen()
    {

        if (currTargetObj != null && currTargetObj.GetComponent<Button>() != null)
            currTargetObj.GetComponent<Button>().onClick.RemoveListener(currOnClick);
        //if (screens[index].pauseGame && PlayerStatus.paused)
        //    GameMgmt.status.PauseGame();
        
        Destroy(currScreen.gameObject);
        if (currArrow != null)
            Destroy(currArrow);
        if (currVeil != null)
            Destroy(currVeil);

        if (index < screens.Count - 1)
        {
            BuildingMenu.disableMenus = true;
            index++;
            CreateTScreen();
        }
        else if (index >= screens.Count -1)
        {
            hasShown = true;
            BuildingMenu.disableMenus = false;

            PlayerStatus.statsAndTriggers.CheckDialogueEvents();

            if (disablePause)
            {
                pauseBut.GetComponent<Button>().interactable = true;
                GameMgmt.status.PauseGame(false, false);
            }
        }

    }



    internal override void CreateButton()
    {
        if (index != 18)
        {
            if (screens[index].targetObj == null) //Doesn't require a particular next button
            {
                if (index == 21)
                {
                    currScreen.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => HideCurrScreen());
                }
                else {
                    currScreen.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => NextScreen()); //Default next button
                }
            }
            else //Non-default next buttons
            {
                currOnClick = new UnityAction(NextScreen);
                currTargetObj = screens[index].targetObj.gameObject;
                if (currTargetObj.gameObject.GetComponent<Button>() != null) //if the object is a button
                {
                    currTargetObj.GetComponent<Button>().onClick.AddListener(currOnClick);
                }
                else
                {
                    currScreen.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => NextScreen()); //Default next button
                }
                SpawnArrow();
                SetOrdering();
            }
        }
    }

    void HideCurrScreen() {
        targetPos = compactPos;
        if (index == 21 && currArrow == null)
        {
            BuildingMenu.disableMenus = false;
            currOnClick = new UnityAction(NextScreen);
            currTargetObj = GameObject.Find("CFL LightbulbsUpgrade").transform.GetChild(1).gameObject;
            currTargetObj.GetComponent<Button>().onClick.AddListener(currOnClick);

            SpawnArrow();
            SetOrdering();
        }
    }


    public override void Update()
    {
        if (index == 18 && GameObject.Find("UpgradeMenu(Clone)") != null)
        {
            BuildingMenu.disableMenus = true;
            //screens[19].targetObj = GameObject.Find(")
            NextScreen();

        } else if (index == 18)
        {
            BuildingMenu.disableMenus = false;
        }

        
        if (currScreen != null &&  (index == 18 || index == 21) && Time.time - screenStartTime > screenTimeLimit)
        {
            HideCurrScreen();
        } else if (currScreen != null && Time.time - screenStartTime > screenTimeLimit)
        {
            NextScreen();
        }
    }

    
}
