// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class MYCLASSNAME : MonoBehaviour {

/*
*	FUNCTION:
*	- This script detects inputs (swipes and gyro) and controls the 
*	player accordingly.	
*	- It also defines the physics that defines
*	the user's actions.	
*	- It is responsible for handling all player animations.
*	- It is responsible for process all collisions.
*	- It is responsible for initiating the death scene.
*	
*	USED BY: This script is a part of the "Player" prefab.
*
*/

enum StrafeDirection {left = 0, right = 1}

private Transform tPlayer;	//the main character transform
private Transform tPlayerRotation;	//Player child transform to rotate it in game
private Animation aPlayer;				//character animation

private Transform tPlayerSidesCollider;	//sides collider transform (detects stumble)
private Transform tFrontCollider;			//front collider transfrom (detects collisions)
private Vector3 v3BNCDefaultScale;
private Vector3 v3BFCDefaultScale;

//Variables
private float fCurrentWalkSpeed;

private float tCurrentAngle = 0.0f;	//current rotation along Y axis
private float fJumpForwardFactor = 0.0f;	//movement speed increase on jump
private float fCurrentUpwardVelocity = 0.0f;	//speed during the duration of jump
private float fCurrentHeight = 0.0f;
private float fContactPointY = 0.0f;	//y-axis location of the path

//player state during gameplay
private bool  bInAir = false;
private bool  bJumpFlag = false;
private bool  bInJump = false;
private bool  bInDuck = false;				//true if the character is sliding
private bool  bDiveFlag = false;			//force character to dive during jump
private bool  bExecuteLand = false;
private bool  bInStrafe = false;

private float fForwardAccleration = 0.0f;
private Transform tBlobShadowPlane;	//the shadow under the player
private Vector3 CurrentDirection;//set player rotation according to path

//script references
private PatchesRandomizer hPatchesRandomizer;
private CheckPointsMain hCheckPointsMain;
private InGameScript hInGameScript;
private PitsMainController hPitsMainController;
private SoundManager hSoundManager;
private CameraController hCameraController;
private PowerupsMainController hPowerupScript;
private EnemyController hEnemyController;
private MenuScript hMenuScript;
private PlayerFrontColliderScript hPlayerFrontColliderScript;
private PlayerSidesColliderScript hPlayerSidesColliderScript;

private RaycastHit hitInfo;	//whats under the player character
private bool  bGroundhit = false;	//is that an object under the player character
private float fHorizontalDistance = 0.0f;	//calculate player's horizontal distance on path

private float fCurrentForwardSpeed = .5;	//sets movement based on spline
private float fCurrentDistance = 0.0f;//distance between the start and current position during the run
private float fCurrentMileage = 0.0f;//used to calculate the score based on distance covered

//detect if there is a terrain_lyr under the player
private float fPitFallLerpValue = 0.0f;
private float fPitFallForwardSpeed = 0.0f;
private float fPitPositionX = 0.0f;	//check the position of the pit in x-axis
private int iDeathAnimStartTime = 0;
private int iDeathAnimEndTime = 3;	//duration wait for death scene

private bool  JumpAnimationFirstTime = true;	//play death animation once
private Camera HUDCamera;

private Transform tPauseButton;
private Transform tHUDGroup;
private SwipeControls swipeLogic;
private int iLanePosition;						//current lane number -- -1, 0 or 1
private int iLastLanePosition; 					//stores the previous lane on lane change
private bool  bMouseReleased = true;
private bool  bControlsEnabled = true;

//action queue
private SwipeDirection directionQueue;
private bool  bDirectionQueueFlag = false;

//Physics Constants
//change these to adjust the initial and final movement speed
private float fStartingWalkSpeed = 150.0f;//when player starts running
private float fEndingWalkSpeed = 230.0f;	//final speed after acclerating
private float fCurrentWalkAccleration = 0.5f;	//rate of accleartion

//change these to adjust the jump height and displacement
private float fJumpPush = 185;			//force with which player pushes the ground on jump
private void  getAccleration (){ return 500; }	//accleration and deceleration on jump

//the initial distance of the player character at launch
//from the start of the path
private float fCurrentDistanceOnPath = 0.0f;

//Is used to switch between gyro and swipe controls
private bool  swipeControlsEnabled = true;
public void  isSwipeControlEnabled (){ return swipeControlsEnabled; }

