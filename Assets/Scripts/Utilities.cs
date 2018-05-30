using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Text;

public class Utilities
{

    // load property from string
    public static void LoadObjectFields(string s, object Obj)
    {
        if (s == null || s == string.Empty )
            return;
        string[] s_value = s.Split(';');
        if (s_value.Length == 0)
            return;
        FieldInfo[] fInfos = Obj.GetType().GetFields();
        for (int i = 0; i < s_value.Length; ++i)
        {
            if ( s_value[i] == string.Empty )
                continue;
            string[] name_pro = s_value[i].Split(':');
            if ( name_pro.Length != 2 )
                continue;
            string name = string.Empty;
            string pvalue = string.Empty;

            try
            {
                name = name_pro[0];
                pvalue = name_pro[1];
            }
            catch(Exception)
            {
                Debug.LogError("Faild to Load object field: " + name_pro );
                throw;
            }
            FieldInfo fi = null;
            for( int j = 0; j < fInfos.Length; ++j )
            {
                if ( string.Compare(fInfos[j].Name , name ) == 0 )
                {
                    fi = fInfos[j];
                    break;
                }
            }
            if ( fi == null )
                continue; // go to next field
            {
                if (fi.FieldType == typeof(Single))
                {
                    float value = 0;
                    if (float.TryParse(pvalue, out value))
                        fi.SetValue(Obj, value);
                }
                else if (fi.FieldType == typeof(Vector3))
                {
                    pvalue = pvalue.Replace("(", "").Replace(")", "");
                    string[] vec_string = pvalue.Split(',');
                    Vector3 value = Vector3.zero;
                    for (int j = 0; j < vec_string.Length; ++j)
                    {
                        float outF = 0;
                        if (float.TryParse(vec_string[j], out outF))
                        {
                            value[j] = outF;
                        }
                    }

                    fi.SetValue(Obj, value);
                }
                else if (fi.FieldType == typeof(UnityEngine.Object))
                {
                    int offset = pvalue.IndexOf("Resources/");
                    if (offset != -1)
                    {
                        string path = pvalue.Substring(offset + "Resources/".Length);
                        int dotpos = path.LastIndexOf('.');
                        if (dotpos != -1)
                            path = path.Remove(dotpos);
                        UnityEngine.Object value = Resources.Load(path);

                        fi.SetValue(Obj, value);
                    }
                }
            }
        }
    }

    // save property from string, only run in editor mode
    public static string SaveObjectFields(object obj)
    {
        StringBuilder sb = new StringBuilder();
        foreach (FieldInfo fi in obj.GetType().GetFields())
        {
            sb.Append(fi.Name);
            sb.Append(":");
            if (fi.FieldType == (typeof(UnityEngine.Object)))
            {
#if UNITY_EDITOR
                UnityEngine.Object value = fi.GetValue(obj) as UnityEngine.Object;
                if (value != null)
                    sb.Append( UnityEditor.AssetDatabase.GetAssetPath(value));
                else
                    sb.Append("None");
#else
                sb.Append("None");
#endif
            }
            else
                sb.Append(fi.GetValue(obj));

            sb.Append(";");
        }
        return sb.ToString();
    }

    // Get Type, for cross assembly set
    public static Type GetType( string typeInfo)
    {
        if (typeInfo == null )
            return null;
        return Type.GetType(typeInfo);
    }

    // Instantiate Test Object
    public static GameObject InstantiateTestObject(TestObject to )
    {
        GameObject ret = null;
        if ( to != null )
        {
            ret = new GameObject(to.name);
            string shapeinfo = to.shapeDatas.typeInfo;
            Type shapeType = GetType(shapeinfo);
            Shape shape = Activator.CreateInstance(shapeType) as Shape;
            if ( shape != null && to.shapeDatas.propertys!=null )
            {
                LoadObjectFields(to.shapeDatas.propertys, shape);
            }
            GameObject shape_go = shape.Generate();
            if ( shape_go != null )
                shape_go.transform.parent = ret.transform;
            // update texture
            {
                MeshRenderer[] meshs = ret.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer m in meshs)
                {
                    var tempMat = new Material(m.sharedMaterial);
                    tempMat.mainTexture = to.texture;
                    m.sharedMaterial = tempMat;
                }
            }

            // behaviors
            if (to.bhDatas != null)
            {
                for (int i = 0; i < to.bhDatas.Count; ++i)
                {
                    string typeinfo = to.bhDatas[i].typeInfo;
                    if (typeinfo != null)
                    {
                        Type type = Utilities.GetType(typeinfo);
                        if (type != null)
                        {
                            MonoBehaviour com = ret.AddComponent(type) as MonoBehaviour;
                            if (com != null)
                            {
                                Utilities.LoadObjectFields(to.bhDatas[i].propertys, com);

                            }
                        }
                    }
                }
            }
        }
        return ret;
    }

    // get test object
    public static TestObject GetTestObject( string name )
    {
        TestObject to = null;
        for( int i = 0; i < ObjectLists.Instance.objectList.Count; ++i )
        {
            if (string.Compare(ObjectLists.Instance.objectList[i].name , name) == 0)
            {
                to = ObjectLists.Instance.objectList[i];
                break;
            }
        }
        return to;
    }
}
