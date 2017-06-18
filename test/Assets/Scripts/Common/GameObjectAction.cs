using System.Collections;
using TMS.Common.Core;
using UnityEngine;

public class GameObjectAction : MonoBehaviourBase
{
	public GameObjectActionType Action;

	public GameObjectActionTrigger Trigger = GameObjectActionTrigger.Awake;

	public float StartTime = 0;

	public Behaviour Target;

	protected override void Awake()
	{
		base.Awake();

		if(Target == null) Target = this;

		if(Trigger != GameObjectActionTrigger.Awake) return;
		DoAction(Action);
	}

	protected override void Start()
	{
		base.Start();

		if (Trigger != GameObjectActionTrigger.Start) return;
		DoAction(Action);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (Trigger != GameObjectActionTrigger.Enabled) return;
		DoAction(Action);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (Trigger != GameObjectActionTrigger.Disabled) return;
		DoAction(Action);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (Trigger != GameObjectActionTrigger.Distroyed) return;
		DoAction(Action);
	}

	public virtual void DoAction()
	{
		DoAction(Action);
	}

	public virtual void DoAction(GameObjectActionType action)
	{
		if (StartTime <= 0)
		{
			DoActionInternal(action);
			return;
		}

		StartCoroutine(DoActionCoroutine(action));
	}

	protected virtual IEnumerator DoActionCoroutine(GameObjectActionType action)
	{
		yield return new WaitForSeconds(StartTime);

		DoActionInternal(action);
	}

	protected virtual void DoActionInternal(GameObjectActionType action)
	{
		switch (action)
		{
			case GameObjectActionType.Hide:
				Target.gameObject.SetActive(false);
				break;

			case GameObjectActionType.Show:
				Target.gameObject.SetActive(true);
				break;

			case GameObjectActionType.DisableScript:
				Target.enabled = false;
				break;

			case GameObjectActionType.EnableScript:
				Target.enabled = true;
				break;

			case GameObjectActionType.Destroy:
				Destroy(Target.gameObject);
				break;

			case GameObjectActionType.PrintLog:
				print(Target.name);
				break;
		}
	}
}