public void  toggleSwipeControls ( bool state  ){
	 swipeControlsEnabled = state;
	
	//permanently save user preference of controls
	PlayerPrefs.SetInt("ControlsType", (state == true ? 1 : 0));
	PlayerPrefs.Save();
}

void  Start (){
	hMenuScript = GameObject.Find("MenuGroup").GetComponent<MenuScript>() as MenuScript;
	hPatchesRandomizer = this.GetComponent<PatchesRandomizer>() as PatchesRandomizer;
	hPlayerSidesColliderScript = GameObject.Find("PlayerSidesCollider").GetComponent<PlayerSidesColliderScript>() as PlayerSidesColliderScript;
	hPlayerFrontColliderScript = GameObject.Find("PlayerFrontCollider").GetComponent<PlayerFrontColliderScript>() as PlayerFrontColliderScript;
	hSoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>() as SoundManager;
	hInGameScript = this.GetComponent<InGameScript>() as InGameScript;	
	hPitsMainController = this.GetComponent<PitsMainController>() as PitsMainController;
	hCheckPointsMain = this.GetComponent<CheckPointsMain>() as CheckPointsMain;
	hPowerupScript = this.GetComponent<PowerupsMainController>() as PowerupsMainController;
	hEnemyController = GameObject.Find("Enemy").GetComponent<EnemyController>() as EnemyController;
	hPowerupScript = this.GetComponent<PowerupsMainController>() as PowerupsMainController;
	hCameraController = GetComponent.<Camera>().main.gameObject.GetComponent<CameraController>() as CameraController;
	swipeLogic = transform.GetComponent<SwipeControls>() as SwipeControls;
	
	tPlayer = transform;
	tPlayerRotation = transform.Find("PlayerRotation");
	
	//get the animation component of the player character
	aPlayer = this.transform.Find("PlayerRotation/PlayerMesh/Noa_GEO").GetComponent<Animation>() as Animation;
	tBlobShadowPlane = transform.Find("BlobShadowPlane");
	
	tPlayerSidesCollider = GameObject.Find("PlayerSidesCollider").transform;	
	tFrontCollider = GameObject.Find("PlayerFrontCollider").transform;
	tHUDGroup = GameObject.Find("HUDMainGroup/HUDGroup").transform;
	tPauseButton = GameObject.Find("HUDMainGroup/HUDGroup/HUDPause").transform;
	
	HUDCamera = GameObject.Find("HUDCamera").GetComponent.<Camera>();
	
	v3BNCDefaultScale = tFrontCollider.localScale;	
	v3BFCDefaultScale = tPlayerSidesCollider.localScale;
	
	bInAir = false;
	fCurrentDistanceOnPath = 50.0f;	//inital distance with respect to spline
	fCurrentDistance = 0.0f;
	fCurrentMileage = 0.0f;
	tCurrentAngle = 0.0f;	
	fPitFallLerpValue = 0.0f;
	fPitFallForwardSpeed = 0.0f;
	fPitPositionX = 0.0f;
	iDeathAnimStartTime = 0;
	bGroundhit = false;	
	bJumpFlag = false;
	bInJump = false;
	fCurrentUpwardVelocity = 0;
	fCurrentHeight = 0;
	
	bDirectionQueueFlag = false;
	directionQueue = SwipeDirection.Null;
	iLanePosition = 0;	//set current lane to mid	
	fCurrentWalkSpeed = fStartingWalkSpeed;
	
	
	if (PlayerPrefs.HasKey("ControlsType"))
		swipeControlsEnabled = PlayerPrefs.GetInt("ControlsType") == 1 ? true : false;
	else
		PlayerPrefs.SetInt("ControlsType", (swipeControlsEnabled == true ? 1 : 0));
		
	hSoundManager.stopSound(CharacterSounds.Footsteps);
	StartCoroutine("playIdleAnimations");//start playing idle animations
}//end of Start()

/*
 * FUNCTION:	Play and alternate between the two idle animations
 * 				when the game is launched/ restarted.
 * CALLED BY:	Start()
 * */
