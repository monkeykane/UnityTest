using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class bh_setPhysics : bh_base
{
    public  float           mass = 10;

	// Use this for initialization
	void Start ()
    {
	    Collider co = GetComponentInChildren<Collider>();
        if ( co != null )
        {
            Rigidbody rb = co.gameObject.GetComponent<Rigidbody>();
            if ( rb == null )
                rb = co.gameObject.AddComponent<Rigidbody>();
            rb.mass = mass;
            //rb.Sleep();
        }	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
