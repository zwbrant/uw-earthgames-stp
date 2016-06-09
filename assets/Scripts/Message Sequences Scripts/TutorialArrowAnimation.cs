using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialArrowAnimation : MonoBehaviour {
    Image image;
    public float repeatRate = .5f;

	// Use this for initialization
	void Start () {
        image = this.gameObject.GetComponent<Image>();
        InvokeRepeating("SwitchVisibility", 0.0f, repeatRate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SwitchVisibility()
    {
        if (image.color.a > .1)
        {
            Color invisible = image.color;
            invisible.a = 0;
            image.color = invisible;
        } else
        {
            Color visible = image.color;
            visible.a = 1;
            image.color = visible;
        }
    }
}
