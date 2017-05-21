public class UIActionPayload
{
	public string Tag { get; set; }

	public UIActionType Action { get; set; }

	public bool IsActive { get; set; }
}

public class UIActionPayload<T> : UIActionPayload
{
	public T Data { get; set; }
}