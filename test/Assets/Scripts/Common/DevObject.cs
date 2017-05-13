using UnityEngine;
using System.Collections;

public class DevObject : GameObjectAction
{
	protected override void Awake()
	{
		base.Awake();

		DoAction(Action);
	}
}