private void  playIdleAnimations (){
	while(true)
	{
		yield return new WaitForFixedUpdate();
		
		if (!aPlayer.IsPlaying("Idle_1") && !aPlayer.IsPlaying("Idle_2"))
		{
			aPlayer.GetComponent.<Animation>().Play("Idle_1");
			aPlayer.PlayQueued("Idle_2", QueueMode.CompleteOthers);
		}
	}//end of while
}

/*
*	FUNCTION: Enable controls, start player animation and movement
*/
public void  launchGame (){
	StopCoroutine("playIdleAnimations");//stop idle animations
	hEnemyController.launchEnemy();
		
	aPlayer["run"].speed = Mathf.Clamp( (fCurrentWalkSpeed/fStartingWalkSpeed)/1.1f, 0.8f, 1.2f );
	aPlayer.Play("run");
	
	hSoundManager.playSound(CharacterSounds.Footsteps);//play the footsteps sound
}

void  Update (){
	if(hInGameScript.isGamePaused()==true)
		return;
	
	if (hInGameScript.isEnergyZero())
		if(DeathScene())
			return;
	
	getClicks();	//get taps/clicks for pause menu etc.
	
	if (bControlsEnabled)
		SwipeMovementControl();
}//end of update()

void  FixedUpdate (){
	if(hInGameScript.isGamePaused() == true)
		return;
		
	setForwardSpeed();
	SetTransform();
	setShadow();
		
	if(!bInAir)
	{
		if(bExecuteLand)
		{
			hSoundManager.playSound(CharacterSounds.JumpLand);
			bExecuteLand = false;
			JumpAnimationFirstTime = true;
		}
	}//end of if not in air
	else
	{		
		if(JumpAnimationFirstTime&&bInJump==true)
		{
			aPlayer.Rewind("jump");
			JumpAnimationFirstTime = false;
			bInDuck = false;
						
			aPlayer.CrossFade("jump", 0.1f);
		}
	}//end of else !in air

	if(bJumpFlag==true)
	{		
		bJumpFlag = false;
		bExecuteLand = true;
		bInJump = true;
		bInAir = true;
		fCurrentUpwardVelocity = fJumpPush;
		fCurrentHeight = tPlayer.position.y;
	}
		
	//acclerate movement speed with time
	if(fCurrentWalkSpeed<fEndingWalkSpeed)
		fCurrentWalkSpeed += (fCurrentWalkAccleration * Time.fixedDeltaTime);
		
	aPlayer["run"].speed = Mathf.Clamp( (fCurrentWalkSpeed/fStartingWalkSpeed)/1.1f, 0.8f, 1.2f );	//set run animation speed according to current speed
}//end of Fixed Update

/*
*	FUNCTION: Check if pause button is tapped in-game
*	CALLED BY:	Update()
*/
private void  getClicks (){
	if(Input.GetMouseButtonUp(0) && bMouseReleased==true)
	{
		Vector3 screenPoint;
		Vector2 buttonSize;
		Rect Orb_Rect;
		
		if (tHUDGroup.localPosition.z==0)
		{
			buttonSize = Vector3(Screen.width/6,Screen.width/6,0.0f);
			screenPoint = HUDCamera.WorldToScreenPoint( tPauseButton.position );
			
			Orb_Rect = new Rect(screenPoint.x - ( buttonSize.x * 0.5f ), screenPoint.y - ( buttonSize.y * 0.5f ), buttonSize.x, buttonSize.y);
			if(Orb_Rect.Contains(Input.mousePosition))
			{				
				hInGameScript.pauseGame();
			}
		}
		
		Orb_Rect = new Rect(screenPoint.x - ( buttonSize.x * 0.5f ), screenPoint.y - ( buttonSize.y * 0.5f ), buttonSize.x, buttonSize.y);
	}//end of mouserelease == true if
		
}//end of get clicks function

/*
*	FUNCITON: Set the position of the shadow under the player and of the
*				colliders to make them move with the character mesh.
*	CALLED BY:	FixedUpdate()
*/
private void  setShadow (){	
	tBlobShadowPlane.up = hitInfo.normal;
	tBlobShadowPlane.position.y = fContactPointY+0.7f;//set shadow's position
	tBlobShadowPlane.localEulerAngles.y = tPlayerRotation.localEulerAngles.y;//set shadow's rotation
	
	//set side collider's position and rotation
	tPlayerSidesCollider.position = tPlayer.position + Vector3(0,5,0);
	tPlayerSidesCollider.localEulerAngles = tBlobShadowPlane.localEulerAngles;//set 
	
	//set front collider's position and rotation
	tFrontCollider.position = tPlayer.position + Vector3(7,5,0);
	tFrontCollider.localEulerAngles = tBlobShadowPlane.localEulerAngles;
}

