using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public string                   typeInfo;
    public string                   propertys;  //  name1:value1; name2:value2...
    public ObjectData()
    {
        typeInfo = string.Empty;
        propertys = string.Empty;
    }
}


[System.Serializable]
public class TestObject
{
    public string                   name;
    // shape
    [HideInInspector]
    public Shape                    shape;
    public ObjectData               shapeDatas = new ObjectData();

    // texture
    public Texture2D                texture;

    // behavior
    public List<ObjectData>         bhDatas;
}


public class ObjectLists    :   ScriptableObject
{
    public List<TestObject>         objectList;

    private static ObjectLists      instance;
    public static ObjectLists       Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ObjectLists>("ObjectLists");

                if (instance == null)
                {
                    instance = ScriptableObject.CreateInstance<ObjectLists>();
                    instance.objectList = new List<TestObject>(50);

#if UNITY_EDITOR
                    // Save into Resource folder
                    string properPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
                    if (!System.IO.Directory.Exists(properPath))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    string fullPath = System.IO.Path.Combine(
                      System.IO.Path.Combine("Assets", "Resources"),
                      "ObjectLists.asset"
                    );
                    UnityEditor.AssetDatabase.CreateAsset(instance, fullPath);
#endif
                }
            }
            return instance;
        }

        set
        {
            instance = value;
        }
    }

}