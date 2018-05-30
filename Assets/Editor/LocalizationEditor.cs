using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalizationEditor :  Editor
{
    [MenuItem("Test/Localization")]
    static void OpenObjectEditor()
    {
        var obj = Localization.Instance;
    }
}
