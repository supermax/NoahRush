using UnityEngine;

public class PlayerTriggerPayload
{
	public EthanController PlayerController { get; set; }

	public Collider TriggerSource { get; set; }

	public override string ToString()
	{
		return string.Format("[{0}] hit [{1}]", 
			PlayerController != null ?  "(" + PlayerController.tag + ") " + PlayerController.name : null,
			TriggerSource != null ? "(" + TriggerSource.tag + ") " + TriggerSource.name : null);
	}
}