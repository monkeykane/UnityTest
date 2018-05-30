using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bh_selfDestroy : bh_base
{
    [Range(1.0f, 50.0f)]
    public  float           lifetime = 20.0f;
    private float           startTime;
	// Use this for initialization
	void Start ()
    {
        startTime = Time.timeSinceLevelLoad;

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if ( (Time.timeSinceLevelLoad - startTime) 	> lifetime )
        {
            DestroyImmediate(gameObject);
        }
	}
}