/*
*	FUNCTION: Set the player's position the path with reference to the spline
*	CALLED BY:	FixedUpdate()
*/
void  SetTransform (){
	if (bControlsEnabled)
		int iStrafeDirection = getLeftRightInput();	//get the current lane (-1, 0 or 1)
	
	fCurrentDistanceOnPath = hCheckPointsMain.SetNextMidPointandRotation(fCurrentDistanceOnPath, fCurrentForwardSpeed);//distance on current patch
	fCurrentDistance = fCurrentDistanceOnPath + hPatchesRandomizer.getCoveredDistance();//total distance since the begining of the run
	fCurrentMileage = fCurrentDistance/12.0f;//calculate milage to display score on HUD
	
	tCurrentAngle = hCheckPointsMain.getCurrentAngle();//get the angle according to the position on path
	tPlayerRotation.localEulerAngles.y = -tCurrentAngle;//set player rotation according to the current player position on the path's curve (if any)
	
	CurrentDirection = hCheckPointsMain.getCurrentDirection();
	Vector3 Desired_Horinzontal_Pos = calculateHorizontalPosition(iStrafeDirection);
	
	bGroundhit = Physics.Linecast(Desired_Horinzontal_Pos + Vector3(0,20,0),Desired_Horinzontal_Pos + Vector3(0,-100,0),hitInfo,(1<<9));	
	
	if(bGroundhit && hPitsMainController.isFallingInPit()==false)//calculate player position in y-axis
		fContactPointY = hitInfo.point.y;
	else//call death if player in not on Terrain_lyr
	{
		fContactPointY = -10000.0f;
		if(!bInAir)
		{
			if(!bInJump)
			{
				if(reConfirmPitFalling(Desired_Horinzontal_Pos,iStrafeDirection)==true)
				{
					hPitsMainController.setPitValues();
				}
			}
			bInAir = true;
			fCurrentUpwardVelocity = 0;
			fCurrentHeight = tPlayer.position.y;
		}
	}
	
	if(!bInAir)//set player position when not in air
	{
		tPlayer.position.y = fContactPointY+0.6f;
	}
	else//set player position if in air
	{
		if (bDiveFlag)	//dive during jump
		{
			setCurrentDiveHeight();
			tPlayer.position.y = fCurrentHeight;
		}
		else			//JUMP
		{
			setCurrentJumpHeight();
			tPlayer.position.y = fCurrentHeight;
		}
	}
	
	tPlayer.position.x = Desired_Horinzontal_Pos.x;//set player position in x-axis
	tPlayer.position.z = Desired_Horinzontal_Pos.z;//set player position in y-axis
	
}//end of Set Transform()

/*
*	FUNCTION: Set the height of the player during jump
*	CALLED BY:	SetTransform()
*/
private function setCurrentJumpHeight()		//set height during jump
{
	fCurrentUpwardVelocity-=Time.fixedDeltaTime*getAccleration();
	fCurrentUpwardVelocity = Mathf.Clamp(fCurrentUpwardVelocity,-fJumpPush,fJumpPush);
	fCurrentHeight+=fCurrentUpwardVelocity*(Time.fixedDeltaTime/1.4f);
	
	if(fCurrentHeight<fContactPointY)
	{
		fCurrentHeight = fContactPointY;
		bInAir = false;
		bInJump = false;
		
		if (bDiveFlag)	//do not resume run animation on Dive
			return;
				
		if (!hInGameScript.isEnergyZero())
		{		
			aPlayer.CrossFade("run", 0.1f);
		}//end of if current energy > 0
	}
}

