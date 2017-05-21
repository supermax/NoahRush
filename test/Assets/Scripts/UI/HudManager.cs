using UnityEngine;
using TMS.Common.Core;

public class HudManager : ViewModel
{
	//public Transform CoinsButton;

	//public Transform PowerUpsButton;

	//public UnityEngine.UI.Text CoinsText;

	//public UnityEngine.UI.Text PowerUpsText;

	//public UnityEngine.UI.Text PointsText;

	//protected override void Start()
	//{
	//	base.Start();

	//	//CoinsButton.gameObject.SetActive(false);
	//	//PowerUpsButton.gameObject.SetActive(false);

	//	//Subscribe<PlayerTriggerPayload>(OnPlayerTrigger);
	//	//Subscribe<UIActionPayload>(OnUIAction);
	//}

	//private void OnPlayerTrigger(PlayerTriggerPayload payload)
	//{
	//	if(payload == null || payload.TriggerSource == null) return;

	//	switch (payload.TriggerSource.tag)
	//	{
	//		case GameObjectTagNames.Gem:
	//			CoinsText.text = string.Format("${0}", ++_coinsCount);
	//			CoinsButton.gameObject.SetActive(_coinsCount > 0);
	//			break;

	//		case GameObjectTagNames.PowerUp:
	//			PowerUpsText.text = string.Format("{0}", ++_powerUpsCount);
	//			PowerUpsButton.gameObject.SetActive(_powerUpsCount > 0);
	//			break;
	//	}

	//	Debug.Log(payload);
	//}

	//private void OnUIAction(UIActionPayload payload)
	//{
	//	switch (payload.Action)
	//	{
	//		case UIActionType.StartGame:
	//			// TODO
	//			break;

	//		case UIActionType.PauseGame:
	//			gameObject.SetActive(false);
	//			break;

	//		case UIActionType.RestartGame:
	//			// TODO
	//			break;

	//		case UIActionType.ResumeGame:
	//			gameObject.SetActive(true);
	//			break;

	//		case UIActionType.QuitGame:
	//			// TODO
	//			break;

	//		case UIActionType.ShowSettings:
	//			// TODO
	//			break;
	//	}
	//}
}
