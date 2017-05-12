using UnityEngine;

public class TapControl : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour[] _handlers;

	public void OnLeftClick()
	{
		SendTouchMessage("OnLeftTap");
	}

	public void OnRightClick()
	{
		SendTouchMessage("OnRightTap");
	}

	private void SendTouchMessage(string message)
	{
		foreach (var handler in _handlers)
		{
			handler.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		}
	}
}