/*
*	FUNCITON: Pull the player down faster if user swipes down int the middle of jump
*	CALLED BY:	SetTransform()
*/
private function setCurrentDiveHeight()	//set height after dive called
{
	fCurrentUpwardVelocity-=Time.fixedDeltaTime*2000;
	fCurrentUpwardVelocity = Mathf.Clamp(fCurrentUpwardVelocity,-fJumpPush,fJumpPush);
	if(hPitsMainController.isFallingInPit() == false)
		fCurrentHeight+=fCurrentUpwardVelocity*Time.fixedDeltaTime;
	else
	{
		fCurrentHeight-=40.0f*Time.fixedDeltaTime;
		hMenuScript.hideHUDElements();
	}	
	
	if(fCurrentHeight<=fContactPointY)
	{
		fCurrentHeight = fContactPointY;//bring character down completely
			
		bInAir = false;
		bInJump = false;
		
		duckPlayer();//make the character slide
		bDiveFlag = false;		//dive complete
	}//end of if
}

/*
*	FUNCTION: 	Make sure that there is no terrain under the player
*				before making it fall
*	CALLED BY:	SetTransform()
*/
private bool reConfirmPitFalling ( Vector3 Desired_Horinzontal_Pos ,   float iStrafeDirection  ){
	 bool  bGroundhit = false;
	
	if(iStrafeDirection>=0)
		bGroundhit = Physics.Linecast(Desired_Horinzontal_Pos + Vector3(1,20,5),Desired_Horinzontal_Pos + Vector3(0,-100,5),hitInfo,1<<9);
	else
		bGroundhit = Physics.Linecast(Desired_Horinzontal_Pos + Vector3(1,20,-5),Desired_Horinzontal_Pos + Vector3(0,-100,-5),hitInfo,1<<9);
	
	if(!bGroundhit)
		return true;
	else
		return false;
}

/*
*	FUNCTION: Called when user runs out of energy
*	CALLED BY:	Update()
*/
bool DeathScene (){
	 bInAir = false;
	tPlayerRotation.localEulerAngles = Vector3(0,0,0);
	
	if (iDeathAnimStartTime == 0)
	{
		hSoundManager.stopSound(CharacterSounds.Footsteps);
		bControlsEnabled = false;
				
		FIXME_VAR_TYPE v3EffectPosition= this.transform.position;
		v3EffectPosition.x += 15;
		v3EffectPosition.y += 5;		
	
		aPlayer.CrossFade("death",0.1f);
		hEnemyController.playDeathAnimation();
		
		hMenuScript.hideHUDElements();
		iDeathAnimStartTime = Time.time;	
	}	
	else if (iDeathAnimStartTime != 0 && (Time.time - iDeathAnimStartTime) >= iDeathAnimEndTime)
	{		
		hInGameScript.setupDeathMenu();
		return true;
	}
	
	return false;
}

/*
*	FUNCTION: Called when player hits an obstacle sideways
*	CALLED BY: PlayerSidesColliderScript.OnCollisionEnter()
*/
public void  processStumble (){
	hCameraController.setCameraShakeImpulseValue(1);
	iLanePosition = iLastLanePosition;	//stop strafe
		
	if (hEnemyController.processStumble())
	{	
		hInGameScript.collidedWithObstacle();//call death if player stumbled twice in unit time
	}
	else
	{
		aPlayer.PlayQueued("run", QueueMode.CompleteOthers);
		
		//enable colliders if they were disabled
		hPlayerFrontColliderScript.activateFrontCollider();
		hPlayerSidesColliderScript.activateSidesCollider();
	}	
}

/*
*	FUNCTION: Returns horizontal the position to move to
*	CALLED BY: SetTransform()
*/
private function getLeftRightInput()	//change lane
{
	if (swipeControlsEnabled == true)//swipe direction
		return iLanePosition;
	else//gyro direction
	{
		float fMovement = 0.0f;
		float fSign = 1.0f;
		
		if(Screen.orientation == ScreenOrientation.Portrait)
			fSign = 1.0f;
		else
			fSign = -1.0f;
		
		if(Application.isEditor)//map gyro controls on mouse in editor mode
		{
			fMovement = (Input.mousePosition.x - (Screen.height/2.0f))/(Screen.height/2.0f) * 4.5f;
		}
		else
		{
			fMovement = (fSign * Input.acceleration.x * 4.5f);
		}
		
		return fMovement;
	}
}

