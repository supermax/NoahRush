using UnityEngine;

public class TouchController : TMS.Common.Core.MonoBehaviourBase
{
	private bool _tap;
	private bool _isOn;

	[SerializeField]
	private GameObject _swipeControl;

	[SerializeField]
	private GameObject _tapControl;

	public bool IsOn
	{
		get { return _isOn; }
		set
		{
			_isOn = value;
			UpdateInputMode();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		IsOn = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		//_tap = MainManager.Default.SettingsManager.Configuration.TapInput;

		UpdateInputMode();
	}

	private void UpdateInputMode()
	{
		_tapControl.SetActive(_tap && IsOn);
		_swipeControl.SetActive(!_tap && IsOn);
	}
}
