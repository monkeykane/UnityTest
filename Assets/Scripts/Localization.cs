using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Localization : ScriptableObject
{
    public List<string>              Langs = new List<string>(2);

    private SimpleJSON.JSONNode     _languageNode;

    public System.Action            OnChangeLocalization;

    private static Localization instance;
    public static Localization Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<Localization>("Localization");

                if (instance == null)
                {
                    instance = ScriptableObject.CreateInstance<Localization>();

#if UNITY_EDITOR
                    // Save into Resource folder
                    string properPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
                    if (!System.IO.Directory.Exists(properPath))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    string fullPath = System.IO.Path.Combine(
                      System.IO.Path.Combine("Assets", "Resources"),
                      "Localization.asset"
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

    public string GetText(string id)
    {
        return _languageNode[id];
    }

    public  void Active()
    {
       Language = "ENG";
    }

    private string _language;
    public string Language
    {
        get
        {
            return _language;
        }
        set
        {
            _language = value;

            string path = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("Localization/");
            sb.Append(_language);
            path = sb.ToString();
            TextAsset asset = Resources.Load<TextAsset>(path);
            if ( asset == null )
                return;
            string data = asset.text;
            _languageNode = SimpleJSON.JSON.Parse(asset.text);
            if (OnChangeLocalization != null)
                OnChangeLocalization.Invoke();

        }
    }
}
