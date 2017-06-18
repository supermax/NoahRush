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

	private Vector3 _mainCameraPosOffset;

	private Vector3 _moveVector;

	private PlayerMovePayload _playerMovePayload;
	private PlayerTriggerPayload _playerTriggerPayload;

	public Transform MainCamera;

	public bool JumpOnTurn = true;

	public bool CanRun;

	public float StartRunTime = 1.5f;

	public float MaxTurnRange = 2f;

	public float MoveForwardSpeed = 1f;

	public float TurnSpeed = 1f;

	public float CamMoveSpeed = 5f;

	private float _defaultTimeScale = 1f;

	protected override void Awake()
	{
		base.Awake();

		_defaultTimeScale = Time.timeScale;

		_playerMovePayload = new PlayerMovePayload { PlayerController = this };
		_playerTriggerPayload = new PlayerTriggerPayload { PlayerController = this };
		_character = GetComponent<ThirdPersonCharacter>();
		
		iTween.Init(MainCamera.gameObject);
	}

	protected override void Start()
	{
		base.Start();

		_initControllerPosition = _character.transform.position;
		_initControllerRotation = _character.transform.rotation;
		_mainCameraPosOffset = MainCamera.position - transform.position;

		Subscribe<UIActionPayload>(OnUIAction);
	}

	private IEnumerator StartRunning()
	{
		yield return new WaitForSeconds(StartRunTime);
		
		CanRun = true;
	}

	private void StartGame()
	{
		StartCoroutine(StartRunning());
	}

	private void ResumeGame()
	{
		CanRun = true;
		gameObject.SetActive(true);
		enabled = true;
		Time.timeScale = _defaultTimeScale;
	}

	private void PauseGame()
	{
		CanRun = false;
		enabled = false;
		Time.timeScale = 0f;
	}

	private void OnUIAction(UIActionPayload payload)
	{
		switch (payload.Action)
		{
			case UIActionType.StartGame:
			case UIActionType.RestartGame:
				GotoStart();
				ResumeGame();
				StartGame();
				break;

			case UIActionType.PauseGame:
				PauseGame();
				break;

			case UIActionType.ResumeGame:
				ResumeGame();
				break;

			case UIActionType.QuitGame:
				break;

			case UIActionType.SettingsSwitch:
				break;
		}
	}

	public void GotoStart()
	{
		_moveVector = _initControllerPosition;
		_character.transform.position = _initControllerPosition;
		_character.transform.rotation = _initControllerRotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		_playerTriggerPayload.TriggerSource = other;
		
		Publish(_playerTriggerPayload);

		_playerTriggerPayload.TriggerSource = null;
	}

	// Fixed update is called in sync with physics
	private void FixedUpdate()
	{
		if(!CanRun) return;

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
		var x = _character.gameObject.transform.position.x / 2f;
		var y = _character.gameObject.transform.position.y / 2f + _mainCameraPosOffset.y;
		var z = _character.gameObject.transform.position.z + _mainCameraPosOffset.z / 2f;
		var targetCamPos = new Vector3(x, y, z);

		//iTween.MoveUpdate(_mainCameraTransform.gameObject, new Vector3(x / 2, y, z), 0.5f);

		MainCamera.position = Vector3.Lerp(MainCamera.position, targetCamPos, CamMoveSpeed * Time.deltaTime);
	}

	public void OnLeft()
	{
		if (_character.transform.position.x <= -MaxTurnRange)
		{
			iTween.ShakePosition(MainCamera.gameObject, new Vector3(-0.1f, 0f, 0f), 0.5f);
			return;
		}

		StartCoroutine(TurnPlayer(-TurnSpeed));
	}

	public void OnRight()
	{
		if (_character.transform.position.x >= MaxTurnRange)
		{
			iTween.ShakePosition(MainCamera.gameObject, new Vector3(0.1f, 0f, 0f), 0.5f);
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