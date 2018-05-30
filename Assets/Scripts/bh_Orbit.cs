using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class bh_Orbit : bh_base
{
    public Vector3      center;
    public float        radius;
    public float        orbitSpeed;

    private float       angle;

	// Use this for initialization
	void Start ()
    {
	    angle = Random.Range(0,360);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += Update;
#endif
    }

    // Update is called once per frame
    void Update ()
    {
        
        float originX = center.x;
        float originZ = center.z;

        float circleX = (originX + (radius * Mathf.Cos(angle)));
        float circleZ = (originZ + (radius * Mathf.Sin(angle)));

        transform.localPosition = new Vector3(circleX, center.y, circleZ);

        angle += (orbitSpeed * Time.deltaTime) / 10;
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= Update;
#endif      
    }
}
