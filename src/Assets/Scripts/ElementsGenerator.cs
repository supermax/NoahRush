// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;


using System.IO;

/*
*	FUNCTION:
*	This script generates obstacles, currency units and powerups on every patch.
*	The spline is used to ensure obstacles are created after regular distances. The start of 
*	the spline is 0.0f and the end is 1.0f.
*
*	USED BY:
*	This script is a part of the "Player" prefab.
*
*/
public class ElementsGenerator : MonoBehavour
{
	
public enum ElementType { SingleLaneObstacle = 0, MultiLaneObstacle = 1, Currency = 3, Powerup = 4, Null = 5 }

//properties of elements (obstacles, powerups, currency)
private class Element 
{
	Transform[] tPrefabHandle;//array of clones of a particular element
	int iFrequency;	//frequency of occurance of a particular element
	ElementType elementType;
	int iPrefabHandleIndex;//element clone currently in use
}

GameObject[] obstaclePrefabs;//prefabs of obstacles
GameObject[] powerupPrefabs;	//prefabs of powerups
GameObject currencyPrefab;	//prefab of currency
private Element[] elements;	//class array
private Transform tPrefabHandlesParent;//holds all instances of element clones

private int iObstacleCount;//total number obstacles
private int iPowerupCount;	//total number of powerups
private int iTotalCount;		//total count of all elements handled by this script

//script references
private PowerupsMainController hPowerupsMainController;
private CheckPointsMain hCheckPointsMain;

private float fDefaultIncrement = 0.05f;//the default gap between obstacles
private int iLastPowerupGenerated = 0;	//how many patches have been generated without a powerup
private int iForcePowerupGeneration = 2;	//force powerup generation after how many patches
private bool  bPowerupPlaced = false;	//force only a single powerup at a time

/*
*	FUNCTION: Tell the randomiser to generate obstacles on newly created patch
*	CALLED BY:	PatchesRandomizer.createNewPatch()
*/
private bool  bGenerateElements;	//flag the script to generate elements on next patch
public void generateElements (){
	bGenerateElements = true;
}

void  Start (){
	iObstacleCount = obstaclePrefabs.Length;
	iPowerupCount = powerupPrefabs.Length;
	iTotalCount = iObstacleCount + iPowerupCount + 1;//obstacles + powerups + currency
	
	bGenerateElements = true;
	bPowerupPlaced = true;//do not place powerup on first patch
	System.DateTime dt = System.DateTime.Now;
	Random.seed = dt.Hour * dt.Minute * dt.Second;
	
	setPrefabHandlers();
	
	hPowerupsMainController = this.GetComponent<PowerupsMainController>() as PowerupsMainController;
	hCheckPointsMain = GameObject.Find("Player").GetComponent<CheckPointsMain>() as CheckPointsMain;
	
	//generate elements on first patch
	float i = 0.20f;
	while (i < 0.99f)
	{	
		FIXME_VAR_TYPE incrementValue= generateElements(getRandomElement(), i, true);//get any type of element
		i += incrementValue;
	}//end of while
}

void  Update (){
	float i = 0.0f;
	if (bGenerateElements)
	{
		bGenerateElements = false;
		bPowerupPlaced = false;//place powerup on current patch if randomised
		iLastPowerupGenerated++;
		
		//Loop from 0.0f to 1.0f to generate obstacle on the complete patch
		while (i < 0.99f)
		{			
			FIXME_VAR_TYPE incrementValue= generateElements(getRandomElement(), i, false);//get any type of element
			i += incrementValue;
		}//end of while
	}//end of if
}

/*
*	FUNCITON: Generate random elements and group of elements.
*	PARAMETER 1: Element to generate.
*	PARAMETER 2: Where to generate based on the spline.
*	PARAMETER 3: If the patch is the starting patch or the next patch generated.
*/
private void  generateElements ( int elementNumber ,   float fLocation ,   bool bStartPatch  ){	
	 Vector3 v3Position;	//position to put the obstacle on
	RaycastHit hitInfo;	//check y-axis location
	float CurrentAngle;	//billboard the obstacle towards the player
	float fDefaultDisplacementValue = 15;
	float fDisplacement = fDefaultDisplacementValue;	//horizontal displacement
	
	//if obstacle only covers one lane generate a random number of instances of the 
	//obstacle on random locations within a particular radius
	if (elements[elementNumber].elementType == ElementType.SingleLaneObstacle)
	{		
		for (int i = 0; i<Random.Range(1,9); i++)
		{
			//pick where to generate obstacle horizontally on path
			FIXME_VAR_TYPE iLane= Random.Range(0,3);
			if (iLane == 0)
				fDisplacement = Mathf.Abs(fDefaultDisplacementValue);
			else if (iLane == 1)
				fDisplacement = -Mathf.Abs(fDefaultDisplacementValue);
			else
				fDisplacement = 0;
			
			//pick where to generate obstacle vertically on path
			FIXME_VAR_TYPE iVerticalPosition= Random.Range(0,3);
			if (iVerticalPosition == 0)
			{
				v3Position = getPosition(fLocation+fDefaultIncrement, bStartPatch);
				hitInfo = getHitInfo(v3Position);
			}
			else if (iVerticalPosition == 1)
			{
				v3Position = getPosition(fLocation+(fDefaultIncrement*2), bStartPatch);
				hitInfo = getHitInfo(v3Position);
			}
			else
			{
				v3Position = getPosition(fLocation, bStartPatch);
				hitInfo = getHitInfo(v3Position);		
			}
			
			if (fLocation >= 1.0f)//dont create obstacles on next patch
				continue;
							
			v3Position.z += fDisplacement;
			v3Position.y = hitInfo.point.y;
			CurrentAngle = -hCheckPointsMain.getWaypointAngle();
			instantiateElement(elementNumber, v3Position, CurrentAngle, hitInfo.normal);
		}		
		
		fLocation = fDefaultIncrement*3;
	}
	//if the randomised element is currency generate three or more currency units together in a particular lane
	//also generate single lane obstacles on the lanes without the currency
	else if (elements[elementNumber].elementType == ElementType.Currency)
	{
		float[] fObstacleDisplacement = new float[2];//horizontal displacement of obstacles that will be created along currency
		
		//pick the lane where to generate currency
		FIXME_VAR_TYPE iCurrencyLane= Random.Range(0,3);
		if (iCurrencyLane == 0)
		{
			fDisplacement = Mathf.Abs(fDefaultDisplacementValue);
			fObstacleDisplacement[0] = 0;			
			fObstacleDisplacement[1] = -Mathf.Abs(fDefaultDisplacementValue);
		}
		else if (iCurrencyLane == 1)
		{
			fDisplacement = -Mathf.Abs(fDefaultDisplacementValue);
			fObstacleDisplacement[0] = Mathf.Abs(fDefaultDisplacementValue);
			fObstacleDisplacement[1] = 0;
		}
		else
		{
			fDisplacement = 0;
			fObstacleDisplacement[0] = -Mathf.Abs(fDefaultDisplacementValue);			
			fObstacleDisplacement[1] = Mathf.Abs(fDefaultDisplacementValue);
		}
		
		int iToGenerate = Random.Range(3,7);//amount of currency units to generate
		for (i=0; i<iToGenerate; i++)
		{
			v3Position = getPosition(fLocation, bStartPatch);
			v3Position.z += fDisplacement;
			hitInfo = getHitInfo(v3Position);
			v3Position.y = hitInfo.point.y;
			CurrentAngle = -hCheckPointsMain.getWaypointAngle();			
			instantiateElement(elementNumber, v3Position, CurrentAngle, hitInfo.normal);
			
			FIXME_VAR_TYPE parallelElement= getRandomElement();
			if (elements[parallelElement].elementType != ElementType.MultiLaneObstacle)
			{
				v3Position = getPosition(fLocation, bStartPatch);
				v3Position.z += fObstacleDisplacement[Random.Range(0,fObstacleDisplacement.Length)];
				hitInfo = getHitInfo(v3Position);
				v3Position.y = hitInfo.point.y;
				CurrentAngle = -hCheckPointsMain.getWaypointAngle();
				instantiateElement(parallelElement, v3Position, CurrentAngle, hitInfo.normal);
			}
			
			fLocation += 0.010f;
			if (fLocation >= 1.0f)
				break;
		}
		
		fLocation = iToGenerate*0.010f;
	}
	//if the obstacle randomised covers multiple lanes, generate it and move on
	else if (elements[elementNumber].elementType == ElementType.MultiLaneObstacle)
	{
		v3Position = getPosition(fLocation, bStartPatch);
		hitInfo = getHitInfo(v3Position);		
		v3Position.y = hitInfo.point.y;
		CurrentAngle = -hCheckPointsMain.getWaypointAngle();
		instantiateElement(elementNumber, v3Position, CurrentAngle, hitInfo.normal);
		
		fLocation = 0.05f;
	}
	
	return fLocation;
}//end of instantiate by number

/*
*	FUNCTION: Transfroms the location on spline (0.0f to 1.0f) to a Vector3 position
*	PARAMETER 1: Position on spline (0.0f to 1.0f)
*	PARAMETER 2: Is the patch the first or consecutive generated ones
*/
private Vector3 getPosition ( float fLocation ,   bool bStartPatch  ){
	if (bStartPatch == true)	
		return hCheckPointsMain.getCurrentWSPointBasedOnPercent(fLocation);
	else
		return hCheckPointsMain.getNextWSPointBasedOnPercent(fLocation);
}

/*
*	FUNCTION: Calculate vertical position to put the element on
*	PARAMETER 1: The Vector3 position where the element will be placed.
*	RETURNS: The Raycast hit information.
*/
private RaycastHit getHitInfo ( Vector3 v3Position  ){
	//Raycast towards the ground to check if terrain present
	bool  Groundhit = false;
	RaycastHit hitInfo;
	Vector3 DownPos = Vector3(0,-100,0) + v3Position;
	FIXME_VAR_TYPE layerMask= 1<<9;
	Groundhit = Physics.Linecast(v3Position + Vector3(0,100,0),DownPos,hitInfo,layerMask);
	
	return hitInfo;
}

/*
*	FUNCTION: Place element on the required position.
*	PARAMETER 1: The obstacle, currency unit or powerup to instantiate.
*	PARAMETER 2: The position where to place the element.
*	PARAMETER 3: The angle of the element which is based on path's curve.
*	PATAMETER 4: The Raycast hit information which is based on the element's placement position.
*/
private void instantiateElement ( int elementNumber ,   Vector3 v3Position ,   float CurrentAngle ,   Vector3 hitInfoNormal  ){
	if (elementNumber < 0)
		return;
	
	Transform ObjectHandle;
	
	if (elementNumber < iObstacleCount)//obstacles
	{
		ObjectHandle = elements[elementNumber].tPrefabHandle[elements[elementNumber].iPrefabHandleIndex];		
		elements[elementNumber].iPrefabHandleIndex++;
		if (elements[elementNumber].iPrefabHandleIndex >= elements[elementNumber].tPrefabHandle.Length)
			elements[elementNumber].iPrefabHandleIndex = 0;
		ObjectHandle.gameObject.SetActive(true);
		ObjectHandle.up = hitInfoNormal;
		ObjectHandle.position = v3Position;
		ObjectHandle.localEulerAngles = Vector3(0,0,0);
		ObjectHandle.Rotate(0,CurrentAngle,0);
	}
	else if (elementNumber >= iObstacleCount && elementNumber < (iObstacleCount+iPowerupCount))//powerups
	{
		ObjectHandle = elements[elementNumber].tPrefabHandle[elements[elementNumber].iPrefabHandleIndex];
		elements[elementNumber].iPrefabHandleIndex++;
		if (elements[elementNumber].iPrefabHandleIndex >= elements[elementNumber].tPrefabHandle.Length)
			elements[elementNumber].iPrefabHandleIndex = 0;
		ObjectHandle.gameObject.SetActive(true);
		ObjectHandle.up = hitInfoNormal;
		ObjectHandle.position = v3Position;
		ObjectHandle.localEulerAngles = Vector3(0,0,0);
		ObjectHandle.Rotate(0,CurrentAngle,0);
		(ObjectHandle.GetComponent<PowerupScript>() as PowerupScript).initPowerupScript();
	}
	else if (elementNumber == (iObstacleCount+iPowerupCount))//currency
	{
		ObjectHandle = elements[elementNumber].tPrefabHandle[elements[elementNumber].iPrefabHandleIndex];
		elements[elementNumber].iPrefabHandleIndex++;
		if (elements[elementNumber].iPrefabHandleIndex >= elements[elementNumber].tPrefabHandle.Length)
			elements[elementNumber].iPrefabHandleIndex = 0;
		ObjectHandle.gameObject.SetActive(true);
		ObjectHandle.up = hitInfoNormal;
		ObjectHandle.position = v3Position;
		ObjectHandle.localEulerAngles = Vector3(0,0,0);
		ObjectHandle.Rotate(0,CurrentAngle,0);
		(ObjectHandle.GetComponent<PowerupScript>() as PowerupScript).initPowerupScript();
	}
}

/*
*	FUNCTION: Randomise an element to generate on the path
*	RETURNS: Element number to generate
*	CALLED BY: 	Start()
*				Update()
*				generateElements()
*/
private int getRandomElement (){
	float highestFrequency = 0;
	int elementIndex = 0;
	
	int i = 0;
	float tempFreq = 0;
	
	if (iLastPowerupGenerated > iForcePowerupGeneration 	//force powerup generation if not generated for a while
	&& !hPowerupsMainController.isPowerupActive() //ensure that a powerup is not currently active
	&& bPowerupPlaced == false)//do not generate powerup if one is already active
	{
		for (i= iObstacleCount; i<(iObstacleCount+iPowerupCount); i++)
		{
			tempFreq = elements[i].iFrequency * Random.value;
			if (highestFrequency < tempFreq)
			{
				highestFrequency = tempFreq;
				elementIndex = i;
			}
		}
		iLastPowerupGenerated = 0;//reset variable		
	}
	else//normal case; generate any random element
	{
		for (i=0; i<iTotalCount; i++)
		{	
			if ( (elements[i].elementType == ElementType.Powerup 
			&& hPowerupsMainController.isPowerupActive() )	//do not generate powerup if one is already active
			|| (elements[i].elementType == ElementType.Powerup && bPowerupPlaced == true) )	//do not place powerup on current patch if one has already been placed
				continue;
			
			tempFreq = elements[i].iFrequency * Random.value;
			if (highestFrequency < tempFreq)
			{
				highestFrequency = tempFreq;
				elementIndex = i;
			}
		}//end of for
	}
	
	//if a powerup has been placed, stop placing more powerups in current patch
	if (elements[elementIndex].elementType == ElementType.Powerup)
		bPowerupPlaced = true;
	
	return elementIndex;
}

/*
*	FUNCTION: Generate a list of clones of all elements to be used int the game
*	CALLED BY: Start()
*/
private void  setPrefabHandlers (){
	tPrefabHandlesParent = GameObject.Find("PFHandlesGroup").transform;//get the transfrom of the parent of all clones
	elements = new Element[iTotalCount];
	
	//Obstacles
	for (FIXME_VAR_TYPE i=0; i< iObstacleCount; i++)
	{			
		elements[i].iFrequency = (obstaclePrefabs[i].GetComponent<ObstacleScript>() as ObstacleScript).frequency;//get the occurance frequency of the particular obstacle
		elements[i].tPrefabHandle = new Transform[elements[i].iFrequency*4 + 1];	//allocate memory to element prefabs handler array
		elements[i].iPrefabHandleIndex = 0;
		elements[i].elementType = (obstaclePrefabs[i].GetComponent<ObstacleScript>() as ObstacleScript).obstacleAreaType;	//set the element type
		
		//generate obstacle clones
		for (FIXME_VAR_TYPE j=0; j<elements[i].tPrefabHandle.Length; j++)
		{
			elements[i].tPrefabHandle[j] = Instantiate(obstaclePrefabs[i],Vector3(-100,0,0),Quaternion()).transform;
			elements[i].tPrefabHandle[j].parent = tPrefabHandlesParent;
		}		
	}
	
	//Powerups
	int index = 0;
	for (i=iObstacleCount; i<(iPowerupCount+iObstacleCount); i++)
	{		
		elements[i].iFrequency = (powerupPrefabs[index].GetComponent<PowerupScript>() as PowerupScript).frequency;
		elements[i].tPrefabHandle = new Transform[elements[i].iFrequency*1 + 1];
		elements[i].iPrefabHandleIndex = 0;
		elements[i].elementType = ElementType.Powerup;
		
		for (j=0; j<elements[i].tPrefabHandle.Length; j++)
		{
			elements[i].tPrefabHandle[j] = Instantiate(powerupPrefabs[index],Vector3(-100,0,0),Quaternion()).transform;
			elements[i].tPrefabHandle[j].parent = tPrefabHandlesParent;
		}
		
		++index;		
	}
	
	//Currency
	index = iPowerupCount+iObstacleCount;
	elements[index].iFrequency = (currencyPrefab.GetComponent<PowerupScript>() as PowerupScript).frequency;
	elements[index].tPrefabHandle = new Transform[elements[i].iFrequency*11 + 1];
	elements[index].iPrefabHandleIndex = 0;
	elements[index].elementType = ElementType.Currency;
	for (j=0; j<elements[index].tPrefabHandle.Length; j++)
	{
		elements[index].tPrefabHandle[j] = Instantiate(currencyPrefab,Vector3(-100,0,0),Quaternion()).transform;
		elements[index].tPrefabHandle[j].parent = tPrefabHandlesParent;
	}	
}
}