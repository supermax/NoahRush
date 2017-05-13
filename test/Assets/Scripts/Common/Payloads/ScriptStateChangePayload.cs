using UnityEngine;

public class ScriptStateChangePayload<T>
	where T : MonoBehaviour
{
	public T Source { get; set; }

	public ScriptStateType State { get; set; }
}