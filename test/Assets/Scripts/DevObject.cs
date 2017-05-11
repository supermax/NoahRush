using UnityEngine;
using System.Collections;

public class DevObject : MonoBehaviour
{
	public bool IsActiveOnRun;

	public bool DestroyOnRun;

	void Awake()
	{
		if (DestroyOnRun)
		{
			DestroyImmediate(gameObject);
			return;
		}

		gameObject.SetActive(IsActiveOnRun);
	}
}
