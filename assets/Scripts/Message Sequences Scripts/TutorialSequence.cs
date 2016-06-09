using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class TutorialSequence : MessageSequence {
    //public override void NextScreen()
    //{
    //    //Debug.Log("Going to next screen (index: " + index + ")");

    //    if (currTargetObj != null && currTargetObj.GetComponent<Button>() != null)
    //        currTargetObj.GetComponent<Button>().onClick.RemoveListener(currOnClick);
    //    //if (screens[index].pauseGame && PlayerStatus.paused)
    //    //    GameMgmt.status.PauseGame();

    //    Destroy(currScreen.gameObject);
    //    if (currArrow != null)
    //        Destroy(currArrow);
    //    if (currVeil != null)
    //        Destroy(currVeil);

    //    if (index < screens.Count - 1)
    //    {
    //        index++;
    //        CreateTScreen();
    //    }
    //    else
    //    {
    //        hasShown = true;
    //        BuildingMenu.disableMenus = false;

    //        PlayerStatus.statsAndTriggers.CheckDialogueEvents();

    //        if (disablePause)
    //        {
    //            pauseBut.GetComponent<Button>().interactable = true;
    //            //Removed un-pausing
    //        }
    //    }

    //}

}
