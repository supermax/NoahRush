using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportProject : ScriptableObject {
	
	[MenuItem ("Custom/Export Project")]
	static void DoApply()
	{
		string[] projectContent = AssetDatabase.GetAllAssetPaths();
		AssetDatabase.ExportPackage(projectContent, "UltimateTemplate.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets );
		Debug.Log("Project Exported");
	}
}
