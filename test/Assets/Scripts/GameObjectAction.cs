using TMS.Common.Core;

public class GameObjectAction : MonoBehaviourBase
{
	public GameObjectActionType Action;

	public void DoAction(GameObjectActionType action)
	{
		switch (action)
		{
			case GameObjectActionType.Hide:
				gameObject.SetActive(false);
				break;

			case GameObjectActionType.Show:
				gameObject.SetActive(true);
				break;

			case GameObjectActionType.DisableScript:
				enabled = false;
				break;

			case GameObjectActionType.EnableScript:
				enabled = true;
				break;

			case GameObjectActionType.Destroy:
				Destroy(gameObject);
				break;

			case GameObjectActionType.PrintLog:
				print(gameObject.name);
				break;
		}
	}
}