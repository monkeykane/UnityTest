using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bh_meshDeformer : bh_base
{
    public      float           deformSpeed = 4.0f;
    public      float           force = 3.0f;

    private     MeshFilter      deformMeshFilter;
    private     Vector3[]       originVex;
    private     Vector3[]       deformVex;

	// Use this for initialization
	void Start ()
    {
	    deformMeshFilter = GetComponentInChildren<MeshFilter>();
        if ( deformMeshFilter != null )
        {
            if (deformMeshFilter.mesh != null)
            {
                originVex = deformMeshFilter.mesh.vertices;
                deformVex = new Vector3[originVex.Length];
                for( int i = 0; i < originVex.Length; ++i )
                {
                    deformVex[i] = originVex[i];
                }
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
		for( int i = 0; i < originVex.Length; ++i )
        {
            if ( originVex[i].y > 0 )
            {
                deformVex[i].x = originVex[i].x + force * Mathf.Sin( deformSpeed * Time.timeSinceLevelLoad );
            }
            else
            {
                deformVex[i].x = originVex[i].x - force * Mathf.Sin(deformSpeed * Time.timeSinceLevelLoad);
            }
        }

        if (deformMeshFilter.mesh != null )
        {
            deformMeshFilter.mesh.vertices = deformVex;
            deformMeshFilter.mesh.RecalculateNormals();
        }
	}
}
