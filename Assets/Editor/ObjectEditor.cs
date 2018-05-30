using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEditor;
using WH.Editor;
using System.Text;

// Object Editor: adding, deleteing, modifing Object
public class ObjectEditor : EditorWindow
{
    // editor UI
    private GUIStyle                m_labelStyle = new GUIStyle();
    private Color                   m_orgColor;

    // shape info
    private List<Type>              m_shapeTypeList;
    private List<string>            m_shapeTypeName;

    // behavior info
    private List<Type>              m_bhTypeList;
    private List<string>            m_bhTypeName;

    // dirty flag, for recreating test object
    private bool                    m_dirty = false;

    // test object
    private GameObject              m_curInstance;

    // list view
    protected ListViewState         m_ListView;
    protected bool                  m_Focus;

    protected class Styles
    {
        public readonly GUIStyle listItem = new GUIStyle("PR Label");
        public readonly GUIStyle listItemBackground = new GUIStyle("CN EntryBackOdd");
        public readonly GUIStyle listItemBackground2 = new GUIStyle("CN EntryBackEven");
        public readonly GUIStyle listBackgroundStyle = new GUIStyle("CN Box");
        public Styles()
        {
            Texture2D background = this.listItem.hover.background;
            this.listItem.onNormal.background = background;
            this.listItem.onActive.background = background;
            this.listItem.onFocused.background = background;
        }
    }
    protected static Styles         s_Styles;


    // current selected item
    private TestObject              m_selected = null;


    [MenuItem("Test/Object Editor")]
    static void OpenObjectEditor()
    {
        Rect rect = new Rect(320, 180, 640, 720);
        ObjectEditor windows = EditorWindow.GetWindowWithRect<ObjectEditor>(rect, true, "Object Editor", true);
        windows.Show();
    }

    private void OnEnable()
    {
        m_labelStyle.alignment = TextAnchor.UpperCenter;
        m_labelStyle.fontSize = 14;
        m_labelStyle.fontStyle = FontStyle.Bold;

        m_ListView = new ListViewState();

        // get shape type list;
        Assembly asm = Assembly.GetAssembly(typeof(Shape));
        m_shapeTypeList = new List<Type>(asm.GetTypes().Length + 1);  // one more then count of shapes, first is none shape
        m_shapeTypeName = new List<string>(asm.GetTypes().Length + 1);
        m_shapeTypeList.Add(null);
        m_shapeTypeName.Add("None");
        foreach (System.Type child in asm.GetTypes())
        {
            if (child.BaseType == typeof(Shape))
            {
                m_shapeTypeList.Add(child);
                m_shapeTypeName.Add(child.ToString());
            }
        }

        // get behavior type list
        m_bhTypeList = new List<Type>(asm.GetTypes().Length + 1);
        m_bhTypeName = new List<string>(asm.GetTypes().Length + 1 );
        m_bhTypeList.Add(null);
        m_bhTypeName.Add("None");
        foreach( Type child in asm.GetTypes() )
        {
            //Type if_type = child.GetInterface(typeof(bh_interface).FullName);
            if (child.BaseType == typeof(bh_base))
            {
                m_bhTypeList.Add(child);
                m_bhTypeName.Add(child.ToString());
            }
        }
    }

    private void OnFocus()
    {
        m_Focus = true;
    }

    private void OnLostFocus()
    {
        m_Focus = false;
    }

