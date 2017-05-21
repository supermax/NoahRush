using TMS.Common.Messaging;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBalance", menuName = "Ethan Runner/Player Balance")]
public class PlayerBalance : ScriptableObject
{
	[SerializeField]
	private int _gems;

	[SerializeField]
	private int _powerUps;

	[SerializeField]
	private int _points;

	public int Gems
	{
		get { return _gems; }
		set
		{
			if(_gems == value) return;
			_gems = value;
			UpdateUI(GameObjectTagNames.Gem, string.Format("${0}", value), value > 0);
		}
	}

	public int PowerUps
	{
		get { return _powerUps; }
		set
		{
			if (_powerUps == value) return;
			_powerUps = value;
			UpdateUI(GameObjectTagNames.PowerUp, _powerUps.ToString(), value > 0);
		}
	}

	public int Points
	{
		get { return _points; }
		set
		{
			if (_points == value) return;
			_points = value;
			UpdateUI(GameObjectTagNames.Points, string.Format("{0} pts", value), value > 0);
		}
	}

	private static void UpdateUI(string tag, string value, bool isActive)
	{
		Messenger.Default.Publish(new UIActionPayload<string>
		{
			Action = UIActionType.UpdateHud,
			Tag = tag,
			Data = value,
			IsActive = isActive
		});
	}

	public void Init()
	{
		UpdateUI(GameObjectTagNames.Points, string.Format("{0} pts", _points), _points > 0);
		UpdateUI(GameObjectTagNames.Gem, string.Format("${0}", _gems), _gems > 0);
		UpdateUI(GameObjectTagNames.PowerUp, _powerUps.ToString(), _powerUps > 0);
	}

	public void Reset()
	{
		Gems = 0;
		PowerUps = 0;
		Points = 0;
	}
}