/*
*	FUNCTION: Set the movement speed
*	CALLED BY: FixedUpdate()
*/
private void  setForwardSpeed (){
	//if the player is not on Terrain_lyr
	if(hPitsMainController.isFallingInPit() == true)
	{		
		if(transform.position.x>fPitPositionX)
			fCurrentForwardSpeed = 0.0f;
		else
			fCurrentForwardSpeed = Mathf.Lerp(fPitFallForwardSpeed,0.01f,(Time.time-fPitFallLerpValue)*3.5f);
		return;
	}
	
	if (hInGameScript.isEnergyZero())//on death
	{
		fCurrentForwardSpeed = 0;
		return;
	}
	
	if(bInAir)
		fForwardAccleration = 1.0f;
	else
		fForwardAccleration = 2.0f;
		
	fJumpForwardFactor = 1 + ((1/fCurrentWalkSpeed)*50);
		
	if(bInJump==true)
		fCurrentForwardSpeed = Mathf.Lerp(fCurrentForwardSpeed,fCurrentWalkSpeed*Time.fixedDeltaTime*fJumpForwardFactor,Time.fixedDeltaTime*fForwardAccleration);
	else
		fCurrentForwardSpeed = Mathf.Lerp(fCurrentForwardSpeed,(fCurrentWalkSpeed)*Time.fixedDeltaTime,Time.fixedDeltaTime*fForwardAccleration);
}

/*
*	FUNCTION: Make the player change lanes
*	CALLED BY:	SetTransform()
*/
private float fCurrentStrafePosition = 0.0f;	//keeps track of strafe position at each frame
private float fSpeedMultiplier = 5.0f;	//how fast to strafe/ change lane
private void  calculateHorizontalPosition ( int iStrafeDirection  ){
	if (swipeControlsEnabled == true)
	{
		Vector2 SideDirection_Vector2 = rotateAlongZAxis(Vector2(CurrentDirection.x,CurrentDirection.z),90.0f);
		SideDirection_Vector2.Normalize();
			
		if(iStrafeDirection==-1)//strafe left from center
		{
			if(fCurrentStrafePosition>-1)
			{
				fCurrentStrafePosition-= fSpeedMultiplier*Time.fixedDeltaTime;
				if(fCurrentStrafePosition<=-1.0f)
				{
					fCurrentStrafePosition = -1.0f;
					switchStrafeToSprint();
				}
			}
		}
		else if(iStrafeDirection==1)//strafe right from center
		{
			if(fCurrentStrafePosition<1)
			{
				fCurrentStrafePosition+= fSpeedMultiplier*Time.fixedDeltaTime;
				if(fCurrentStrafePosition>=1.0f)
				{
					fCurrentStrafePosition = 1.0f;
					switchStrafeToSprint();
				}
			}
		}
		else if(iStrafeDirection==0&&fCurrentStrafePosition!=0.0f)//strafe from left or right lane to center
		{	
			if(fCurrentStrafePosition<0)
			{
				fCurrentStrafePosition+= fSpeedMultiplier*Time.fixedDeltaTime;
				if(fCurrentStrafePosition>=0.0f)
				{
					fCurrentStrafePosition = 0.0f;
					switchStrafeToSprint();
				}
			}
			else if(fCurrentStrafePosition>0)
			{
				fCurrentStrafePosition-= fSpeedMultiplier*Time.fixedDeltaTime;
				if(fCurrentStrafePosition<=0.0f)
				{
					fCurrentStrafePosition = 0.0f;
					switchStrafeToSprint();
				}
			}
		}//end of else
			
		fHorizontalDistance = -fCurrentStrafePosition*16.0f;	
		fHorizontalDistance = Mathf.Clamp(fHorizontalDistance,-20.0f,20.0f);
		
		Vector2 fHorizontalPoint = hCheckPointsMain.getCurrentMidPoint() + SideDirection_Vector2*fHorizontalDistance;
			
		return Vector3(fHorizontalPoint.x,tPlayerRotation.position.y,fHorizontalPoint.y);
	}
	else
	{
		SideDirection_Vector2 = rotateAlongZAxis(Vector2(CurrentDirection.x,CurrentDirection.z),90.0f);
		SideDirection_Vector2.Normalize();
		
		fHorizontalDistance = Mathf.Lerp(fHorizontalDistance,-iStrafeDirection * 40.0f, 0.05f*fCurrentForwardSpeed);		
		fHorizontalDistance = Mathf.Clamp(fHorizontalDistance,-20.0f,20.0f);		
		fHorizontalPoint = hCheckPointsMain.getCurrentMidPoint() + SideDirection_Vector2*fHorizontalDistance;
				
		return Vector3(fHorizontalPoint.x,tPlayerRotation.position.y,fHorizontalPoint.y);
	}//end of else
}

