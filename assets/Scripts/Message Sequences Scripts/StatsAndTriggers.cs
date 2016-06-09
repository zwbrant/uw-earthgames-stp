using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsAndTriggers : MonoBehaviour {
    public List<DialogueEvent> dialogueEvents;
    internal static Dictionary<string, float> carbEfficiencies = new Dictionary<string, float>();
    internal static Dictionary<string, float> incomeEfficiencies = new Dictionary<string, float>();


    public void CheckDialogueEvents()
    {
        foreach (DialogueEvent dEvent in dialogueEvents)
        {
            bool reqSeqShown = true;
            if (dEvent.requisiteSeq != null)
                if (!dEvent.requisiteSeq.hasShown)
                    reqSeqShown = false;

            if (dEvent.minCarbon <= PlayerStatus.carbon
                && dEvent.minIncome <= PlayerStatus.income
                && dEvent.minMoney <= PlayerStatus.money
                && dEvent.minLevel <= PlayerStatus.level
                && reqSeqShown && dEvent.mesgSequence.showSequence 
                && !dEvent.mesgSequence.hasShown)
            {
                dEvent.mesgSequence.StartSequence();
            }
        }
    }

    [System.Serializable]
    public struct DialogueEvent
    {
        public MessageSequence mesgSequence;
        public MessageSequence requisiteSeq;
        public float minMoney;
        public float minIncome;
        public float minCarbon;
        public float minLevel;
    }


}
