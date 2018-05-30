using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class bh_setStartTransform : bh_base
{

    public Vector3  startPosition = Vector3.zero;
    public Vector3  startRotation = Vector3.zero;
    public Vector3  startScale = Vector3.one;


    // Use this for initialization
    void Start ()
    {
	    transform.position = startPosition;	
        transform.eulerAngles = startRotation;
        transform.localScale = startScale;
	}
}