/*
*	FUNCTION: Determine the rotation of the player character
*/
private void  rotateAlongZAxis ( Vector2 inputVector ,   float angletoRotate  ){
	Vector2 FinalVector = Vector2.zero;
	angletoRotate = angletoRotate/57.3f;
	FinalVector.x = Mathf.Cos(angletoRotate) * inputVector.x - Mathf.Sin(angletoRotate) * inputVector.y;
	FinalVector.y = Mathf.Sin(angletoRotate) * inputVector.x + Mathf.Cos(angletoRotate) * inputVector.y;
	
	return FinalVector;
}

/*
*	FUNCTION: Play the "run" animation
*	CALLED BY:	calculateHorizontalPosition()
*/
private void switchStrafeToSprint (){
	if (!hInGameScript.isEnergyZero() && !isInAir())
	{
		aPlayer.CrossFade("run", 0.1f);
		bInStrafe = false;
	}	
}

/*
*	FUNCITON: Detect swipes on screen
*	CALLED BY: Update()
*/
void  SwipeMovementControl (){	
	//check and execute two jump or duck commands simultaneously
	if (bDirectionQueueFlag)
	{
		if(!bInAir && directionQueue == SwipeDirection.Jump)		//queue JUMP
		{
			bJumpFlag = true;			
			bDirectionQueueFlag = false;
		}//end of jump queue
		if (directionQueue == SwipeDirection.Duck && !bInDuck)		//queue SLIDE
		{
			duckPlayer();			
			bDirectionQueueFlag = false;
		}//end of duck queue
		
	}//end of direction queue

	//swipe controls
	FIXME_VAR_TYPE direction= swipeLogic.getSwipeDirection();	//get the swipe direction	
	if (direction != SwipeDirection.Null)
	{
		bMouseReleased = false;//disallow taps on swipe
		
		if (direction == SwipeDirection.Jump)	//JUMP
		{
			if(!bInAir)
			{					
				bJumpFlag = true;
			}
			if (bInAir)	//queue the second jump if player swipes up in the middle of a jump
			{
				bDirectionQueueFlag = true;
				directionQueue = SwipeDirection.Jump;
			}
		}//end of if direction is jump
		if (direction == SwipeDirection.Right && swipeControlsEnabled == true)	//RIGHT swipe
		{
			if (iLanePosition != 1) 
			{
				iLastLanePosition = iLanePosition;
				iLanePosition++;
				
				strafePlayer(StrafeDirection.right);
				
			}//end of lane check if
		}//end of swipe direction if
		if (direction == SwipeDirection.Left && swipeControlsEnabled == true)	//LEFT swipe
		{
			if (iLanePosition != -1) 
			{
				iLastLanePosition = iLanePosition;
				iLanePosition--;
				
				strafePlayer(StrafeDirection.left);
				
			}//end of lane check if
		}//end of swipe direction if
		if (direction == SwipeDirection.Duck && bInDuck)//SLIDE: queue the second duck command if player is in the middle of slide animation
		{
			bDirectionQueueFlag = true;
			directionQueue = SwipeDirection.Duck;
		}
		if (direction == SwipeDirection.Duck && !bInAir && !bInDuck)//SLIDE: on ground
		{
			duckPlayer();
		}
		if (direction == SwipeDirection.Duck && bInAir && !bInDuck)//SLIDE/ DIVE: in air
		{				
			bDiveFlag = true;	//used by Set Transform() to make the character dive
		}//end of slide in air if
		
		//swipeLogic.iTouchStateFlag = 2;
	}//end of if	
	if (Input.GetMouseButtonUp(0))	//allow taps on mouse/ tap release
	{
		bMouseReleased = true;
	}
		
	if (!isPlayingDuck() && bInDuck == true)	//restore the size of the collider after slide ends
	{
		hSoundManager.playSound(CharacterSounds.Footsteps);
		
		tPlayerRotation.localEulerAngles.z = 0;//rotation correction after DIVE
		tBlobShadowPlane.localPosition.x = 0;//translation correction after DIVE (to fix mysterious  S bug  )
	
		bInDuck = false;
		tFrontCollider.localScale = v3BNCDefaultScale;
		tPlayerSidesCollider.localScale = v3BFCDefaultScale;		//restore far collider
		
		if (bDiveFlag)	//do not resume run animation on Dive
			return;
				
		aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers);
	}
	
	//keyboard controls (DEBUG)
	if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))//Up/ jump
	{
		if(!bInAir)
		{					
			bJumpFlag = true;
		}
		if (bInAir)
		{
			bDirectionQueueFlag = true;
			directionQueue = SwipeDirection.Jump;
		}
	}
	else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))//Right
	{
		if (iLanePosition != 1) 
		{
			iLastLanePosition = iLanePosition;
			iLanePosition++;
			
			strafePlayer(StrafeDirection.right);
			
		}//end of lane check if
	}
	else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))//Left
	{
		if (iLanePosition != -1) 
		{
			iLastLanePosition = iLanePosition;
			iLanePosition--;
			
			strafePlayer(StrafeDirection.left);
			
		}//end of lane check if
	}
	else if ( (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && bInDuck)
	{
		bDirectionQueueFlag = true;
		directionQueue = SwipeDirection.Duck;
	}
	else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !bInAir && !bInDuck)
	{
		duckPlayer();
	}
	else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && bInAir && !bInDuck)
	{
		bDiveFlag = true;	//used by Set Transform() to make the character dive
	}
	
}//end of Movement Control function

