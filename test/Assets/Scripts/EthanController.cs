using UnityEngine;
using System.Collections;
using TMS.Common.Core;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(ThirdPersonCharacter), typeof(ThirdPersonUserControl))]
public class EthanController : MonoBehaviourBase
{
	private ThirdPersonCharacter _character;
	private ThirdPersonUserControl _userControl;

	private Vector3 _initControllerPosition;
	private Quaternion _initControllerRotation;

	protected override void Start()
	{
		base.Start();

		_character = GetComponent<ThirdPersonCharacter>();
		_initControllerPosition = _character.transform.position;
		_initControllerRotation = _character.transform.rotation;

		_userControl = GetComponent<ThirdPersonUserControl>();

		if (Camera.main == null)
		{
			Debug.LogWarning("Main camera is required for better speed formula");
			return;
		}
		_mainCameraTransform = Camera.main.transform;
	}

	public void GotoStart()
	{
		_character.transform.position = _initControllerPosition;
		_character.transform.rotation = _initControllerRotation;
	}

	void OnTriggerEnter(Collider other)
	{
		// TODO
		print(other.gameObject.name);
	}

	private Vector3 _camForward;    
	private Vector3 _moveVector;
	private Transform _mainCameraTransform;
	private bool _isJumping;

	public float MoveForwardSpeed = 1f;

	private float _moveSideSpeed = 0f;

	private void Update()
	{
		if (!_isJumping)
		{
			_isJumping = CrossPlatformInputManager.GetButtonDown("Jump");
		}
	}

	private bool _crouch;

	// Fixed update is called in sync with physics
	private void FixedUpdate()
	{
		// read inputs
		float h = _moveSideSpeed + CrossPlatformInputManager.GetAxis("Horizontal");
		float v = MoveForwardSpeed; //CrossPlatformInputManager.GetAxis("Vertical");
		//print("Vertical: " + v);

		if (Input.GetKey(KeyCode.C))
			_crouch = true;

		// calculate move direction to pass to character
		if (_mainCameraTransform != null)
		{
			// calculate camera relative direction to move:
			_camForward = Vector3.Scale(_mainCameraTransform.forward, new Vector3(1, 0, 1)).normalized;
			_moveVector = v * _camForward + h * _mainCameraTransform.right;
		}
		else
		{
			// we use world-relative directions in the case of no main camera
			_moveVector = v * Vector3.forward + h * Vector3.right;
		}
#if !MOBILE_INPUT
		// walk speed multiplier
		if (Input.GetKey(KeyCode.LeftShift)) _moveVector *= 0.5f;
#endif

		// pass all parameters to the character control script
		_character.Move(_moveVector, _crouch, _isJumping);

		_isJumping = false;
	}

	public void OnLeft()
	{
		_moveSideSpeed = -1f;
		_isJumping = true;

		StartCoroutine(RotateToCenter());
	}

	public void OnRight()
	{
		_moveSideSpeed = 1f;
		_isJumping = true;

		StartCoroutine(RotateToCenter());
	}

	public void OnUp()
	{
		_isJumping = true;
	}

	public void OnDown()
	{
		_crouch = true;

		StartCoroutine(GetUp());
	}

	private IEnumerator GetUp()
	{
		yield return new WaitForSeconds(0.2f);

		_crouch = false;
	}

	private IEnumerator RotateToCenter()
	{
		yield return new WaitForSeconds(0.2f);

		if(_moveSideSpeed < 0)
			_moveSideSpeed = 1f;
		else
			_moveSideSpeed = -1f;

		yield return new WaitForEndOfFrame();

		_moveSideSpeed = 0f;
	}
}
