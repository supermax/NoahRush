using UnityEngine;
using System.Collections;
using TMS.Common.Core;

public class HudManager : ViewModel
{
	public Transform CoinsButton;

	public Transform PowerUpsButton;

	public UnityEngine.UI.Text CoinsText;

	public UnityEngine.UI.Text PowerUpsText;

	private int _coinsCount;

	private int _powerUpsCount;

	protected override void Start()
	{
		base.Start();

		CoinsButton.gameObject.SetActive(false);
		PowerUpsButton.gameObject.SetActive(false);

		Subscribe<PlayerTriggerPayload>(OnPlayerTrigger);
	}

	private void OnPlayerTrigger(PlayerTriggerPayload payload)
	{
		switch (payload.TriggerSource.tag)
		{
			case GameObjectTagNames.Gem:
				CoinsText.text = string.Format("${0}", ++_coinsCount);
				CoinsButton.gameObject.SetActive(_coinsCount > 0);
				break;

			case GameObjectTagNames.PowerUp:
				PowerUpsText.text = string.Format("{0}", ++_powerUpsCount);
				PowerUpsButton.gameObject.SetActive(_coinsCount > 0);
				break;
		}
	}
}
