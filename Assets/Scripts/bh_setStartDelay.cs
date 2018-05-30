using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bh_setStartDelay : bh_base
{
    [Range(0.0f, 100.0f)]
    public float                startDelay;


    // Use this for initialization
    void Start ()
    {
	    ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem>();
        if ( ps != null )
        {
            //ParticleSystem.MainModule main = ps.main;
            //ParticleSystem.MinMaxCurve curve = main.startDelay;
            //curve.constant = startDelay;
            ps.startDelay = startDelay;
        }	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
