using TMS.Common.Core;
using UnityEngine;

public class GameBalanceManager : ViewModelSingleton<GameBalanceManager>
{
	public GameBalanceTemplate GameBalance;

	public PlayerBalance PlayerBalance;

	private void OnPlayerTrigger(PlayerTriggerPayload payload)
	{
		if (payload == null || payload.TriggerSource == null || 
			!GameBalance.ObjectsBalanceInfos.ContainsKey(payload.TriggerSource.tag)) return;

		var info = GameBalance.ObjectsBalanceInfos[payload.TriggerSource.tag];
		PlayerBalance.Gems += info.CointsValue;
		PlayerBalance.PowerUps += info.PowerUpsValue;
		PlayerBalance.Points += info.PointsValue;

		//switch (payload.TriggerSource.tag)
		//{
		//	case GameObjectTagNames.Gem:
				
		//		break;

		//	case GameObjectTagNames.PowerUp:
				
		//		break;
		//}

		//Debug.Log(payload);
	}

	protected override void Start()
	{
		base.Start();

		PlayerBalance.Init();
		Subscribe<PlayerTriggerPayload>(OnPlayerTrigger);
	}
}