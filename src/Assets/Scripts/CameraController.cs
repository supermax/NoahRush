// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class MYCLASSNAME : MonoBehaviour {


/*
*	FUNCTION:
*	This script controls the camera movements based on the Player's movements.
*
*	USED BY: This script is part of the "Main Camera" prefab.
*
*/

private Transform tPlayer;	//player transform
private Transform tCamera;	//Main Camera transform
private Transform tPlayerMesh;	//player's mesh

//script references
private InGameScript hInGameScript;
private ControllerScript hControllerScript;

//defines the distance between the player and the camera
//set in the CameraMain() function
private float fCameraLerpValue;

//camera position variables
//all these variables are changed continuously during runtime
//to set the camera position
private float fCameraDistance = 30;//distance between player and camera
private Vector3 v3CamDirection;	//camera direction
private float fCurrentCamDir = 90.0f;	//camera rotation based on player's rotation
private float fCameraRotationX = 0.0f;	//camera x rotation
private float fCameraRotationZ = 0.0f;	//camera z rotation
private float fCameraPositionY = 35;	//camera Y position
private float fCameraPositionX = -10;	//camera X position

private int iCameraState = 0;	//camera state
private float fCamShakeImpulse = 0.0f;	//Camera Shake Impulse

void  Start (){
	tCamera = GetComponent<Camera>().main.transform;
	tPlayerMesh = GameObject.Find("PlayerRotation/PlayerMesh").transform;
	tPlayer = GameObject.Find("Player").transform;
	
	hInGameScript = GameObject.Find("Player").GetComponent<InGameScript>() as InGameScript;
	hControllerScript = GameObject.Find("Player").GetComponent<ControllerScript>() as ControllerScript;
			
	fCameraRotationX = tCamera.localEulerAngles.x;
	fCameraRotationZ = tCamera.localEulerAngles.z;
	
	iCameraState = 0;	
	fCamShakeImpulse = 0.0f;
}

/*
*	FUNCTION: Start following the player
*	CALLED BY: InGameScript.launchGame()
*/
public void  launchGame (){	
	iCameraState = 1;
}

void  Update (){
	if(hInGameScript.isGamePaused()==true)		
		return;
	
	if (hInGameScript.isEnergyZero())	//switch to death camera state on depletion of energy
		iCameraState = 2;
}

void  FixedUpdate (){
	CameraMain();//camera transitions
}

/*
*	FUNCTION: Controls camera movements
*	CALLED BY: FixedUpdate()
*/
private void  CameraMain (){
	fCameraDistance = Mathf.Lerp(fCameraDistance,fCameraLerpValue,Time.deltaTime*1.5f);
	fCurrentCamDir = Mathf.Lerp(fCurrentCamDir,-hControllerScript.getCurrentPlayerRotation()+90.0f,Time.deltaTime*4.0f);
	tCamera.localEulerAngles = Vector3(fCameraRotationX, fCurrentCamDir, fCameraRotationZ);
	v3CamDirection = rotateAlongY(Vector3(-1,0,0),-hControllerScript.getCurrentPlayerRotation());
		
	if (iCameraState == 1)	//regular gameplay
	{
		fCameraLerpValue = 35;//maintain a static distance between camera and the player
		tCamera.position.x = tPlayerMesh.position.x + v3CamDirection.x*fCameraDistance + fCameraPositionX;
		tCamera.position.z = Mathf.Lerp(tCamera.position.z, (tPlayerMesh.position.z + v3CamDirection.z*fCameraDistance), Time.deltaTime*50);		
		tCamera.position.y = Mathf.Lerp(tCamera.position.y, tPlayerMesh.position.y + fCameraPositionY, Time.deltaTime*70);		
	}	
	else if(iCameraState == 2)	//Camera on death 
	{	
		fCameraLerpValue = 60;//increase the distance between the camera and the player
		tCamera.position = tPlayerMesh.position + v3CamDirection*fCameraDistance;
		tCamera.position.y+= 30.0f;//increase the height of the camera
		
		tCamera.localEulerAngles.x = Mathf.Lerp(tCamera.localEulerAngles.x, 40, Time.deltaTime*25);//change the camera angle (look at the death scene)
	}
	
	//make the camera shake if the fCamShakeImpulse is not zero
	if(fCamShakeImpulse>0.0f)
		shakeCamera();
}

/*
*	FUNCTION: Calculate camera rotation vector based on player movement
*	CALLED BY: CameraMain()
*	PARAMETER 1: Camera rotation vector
*	PARAMETER 2: Player rotation value
*/
private Vector3 rotateAlongY ( Vector3 inputVector ,   float angletoRotate  ){
	Vector3 FinalVector = Vector3.zero;
	angletoRotate = angletoRotate/57.3f;
	FinalVector.x = Mathf.Cos(-angletoRotate) * inputVector.x - Mathf.Sin(-angletoRotate) * inputVector.z;
	FinalVector.z = Mathf.Sin(-angletoRotate) * inputVector.x + Mathf.Cos(-angletoRotate) * inputVector.z;
	
	return FinalVector;
}

/*
*	FUNCTION: Set the intensity of camera vibration
*	PARAMETER 1: Intensity value of the vibration
*/
public void  setCameraShakeImpulseValue ( int iShakeValue  ){
	if(iShakeValue==1)
		fCamShakeImpulse = 1.0f;
	else if(iShakeValue==2)
		fCamShakeImpulse = 2.0f;
	else if(iShakeValue==3)
		fCamShakeImpulse = 1.3f;
	else if(iShakeValue==4)
		fCamShakeImpulse = 1.5f;
	else if(iShakeValue==5)
		fCamShakeImpulse = 1.3f;
}

/*
*	FUNCTION: Make the camera vibrate. Used for visual effects
*/
private void  shakeCamera (){
	tCamera.position.y+=Random.Range(-fCamShakeImpulse,fCamShakeImpulse);
	tCamera.position.z+=Random.Range(-fCamShakeImpulse,fCamShakeImpulse);
	fCamShakeImpulse-=Time.deltaTime * fCamShakeImpulse*4.0f;
	if(fCamShakeImpulse<0.01f)
		fCamShakeImpulse = 0.0f;
}

}