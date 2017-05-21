using TMS.Common.Core;
using TMS.Common.Messaging;

public class UIActionHandler : MonoBehaviourBase
{
	public UIActionType ActionType;

	public string TagName;

	public UnityEngine.UI.Text Label;

	protected override void Awake()
	{
		base.Awake();

		Messenger.Default.Subscribe<UIActionPayload<string>>(OnUIAction);
	}

	protected override void OnDestroy()
	{
		Messenger.Default.Unsubscribe<UIActionPayload<string>>(OnUIAction);

		base.OnDestroy();
	}

	private void OnUIAction(UIActionPayload<string> payload)
	{
		if(payload.Action != ActionType || payload.Tag != TagName || payload.Data == null) return;

		Label.text = payload.Data;
		gameObject.SetActive(payload.IsActive);
	}
}