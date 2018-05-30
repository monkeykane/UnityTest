using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class bh_selfRotate : bh_base
{

    public float            rotatespeed;

    private void Start()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += Update;
#endif
    }
    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(Vector3.up * (Time.deltaTime * rotatespeed));
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= Update;
#endif      
    }
}
