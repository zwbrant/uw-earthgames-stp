using UnityEngine;
using System.Collections;

public class SmogBehavior : MonoBehaviour {
    public GameObject townSmog;
    public float driftSpeed;


    private static Renderer[] renderers;
    private bool hasSpawnedNew = false;

	// Use this for initialization
	void Start () {
        //renderers = GetComponentsInChildren<Renderer>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (transform.position.x >= 75 && !hasSpawnedNew)
        {
            hasSpawnedNew = true;
            GameObject newSmog = Instantiate(this.gameObject);
            newSmog.transform.SetParent(transform.parent.transform, false);
            newSmog.transform.localPosition = new Vector3(-844.9f, -19f, -909.9f);
        }
        if (transform.position.x >= 920)
        {
            Destroy(this.gameObject);
        }

            if (!PlayerStatus.paused)
            transform.position += townSmog.transform.right * Time.deltaTime * driftSpeed;
	}

    public static void SetCutoff()
    {
        float cutoff = PlayerStatus.carbon / 10000;
        GameObject[] smogs = GameObject.FindGameObjectsWithTag("SmogContainer");

        for (int i = 0; i < smogs.Length; i++)
        {
            renderers = smogs[i].GetComponentsInChildren<Renderer>();
            for (int z = 0; z < renderers.Length; z++)
            {
                renderers[z].material.SetFloat("_Cutoff", cutoff * .4f + .5f);
            }
        }
        
    }

}
