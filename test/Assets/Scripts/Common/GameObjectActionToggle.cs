public class GameObjectActionToggle : GameObjectAction
{
	protected override void DoActionInternal(GameObjectActionType action)
	{
		switch (action)
		{
			case GameObjectActionType.Hide:
				action = GameObjectActionType.Show;
				break;

			case GameObjectActionType.Show:
				action = GameObjectActionType.Hide;
				break;

			case GameObjectActionType.DisableScript:
				action = GameObjectActionType.EnableScript;
				break;

			case GameObjectActionType.EnableScript:
				action = GameObjectActionType.DisableScript;
				break;
		}
		base.DoActionInternal(action);
	}
}