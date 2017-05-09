using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
 
public class TransformCopier : ScriptableObject
{
        private static Vector3 position;
        private static Quaternion rotation;
        private static Vector3 scale;
        //private static string myName;
    
    [MenuItem ("Custom/Transform Copier/Copy Transform &c")]
    static void DoRecord()
    {
       position = Selection.activeTransform.localPosition;
       rotation = Selection.activeTransform.localRotation;
       scale = Selection.activeTransform.localScale;
      // myName = Selection.activeTransform.name;       
        
        //EditorUtility.DisplayDialog("Transform Copy", "Local position, rotation, & scale of "+myName +" copied relative to parent.", "OK", "");
    }
 
    [MenuItem ("Custom/Transform Copier/Paste Transform &v")]
    static void DoApply()
    {
        Selection.activeTransform.localPosition = position;
        Selection.activeTransform.localRotation = rotation;
        Selection.activeTransform.localScale = scale;      
        
        //EditorUtility.DisplayDialog("Transform Paste", "Local position, rotation, and scale of "+myName +"  pasted relative to parent of "+Selection.activeTransform.name+".", "OK", "");
    }
    
    [MenuItem ("Custom/Take Screenshot &a")]
    static void TakeScreenshot()
    {
		string screenshotFilename;
		int screenshotCount = 1;
		
		int i = 0;
		while(true)
		{
			screenshotFilename = "Screenshot_" + screenshotCount + ".png";
			if(System.IO.File.Exists(screenshotFilename))
				screenshotCount++;
			else
				break;
			i++;
			if(i>=999)
			{
				Debug.LogWarning("Empty your folder. Screenshots limit reached");
				return;
			}
		}
		
		Application.CaptureScreenshot(screenshotFilename);
		Debug.Log("Screenshot saved as : "+screenshotFilename);
    	//Application.CaptureScreenshot("Screenshot.png");
    }
    
     [MenuItem ("Custom/Prune Small Valeus &d")]
    static void PruneSmallValues()
    {
        //Selection.activeTransform.localPosition = position;
        //Selection.activeTransform.localRotation = rotation;
        //Selection.activeTransform.localScale = scale;      
        
        float wrong_Val = 0.0f;
        float right_Val_X = 0.0f;
        float right_Val_Y = 0.0f;
        float right_Val_Z = 0.0f;
        
       	wrong_Val = Selection.activeTransform.localScale.x;
        right_Val_X = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localScale.y;
        right_Val_Y = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localScale.z;
        right_Val_Z = Mathf.Round(wrong_Val*100)/100;
        Selection.activeTransform.localScale = new Vector3(right_Val_X, right_Val_Y, right_Val_Z);
        
        wrong_Val = Selection.activeTransform.localPosition.x;
        right_Val_X = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localPosition.y;
        right_Val_Y = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localPosition.z;
        right_Val_Z = Mathf.Round(wrong_Val*100)/100;
        Selection.activeTransform.localPosition = new Vector3(right_Val_X, right_Val_Y, right_Val_Z);
        
        wrong_Val = Selection.activeTransform.localEulerAngles.x;
        right_Val_X = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localEulerAngles.y;
        right_Val_Y = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localEulerAngles.z;
        right_Val_Z = Mathf.Round(wrong_Val*100)/100;
        Selection.activeTransform.localEulerAngles = new Vector3(right_Val_X, right_Val_Y, right_Val_Z);
    }
    
    
}