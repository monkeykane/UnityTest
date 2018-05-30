using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsGpp : MonoBehaviour
{
    private  GameObject          m_curBullet;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine( CreatePhyiscsWorld() );
	}
	
    IEnumerator CreatePhyiscsWorld()
    {
        // create ground
        TestObject  gd_to = Utilities.GetTestObject("Ground");
        GameObject ground = Utilities.InstantiateTestObject(gd_to);
        yield return new WaitForSeconds(2.0f);

        // deforme
        TestObject de_to = Utilities.GetTestObject("Deformer");
        float dgree = 0;
        float step = 45;
        float dis = 20;
        for ( int i = 0; i < 8; ++i )
        {
            float x = dis * Mathf.Sin(dgree);
            float z = dis * Mathf.Cos(dgree);
            GameObject de = Utilities.InstantiateTestObject(de_to);
            de.transform.position = new Vector3(x, 0, z ); 
            dgree += step;
        }
        yield return new WaitForSeconds(2.0f);

        // brick wall
        TestObject br_to = Utilities.GetTestObject("Brick");
        float offset = 0.5f;
        float LineBrick = 10;
        float lineCount = 10;
        float startX = -5;
        float startY = 0.5f;
        for( int i = 0; i < lineCount; ++i )
        {
            for( int j = 0; j < LineBrick; ++j )
            {
                GameObject br = Utilities.InstantiateTestObject(br_to);
                br.transform.position = new Vector3( startX + offset * i + 1 * j, startY + 0.5f * i, 0 );
            }
            --LineBrick;
        }


        yield break;
    }
    // Update is called once per frame
    void Update ()
    {
        if ( m_curBullet != null )
        {
            Rigidbody rb = m_curBullet.GetComponentInChildren<Rigidbody>();
            if ( rb != null )
                rb.AddForce(Camera.main.transform.forward * 10000.0f);
            m_curBullet = null;
        }
	    if ( Input.GetMouseButtonUp(0) )
        {
            Vector3 pos = Camera.main.gameObject.transform.position;
            
            // bullet
            TestObject bu_to = Utilities.GetTestObject("Bullet");
            m_curBullet = Utilities.InstantiateTestObject(bu_to);
            m_curBullet.transform.position = pos;
        }	
	}
}
