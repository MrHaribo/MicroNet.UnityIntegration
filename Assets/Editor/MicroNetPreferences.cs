using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MicroNetPreferences : MonoBehaviour
{
    private static bool prefsLoaded = false;

    public static string sharedDirLocation;

    [PreferenceItem("MicroNet")]
    public static void PreferencesGUI()
    {
        // Load the preferences
        if (!prefsLoaded)
        {
            sharedDirLocation = EditorPrefs.GetString("SharedDirLocation", null);
            prefsLoaded = true;
        }



        // Preferences GUI
        sharedDirLocation = EditorGUILayout.TextField("SharedDir Location", sharedDirLocation);

        if (GUILayout.Button("Select SharedDir Location"))
        {
            sharedDirLocation = EditorUtility.OpenFolderPanel("Select SharedDir Location", "", null);
            EditorPrefs.SetString("SharedDirLocation", sharedDirLocation);
        }

        // Save the preferences
        if (GUI.changed)
            EditorPrefs.SetString("SharedDirLocation", sharedDirLocation);
    }
}