/*
*	FUNCTION: make the character slide
*	CALLED BY: SwipeMovementControl()
*/
void  duckPlayer (){
	bInDuck = true;
	hSoundManager.stopSound(CharacterSounds.Footsteps);
	
	aPlayer["slide"].speed = 1.4f;
	aPlayer.CrossFade("slide", 0.1f);
	
	tFrontCollider.localScale = v3BNCDefaultScale/2;	//reduce the near collider size
	tPlayerSidesCollider.localScale = v3BFCDefaultScale/2;	//reduce the far collider size
}

/*
*	FUNCTION: Check if the user is sliding
*/
bool isPlayingDuck (){
	 if (hInGameScript.isEnergyZero())
		return false;
	
	if (aPlayer.IsPlaying("slide"))
		return true;
	else
		return false;
}

/*
*	FUNCTION: strafe charater right or left
*	INPUT: "right" OR "left"
*	OUTPUT: move the character left or right
*/
void  strafePlayer ( StrafeDirection strafeDirection  ){
	if (isInAir())
	{	
		aPlayer[strafeDirection.ToString()].speed = 2;
		aPlayer.Play(strafeDirection.ToString());		
	}
	else if (aPlayer.IsPlaying(strafeDirection.ToString()))	//if strafed while already strafing
	{
		aPlayer.Stop(strafeDirection.ToString());
		
		aPlayer[strafeDirection.ToString()].speed = 1.75f;
		aPlayer.CrossFade(strafeDirection.ToString(),0.01f);
		
		bInStrafe = true;
	}
	else
	{
		aPlayer[strafeDirection.ToString()].speed = 1.75f;
		aPlayer.CrossFade(strafeDirection.ToString(),0.01f);
		
		bInStrafe = true;
	}
}

public float getCurrentMileage (){ return fCurrentMileage; }
public float getCurrentForwardSpeed (){ return fCurrentForwardSpeed; }
public int getCurrentLane (){ return iLanePosition; }
public float getCurrentPlayerRotation (){ return tCurrentAngle; }
public float getCurrentWalkSpeed (){ return fCurrentWalkSpeed; }
public bool isInAir (){
	 if (bInAir || bJumpFlag || bInJump || bDiveFlag)
		return true;
	else
		return false;
}

public void  setCurrentDistanceOnPath ( float iValue  ){ fCurrentDistanceOnPath = iValue; }
public void  setPitFallLerpValue ( float iValue  ){ fPitFallLerpValue = iValue; }
public void  setPitFallForwardSpeed ( float iValue  ){ fPitFallForwardSpeed = iValue; }

/*
*	FUNCTION: Turn player animations On or Off
*/
public void togglePlayerAnimation ( bool bValue  ){ aPlayer.enabled = bValue; }
}