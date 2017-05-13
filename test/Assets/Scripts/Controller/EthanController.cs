#region Usings

using System;
using System.Collections;
using TMS.Common.Core;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

#endregion

[RequireComponent(typeof(ThirdPersonCharacter))]
public class EthanController : ViewModel
{
	private ThirdPersonCharacter _character;

	private bool _crouch;

	private Vector3 _initControllerPosition;
	private Quaternion _initControllerRotation;
	private bool _isJumping;
	private Vector3 _mainCameraInitPos;

	private Transform _mainCameraTransform;

	private Vector3 _moveVector;

	private PlayerMovePayload _playerMovePayload;
	private PlayerTriggerPayload _playerTriggerPayload;

	public bool JumpOnTurn = true;

	public float MaxTurnRange = 2f;

	public float MoveForwardSpeed = 1f;

	public float TurnSpeed = 1f;

	protected override void Awake()
	{
		_playerMovePayload = new PlayerMovePayload { PlayerController = this };
		_playerTriggerPayload = new PlayerTriggerPayload { PlayerController = this };

		base.Awake();

		_character = GetComponent<ThirdPersonCharacter>();
		_initControllerPosition = _character.transform.position;
		_initControllerRotation = _character.transform.rotation;

		if (Camera.main == null)
		{
			Debug.LogError("Main camera is required for the game!");
			return;
		}
		_mainCameraTransform = Camera.main.transform;
		_mainCameraInitPos = _mainCameraTransform.position;

		iTween.Init(_mainCameraTransform.gameObject);
		iTween.Init(_character.gameObject);
	}

	protected override void Start()
	{
		base.Start();

		Subscribe<UIActionPayload>(OnUIAction);
	}

	private void OnUIAction(UIActionPayload payload)
	{
		switch (payload.Action)
		{
			case UIActionType.StartGame:
			case UIActionType.RestartGame:
				GotoStart();
				break;

			case UIActionType.PauseGame:
				enabled = false;
				Time.timeScale = 0f;
				break;

			case UIActionType.ResumeGame:
				enabled = true;
				Time.timeScale = 1f;
				break;

			case UIActionType.QuitGame:
				break;

			case UIActionType.ShowSettings:
				break;
		}
	}

	public void GotoStart()
	{
		_character.transform.position = _initControllerPosition;
		_character.transform.rotation = _initControllerRotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		_playerTriggerPayload.TriggerSource = other;

		Publish(_playerTriggerPayload);
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

	public void OnLeft()
	{
		if (_character.transform.position.x <= -MaxTurnRange)
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

		_character.transform.position = new Vector3(x, y, z);
	}
}