using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[System.Serializable]
public class Shape
{
    virtual public GameObject Generate() { return null;}
    protected Shape() { }
    public static System.Type  GetShapeType( string typeInfo )
    {
        if ( typeInfo == null )
            return null;
        return System.Type.GetType(typeInfo);
    }
}

[System.Serializable]
public class Shape_Sphere : Shape
{
    public float radius = 0.5f;

    public override GameObject Generate()
    {
        GameObject ret = GameObject.CreatePrimitive( PrimitiveType.Sphere);
        ret.transform.localScale = new Vector3(radius, radius, radius);
        return ret;
    }
}

[System.Serializable]
public class Shape_Cube : Shape
{
    public Vector3 extent = Vector3.one;
    public override GameObject Generate()
    {
        GameObject ret = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ret.transform.localScale = extent;
        return ret;
    }
}

[System.Serializable]
public class Shape_Particles : Shape
{
    public UnityEngine.Object  particle;
    public override GameObject Generate()
    {
        GameObject ret = null;
        if ( particle != null)
        {
            ret = GameObject.Instantiate(particle) as GameObject;
        }
        return ret;
    }
}

[System.Serializable]
public class Shape_Pyramid : Shape
{
    public float length = 1.0f;
    public float height = 1.0f;
    public override GameObject Generate()
    {
        GameObject ret = new GameObject("Pyramid");
        Mesh mesh = new Mesh();
        mesh.name = "Pyramid_mesh";
        mesh.vertices = new Vector3[]
        {
            new Vector3( 0, height, 0),
            new Vector3( length *0.5f, 0, length * 0.5f),
            new Vector3( length *0.5f, 0, -length * 0.5f),
            new Vector3( -length *0.5f, 0, -length * 0.5f),
            new Vector3( -length *0.5f, 0, length * 0.5f),
        };

        mesh.uv = new Vector2[]
        {
            new Vector2 ( 1, 1),
            new Vector2( 0.5f, 0.5f),
            new Vector2( 0.5f, -0.5f),
            new Vector2( -0.5f, -0.5f),
            new Vector2( -0.5f, 0.5f),
        };

        mesh.triangles = new int[] {0,1,2,0,2,3,0,3,4,0,4,1,1,4,3,3,2,1 };

        mesh.RecalculateNormals();

        MeshFilter mf = ret.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = ret.AddComponent<MeshRenderer>();
        mr.sharedMaterial = new Material(Shader.Find("Mobile/Diffuse"));
        return ret;
    }
}

[System.Serializable]
public class Shape_Mesh : Shape
{
    public UnityEngine.Object       mesh_prefab;
    public override GameObject Generate()
    {
        GameObject ret = null;
        if (mesh_prefab != null)
        {
            ret = GameObject.Instantiate(mesh_prefab) as GameObject;
        }
        return ret;
    }
}
