using UnityEngine;
using System.Collections;
using TMS.Common.Core;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter), typeof(ThirdPersonUserControl))]
public class EthanController : MonoBehaviourBase
{
	private ThirdPersonCharacter _controller;
	private ThirdPersonUserControl _userControl;

	private Vector3 _initControllerPosition;
	private Quaternion _initControllerRotation;

	protected override void Awake()
	{
		base.Awake();

		_controller = GetComponent<ThirdPersonCharacter>();
		_initControllerPosition = _controller.transform.position;
		_initControllerRotation = _controller.transform.rotation;

		_userControl = GetComponent<ThirdPersonUserControl>();
	}

	public void GotoStart()
	{
		_controller.transform.position = _initControllerPosition;
		_controller.transform.rotation = _initControllerRotation;
	}

	void OnTriggerEnter(Collider other)
	{
		print(other.gameObject.name);
	}

}