    // draw Windows
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(220), GUILayout.Height(700));
        EditorGUIUtility.labelWidth = 110f;
        m_orgColor = GUI.color;

        _ObjectListViewEditor();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Object Editor:", m_labelStyle);

        if (m_selected != null && m_ListView.row != -1)
        {

            TestObject obj = ObjectLists.Instance.objectList[m_ListView.row];

            // name
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object Name:", EditorStyles.boldLabel);
            obj.name = EditorGUILayout.TextField(m_selected.name, GUILayout.Width(280));
            EditorGUILayout.EndHorizontal();
            // end name section

            EditorGUILayout.Space(); //---------------------------------------------------------------

            // texture
            EditorGUILayout.BeginVertical(GUI.skin.box);
            _ObjectTextureEditor(obj);
            EditorGUILayout.EndVertical();
            // end texture section

            EditorGUILayout.Space(); //---------------------------------------------------------------

            // render shapes
            EditorGUILayout.BeginVertical(GUI.skin.box);
            _ObjectShapeEditor(obj);
            EditorGUILayout.EndVertical();
            // end shape section

            EditorGUILayout.Space(); //---------------------------------------------------------------

            // behavior section
            EditorGUILayout.BeginVertical(GUI.skin.box);
            _ObjectBehaviorEditor(obj);
            EditorGUILayout.EndVertical();
            // end behavior section

            EditorGUILayout.Space(); //---------------------------------------------------------------

            // save to prefae
            _SaveObjectToPrefab();

            EditorGUILayout.Space(); //---------------------------------------------------------------
            if (m_dirty)
            {
                _Instantiate(m_selected);
                EditorUtility.SetDirty(ObjectLists.Instance);
                m_dirty = false;
            }
        }
        else
        {
            ClearTestObjectEntity();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();



    }

    // UI: object list view
    private void _ObjectListViewEditor()
    {
        // Object List Section:
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Object Lists:", m_labelStyle);

        if (s_Styles == null)
        {
            s_Styles = new Styles();
        }
        m_ListView.totalRows = ObjectLists.Instance.objectList.Count;
        Event current = Event.current;

        GUIContent textContent = new GUIContent();
        foreach (ListViewElement el in ListViewGUI.ListView(m_ListView, s_Styles.listBackgroundStyle))
        {
            if (current.type == EventType.MouseDown && current.button == 0 && el.position.Contains(current.mousePosition) && current.clickCount == 1)
            {
                Debug.Log(el.row);
                m_selected = ObjectLists.Instance.objectList[el.row];
                if ( el.row != m_ListView.row )
                {
                    m_dirty = true;
                }
            }
            if (current.type == EventType.Repaint)
            {
                textContent.text = ObjectLists.Instance.objectList[el.row].name;

                GUIStyle style = (el.row % 2 != 0) ? s_Styles.listItemBackground2 : s_Styles.listItemBackground;
                style.Draw(el.position, false, false, m_ListView.row == el.row, false);
                s_Styles.listItem.Draw(el.position, textContent, false, false, m_ListView.row == el.row, m_Focus);
            }
        }

        // add object button
        if (GUILayout.Button("Add Object"))
        {
            TestObject item = new TestObject();
            item.name = "TestObject";
            ObjectLists.Instance.objectList.Add(item);
            m_dirty = true;
        }

        // delete object button
        GUI.color = Color.red;
        if (GUILayout.Button("Delete Object") && m_selected != null)
        {
            ObjectLists.Instance.objectList.Remove(m_selected);

            m_selected = null;

            m_ListView.row = -1;

            m_dirty = true;
        }
        GUI.color = m_orgColor;

        EditorGUILayout.EndVertical();
        // end List Section
    }

    // UI: object texture
    private void _ObjectTextureEditor( TestObject obj)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Texture:", EditorStyles.boldLabel);
        Texture2D newOne = (Texture2D)EditorGUILayout.ObjectField(obj.texture, typeof(Texture2D), false);
        if (newOne != obj.texture)
        {
            obj.texture = newOne;
            m_dirty = true;
        }
        EditorGUILayout.EndHorizontal();

    }

    // UI: object shape editor
    private void _ObjectShapeEditor( TestObject obj )
    {
        EditorGUILayout.LabelField("Shape:", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        System.Type type = null;
        if ( obj.shapeDatas != null )
            type = Utilities.GetType(obj.shapeDatas.typeInfo);

        if (type != null)
        {
            if ( obj.shape == null || obj.shape.GetType() != type )
                obj.shape = Activator.CreateInstance(type) as Shape;
            if (obj.shape != null)
                Utilities.LoadObjectFields(obj.shapeDatas.propertys, obj.shape);
            m_dirty = true;
        }
        else
        {
            obj.shape = null;
        }

        string[] options = m_shapeTypeName.ToArray();
        int index = 0;
        if (obj.shape != null)
        {
            for (int i = 0; i < m_shapeTypeList.Count; ++i)
            {
                if (obj.shape.GetType() == m_shapeTypeList[i])
                {
                    index = i;
                    break;
                }
            }
        }
        int newIndex = EditorGUILayout.Popup(index, options);
        if (newIndex != index)
        {
            if (newIndex == 0)
            {
                obj.shape = null;
                obj.shapeDatas.typeInfo = "";
                obj.shapeDatas.propertys = "";
            }
            else
            {
                Type objType = m_shapeTypeList[newIndex];
                obj.shape = (Shape)Activator.CreateInstance(objType);
                obj.shapeDatas.typeInfo = objType.ToString();
            }

            m_dirty = true;
        }

        if (obj.shape != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fields:", EditorStyles.boldLabel);
            foreach (FieldInfo fi in obj.shape.GetType().GetFields())
            {
                if (_ShowField(obj.shape, fi))
                {
                    obj.shapeDatas.propertys = Utilities.SaveObjectFields(obj.shape);
                }
            }
        }
        EditorGUILayout.Space();
    }

    // UI: behavior editor
    Vector2 scrollPos;
    private void _ObjectBehaviorEditor(TestObject obj)
    {
        EditorGUILayout.LabelField("Behavior:", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        List<int> removed = new List<int>(20);
        string[] options = m_bhTypeName.ToArray();
        if (obj.bhDatas != null)
        {
            for (int i = 0; i < obj.bhDatas.Count; ++i)
            {
                int index = 0;
                if (obj.bhDatas[i].typeInfo != null)
                {
                    for (int j = 0; j < options.Length; ++j)
                    {
                        if (string.Compare(obj.bhDatas[i].typeInfo, options[j]) == 0)
                        {
                            index = j;
                            break;
                        }
                    }
                }
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal();
                int newIndex = EditorGUILayout.Popup(index, options);
                if (newIndex != index)
                {
                    if (newIndex == 0)
                        obj.bhDatas[i].typeInfo = null;
                    else
                        obj.bhDatas[i].typeInfo = options[newIndex];
                }

                // delete button
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(18)))
                {
                    removed.Add(i);
                }
                GUI.color = m_orgColor;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                // show Property
                
                Type bhType = m_bhTypeList[newIndex];
                if (bhType != null)
                {
                    object bh = Activator.CreateInstance(bhType);
                    Utilities.LoadObjectFields(obj.bhDatas[i].propertys, bh);
                    foreach (FieldInfo fi in bhType.GetFields())
                    {

                        if (bh != null)
                        {
                            if (_ShowField(bh, fi))
                            {
                                obj.bhDatas[i].propertys = Utilities.SaveObjectFields(bh);
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }
        foreach (int s in removed)
        {
            if ( s < obj.bhDatas.Count )
                obj.bhDatas.RemoveAt(s);
        }
        removed.Clear();
        removed = null;
        EditorGUILayout.EndScrollView();

        // Add Behavior
        GUI.color = Color.green;
        if (GUILayout.Button("Add Behavior"))
        {
            if (obj.bhDatas == null)
            {
                obj.bhDatas = new List<ObjectData>(20);
            }
            obj.bhDatas.Add(new ObjectData());
        }
        GUI.color = m_orgColor;
    }

    // UI: save testobject to Prefab
    private void _SaveObjectToPrefab()
    {
        GUI.color = Color.yellow;
        if (GUILayout.Button("Save as Prefab") && m_curInstance != null)
        {
            string properPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "prefab");
            if (!System.IO.Directory.Exists(properPath))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets", "prefab");
            }
            // save mesh renders and materials
            Renderer[] renders = m_curInstance.GetComponentsInChildren<Renderer>();
            int id = 0;
            foreach (Renderer rd in renders)
            {
                if (rd == null)
                    continue;

                Material[] mats = rd.sharedMaterials;
                foreach (Material mat in mats)
                {
                    if (mat != null && !AssetDatabase.Contains(mat))
                    {
                        StringBuilder matPath = new StringBuilder();
                        matPath.Append("Assets/prefab/");
                        matPath.Append(m_curInstance.name);
                        matPath.Append("_mat");
                        matPath.Append(id.ToString());
                        matPath.Append(".mat");
                        AssetDatabase.CreateAsset(mat, matPath.ToString());
                    }
                    ++id;
                }
            }

            // save static mesh
            MeshFilter[] mfs = m_curInstance.GetComponentsInChildren<MeshFilter>();
            int meshid = 0;
            foreach (MeshFilter mf in mfs)
            {
                if (mf != null && mf.sharedMesh != null && !AssetDatabase.Contains(mf.sharedMesh))
                {
                    StringBuilder meshPath = new StringBuilder();
                    meshPath.Append("Assets/prefab/");
                    meshPath.Append(m_curInstance.name);
                    meshPath.Append("_mesh");
                    meshPath.Append(meshid.ToString());
                    meshPath.Append(".asset");
                    MeshUtility.Optimize(mf.sharedMesh);
                    AssetDatabase.CreateAsset(mf.sharedMesh, meshPath.ToString());
                }
                ++meshid;
            }
            //save prefabs
            StringBuilder sb = new StringBuilder();
            sb.Append("Assets/prefab/");
            sb.Append(m_curInstance.name);
            sb.Append(".prefab");
            GameObject prefab = PrefabUtility.CreatePrefab(sb.ToString(), m_curInstance, ReplacePrefabOptions.ReplaceNameBased);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUI.color = m_orgColor;

    }

    // update shape field
    private bool _ShowField( object obj, FieldInfo pInfo)
    {
        var oldvalue = pInfo.GetValue(obj);
        bool ret = false;
        if (pInfo.FieldType == typeof(System.Single))
        {
            var newOne = EditorGUILayout.Slider(pInfo.Name, (float)oldvalue, -100, 500);
            if (newOne != (float)oldvalue)
            {
                pInfo.SetValue(obj, newOne);
                m_dirty = true;
                ret = true;
            }
        }
        else if (pInfo.FieldType == typeof(UnityEngine.Vector3))
        {
            var newOne = EditorGUILayout.Vector3Field(pInfo.Name, (Vector3)oldvalue);
            if (newOne != (Vector3)oldvalue)
            {
                pInfo.SetValue(obj, newOne);
                m_dirty = true;
                ret = true;
            }
        }
        else if (pInfo.FieldType == (typeof(UnityEngine.Object)))
        {
            UnityEngine.Object newOne = EditorGUILayout.ObjectField(pInfo.Name, (UnityEngine.Object)oldvalue, pInfo.FieldType, false);
            if (newOne != (UnityEngine.Object)oldvalue)
            {
                pInfo.SetValue(obj, newOne);
                m_dirty = true;
                ret = true;
            }
        }
        EditorGUILayout.Space();
        return ret;
    }


    // Instantiate TestObject in EditorMode
    private void _Instantiate(TestObject obj)
    {
        ClearTestObjectEntity();

        if ( obj.shape == null )
            return;

        m_curInstance = new GameObject(obj.name);
        GameObject shape_go = obj.shape.Generate();
        if (shape_go != null)
            shape_go.transform.parent = m_curInstance.transform;

        // update texture
        if ( obj.texture != null )
        {
            MeshRenderer[] meshs = m_curInstance.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer m in meshs)
            {
                var tempMat = new Material(m.sharedMaterial);
                tempMat.mainTexture = obj.texture;
                m.sharedMaterial = tempMat;
            }
        }

        // behaviors
        if (obj.bhDatas != null)
        {
            for (int i = 0; i < obj.bhDatas.Count; ++i)
            {
                string typeinfo = obj.bhDatas[i].typeInfo;
                if (typeinfo != null)
                {
                    Type type = Utilities.GetType(typeinfo);
                    if (type != null)
                    {
                        MonoBehaviour com = m_curInstance.AddComponent(type) as MonoBehaviour;
                        if (com != null)
                        {
                            Utilities.LoadObjectFields(obj.bhDatas[i].propertys, com);

                        }
                    }
                }
            }
        }
    }

    // destroy window
    private void OnDestroy()
    {
        ClearTestObjectEntity();
    }

    // delete instance GameObject
    private void ClearTestObjectEntity()
    {
        if ( m_curInstance != null )
        {
            GameObject.DestroyImmediate(m_curInstance);
            m_curInstance = null;
        }
    }


}
