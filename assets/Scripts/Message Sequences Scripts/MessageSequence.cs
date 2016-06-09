using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public abstract class MessageSequence : MonoBehaviour {
    public bool showSequence;
    internal bool hasShown = false;
    public bool disablePause = false;

    //Tutorial screen prefabs
    public GameObject tutorialScreen;
    public GameObject tutorialArrow;
    public GameObject veil;
    internal GameObject pauseBut;

    public List<MessageScreen> screens;

    [System.Serializable]
    public struct MessageScreen
    {
        public string heading;
        public string body;
        //public bool isCompact;
        public DialoguePosition entryPoint;
        public DialoguePosition exitPoint;
        public PikaSprites pikaSprite;
        public GameObject targetObj;
        public string targetObjName;
    }


    internal int index; //Pointer for the sequence
    internal GameObject currArrow;
    internal GameObject currScreen;
    internal GameObject currVeil;
    internal GameObject currTargetObj;
    internal UnityAction currOnClick;

    public void StartSequence()
    {
        pauseBut = GameObject.Find("Pause/Play");
        BuildingMenu.disableMenus = true;
        if (disablePause)

        {
            GameMgmt.status.PauseGame(true, false);
            pauseBut.GetComponent<Button>().interactable = false;
        }

        index = 0;

        CreateTScreen();

    }

    internal Vector3 initialPos, topPos, leftPos, rightPos, bottomPos, compactPos;

    //internal Vector3 compactPos;
    internal Vector3 targetPos;
    internal float screenStartTime; //Time when the screen was created (for time based behavior)
    internal float screenTimeLimit;

    internal virtual void CreateTScreen()
    {
        currScreen = GameObject.Instantiate(tutorialScreen);
        currScreen.transform.SetParent(PlayerStatus.uI.transform, false);

        initialPos = currScreen.transform.position;
        topPos = new Vector3(initialPos.x, initialPos.y + 500f);
        leftPos = new Vector3(initialPos.x - 900f, initialPos.y);
        rightPos = new Vector3(initialPos.x + 900f, initialPos.y);
        bottomPos = new Vector3(initialPos.x, initialPos.y - 500f);
        compactPos = new Vector3(initialPos.x, initialPos.y - 270f);

        currScreen.transform.position = GetPosition(screens[index].entryPoint);
        targetPos = initialPos;
        //compactPos = new Vector3(initialPos.x, initialPos.y - 200f);

        //if (index == 0 && this.GetType().Name != "ContinuedTutorialSeq") {
        //    currScreen.transform.position = hiddenPos;
        //    targetPos = initialPos;
        //} else if (index != 0 && screens[index - 1].isCompact && screens[index].isCompact)
        //{
        //    currScreen.transform.position = compactPos;
        //    targetPos = compactPos;
        //} else if (screens[index].isCompact)
        //{
        //    targetPos = compactPos;
        //} else if (index != 0 && screens[index - 1].isCompact)
        //{
        //    currScreen.transform.position = compactPos;
        //}

        Text heading = currScreen.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        Text body = currScreen.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        heading.text = screens[index].heading;
        body.text = screens[index].body;

        screenStartTime = Time.time;
        screenTimeLimit = 2.2f + body.text.Length / 18f;

        SetPikaSprite();
        CreateButton();
    }

    internal virtual Vector3 GetPosition(DialoguePosition dialoguePos)
    {
        switch (dialoguePos) {
            case DialoguePosition.Top :
                return topPos;
            case DialoguePosition.Left:
                return leftPos;
            case DialoguePosition.Right:
                return rightPos;
            case DialoguePosition.Bottom:
                return bottomPos;
            default :
                return initialPos;
        }

    }

    internal virtual void SetPikaSprite()
    {
        GameObject gameObj = currScreen.transform.GetChild(1).gameObject;
        gameObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tutorial Pikas/" + screens[index].pikaSprite.ToString());
    }

    internal bool clicked = false; //Next button has been clicked (animate now)
    internal virtual void CreateButton()
    {

        if (screens[index].targetObj == null && (screens[index].targetObjName == null || screens[index].targetObjName == "")) //Doesn't require a particular next button
        {
            currScreen.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => NextScreen()); //Default next button
        }
        else //Non-default next buttons
        {
            currOnClick = new UnityAction(NextScreen);
            if (screens[index].targetObjName != "" && screens[index].targetObjName != null)
                currTargetObj = GameObject.Find(name);
            else 
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

    public void SetOrdering()
    {
        if (currTargetObj.transform.parent.name == "TopHUD")
        {
            currTargetObj.transform.parent.transform.SetSiblingIndex(currScreen.transform.GetSiblingIndex() - 1);
            currVeil.transform.SetSiblingIndex(currTargetObj.transform.parent.transform.GetSiblingIndex() - 1);
        }
        else if (currTargetObj.transform.parent.name.Contains("UpgradeMenu")) {

        } else {
            currTargetObj.transform.SetSiblingIndex(currScreen.transform.GetSiblingIndex() - 1);
            currVeil.transform.SetSiblingIndex(currTargetObj.transform.GetSiblingIndex() - 1);
        }

    }

    public void SpawnArrow()
    {
        currVeil = Instantiate(veil);
        currArrow = Instantiate(tutorialArrow);

        currVeil.transform.SetParent(PlayerStatus.uI.transform, false);
        currArrow.transform.SetParent(PlayerStatus.uI.transform, false);

        //Debug.Log("Target Obj's X: " + currTargetObj.transform.position.x);
        
        if (currTargetObj.transform.position.x > Screen.width / 2)
        {
            currArrow.transform.position = currTargetObj.transform.position + new Vector3(-125f, 0f, 0f);
            currArrow.transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
        }
        else
        {
            currArrow.transform.position = currTargetObj.transform.position + new Vector3(125f, 0f, 0f);
        }

    }

    public virtual void NextScreen()
    {
        if (screens[index].exitPoint != DialoguePosition.Default && clicked == false)
        {
            clicked = true;
            targetPos = GetPosition(screens[index].exitPoint);
        } else { 
            if (currTargetObj != null && currTargetObj.GetComponent<Button>() != null)
            {
                currTargetObj.GetComponent<Button>().onClick.RemoveListener(currOnClick);
                currTargetObj = null;
            }
            //if (screens[index].pauseGame && PlayerStatus.paused)
            //    GameMgmt.status.PauseGame();

            Destroy(currScreen.gameObject);
            if (currArrow != null)
                Destroy(currArrow);
            if (currVeil != null)
                Destroy(currVeil);

            if (index < screens.Count - 1)
            {
                index++;
                CreateTScreen();
            }
            else
            {
                hasShown = true;
                BuildingMenu.disableMenus = false;

                PlayerStatus.statsAndTriggers.CheckDialogueEvents();

                if (disablePause)
                {
                    pauseBut.GetComponent<Button>().interactable = true;
                    //GameMgmt.status.PauseGame(false, false);
                }
            }
            clicked = false;
        }

    }

    public virtual void MoveCurrScreen()
    {
        if (currScreen.transform.position != targetPos)
        {
            currScreen.transform.position = Vector3.MoveTowards(currScreen.transform.position, targetPos,  16);
        } else if (clicked)
        {
            NextScreen();
        }
    }

    public virtual void Update()
    {
        if (currScreen != null && currTargetObj == null && Time.time - screenStartTime > screenTimeLimit) //Time limit reached
        {
            if (screens[index].exitPoint != DialoguePosition.Default && clicked == false)
            {
                clicked = true;
                targetPos = GetPosition(screens[index].exitPoint);
            }
            else if (targetPos == currScreen.transform.position)
            {
                NextScreen();
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if (currScreen != null)
            MoveCurrScreen();
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

    public enum DialoguePosition { Default, Top, Left, Right, Bottom }
}
