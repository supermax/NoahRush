#region Usings

using System;
using System.Collections;
using TMS.Common.Core;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

#endregion

[RequireComponent(typeof(ThirdPersonCharacter))]
public class EthanController : ViewModel
{
	private Vector3 _camForward;
	private ThirdPersonCharacter _character;

	private bool _crouch;

	private Vector3 _initControllerPosition;
	private Quaternion _initControllerRotation;
	private bool _isJumping;
	private Vector3 _mainCameraInitPos;

	private Transform _mainCameraTransform;

	private Vector3 _moveVector;

	private PlayerMovePayload _playerMovePayload;

	public float MoveForwardSpeed = 1f;

	protected override void Start()
	{
		base.Start();

		_character = GetComponent<ThirdPersonCharacter>();
		_initControllerPosition = _character.transform.position;
		_initControllerRotation = _character.transform.rotation;

		if (Camera.main == null)
		{
			Debug.LogWarning("Main camera is required for better speed formula");
			return;
		}
		_mainCameraTransform = Camera.main.transform;
		_mainCameraInitPos = _mainCameraTransform.position;

		_playerMovePayload = new PlayerMovePayload {PlayerController = this};

		iTween.Init(_mainCameraTransform.gameObject);
		iTween.Init(_character.gameObject);
	}

	public void GotoStart()
	{
		_character.transform.position = _initControllerPosition;
		_character.transform.rotation = _initControllerRotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		// TODO
		print(other.gameObject.name);
	}

	private void Update()
	{
		if (!_isJumping)
			_isJumping = CrossPlatformInputManager.GetButtonDown("Jump");
	}

	// Fixed update is called in sync with physics
	private void FixedUpdate()
	{
		// read inputs
		var v = MoveForwardSpeed;
		_moveVector = v * Vector3.forward;

		// pass all parameters to the character control script
		_character.Move(_moveVector, _crouch, _isJumping);

		_isJumping = false;

		MoveCameraAfterPlayer();

		Publish(_playerMovePayload);
	}

	private void MoveCameraAfterPlayer()
	{
		var x = _character.gameObject.transform.position.x;
		var y = _mainCameraTransform.position.y;
		var z = _character.gameObject.transform.position.z + _mainCameraInitPos.z / 2;

		iTween.MoveUpdate(_mainCameraTransform.gameObject, new Vector3(x / 2, y, z), 0.5f);
	}

	public bool JumpOnTurn = true;

	public float TurnSpeed = 1f;

	public float MaxTurnRange = 2f;

	public void OnLeft()
	{
		if(_character.transform.position.x <= -MaxTurnRange)
		{
			iTween.ShakePosition(_mainCameraTransform.gameObject, new Vector3(-0.1f, 0f, 0f), 0.5f);
			return;
		}

		StartCoroutine(TurnPlayer(-TurnSpeed));
	}

	public void OnRight()
	{
		if (_character.transform.position.x >= MaxTurnRange)
		{
			iTween.ShakePosition(_mainCameraTransform.gameObject, new Vector3(0.1f, 0f, 0f), 0.5f);
			return;
		}

		StartCoroutine(TurnPlayer(TurnSpeed));
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
		yield return null;

		_crouch = false;
	}

	private IEnumerator TurnPlayer(float turnSpeed)
	{
		_isJumping = JumpOnTurn;

		yield return null;

		var multiplier = Math.Abs(_character.transform.position.x) < 0.25f ? 2f : 0f;

		var x = turnSpeed * multiplier;
		var y = _character.transform.position.y;
		var z = _character.transform.position.z;
		
		//iTween.MoveTo(_character.gameObject, new Vector3(x / 2f, y, z), 0.5f);

		_character.transform.position = new Vector3(x, y, z);
	}

	//private IEnumerator TurnPlayer(float turnSpeed, float multiplier = 2)
	//{
	//	_isJumping = JumpOnTurn;

	//	yield return null;

	//	var y = _character.transform.position.y;
	//	var z = _character.transform.position.z;
	//	_character.transform.position = new Vector3(turnSpeed, y, z);

	//	yield return null;

	//	_character.transform.position = new Vector3(turnSpeed * multiplier, y, z);
	//}
}
