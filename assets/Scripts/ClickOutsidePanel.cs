using UnityEngine;
using System.Collections;

public class ClickOutsidePanel : MonoBehaviour
{
    double startTime;

    // Use this for initialization
    void Start()
    {
        startTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerStatus.paused)
            startTime += Time.deltaTime;

        if (startTime > 5.0)
            Destroy(gameObject);
    }

    public void OnMouseDown()
    {
        Destroy(gameObject);

    }
}
