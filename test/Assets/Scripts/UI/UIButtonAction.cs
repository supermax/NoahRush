using TMS.Common.Core;
using TMS.Common.Messaging;

public class UIButtonAction : MonoBehaviourBase
{
	private UIActionPayload _actionPayload;

	public UIActionType Action;

	protected override void Start()
	{
		base.Start();

		_actionPayload = new UIActionPayload {Action = Action};
	}

	public void DoAction()
	{
		Messenger.Default.Publish(_actionPayload);
	}
}