using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class CollisionHandler : GameObjectAction
{
	void OnTriggerEnter(Collider other)
	{
		DoAction(Action);
	}
}