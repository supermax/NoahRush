using System.Collections.Generic;
using TMS.Common.Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceTemplate", menuName = "Ethan Runner/Game Balance Template")]
public class GameBalanceTemplate : ScriptableObject
{
	public GameObjectBalanceInfo[] ObjectsBalanceInfo;

	private readonly object _locker = new object();

	private IDictionary<string, GameObjectBalanceInfo> _objectsBalanceInfos;

	private static IDictionary<string, GameObjectBalanceInfo> PatchInitObjectBalanceInfos(IEnumerable<GameObjectBalanceInfo> objectsBalanceInfo)
	{
		var infos = new Dictionary<string, GameObjectBalanceInfo>();
		foreach (var info in objectsBalanceInfo)
		{
			infos[info.Name] = info;
		}
		return infos;
	}

	public IDictionary<string, GameObjectBalanceInfo> ObjectsBalanceInfos
	{
		get
		{
			return _locker.InitWithLock(ref _objectsBalanceInfos, 
				() => PatchInitObjectBalanceInfos(ObjectsBalanceInfo));
		}
	}
}