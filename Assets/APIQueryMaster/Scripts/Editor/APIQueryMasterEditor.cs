using APIQueryMaster.Structure;
using UnityEditor;
using UnityEngine;

namespace APIQueryMaster.AQMEditor
{
    public class APIQueryMasterEditor : Editor
    {
        [MenuItem("APIQueryMaster/APISettings")]
        static void SelectAPISettings()
        {
            Selection.activeObject = Resources.Load<APISettings>("APISettings");
        }
        
        [MenuItem("APIQueryMaster/QueryName")]
        static void SelectQueryName()
        {
            string[] guids = AssetDatabase.FindAssets("QueryName t:Script");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogWarning("QueryName.cs not found.");
            }
        }
    }
}