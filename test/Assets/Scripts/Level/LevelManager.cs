#region Usings

using TMS.Common.Core;
using TMS.Common.Extensions;
using UnityEngine;

#endregion

public class LevelManager : ViewModel
{
	private LevelBuilder _activeLevelBuilder;

	[SerializeField] public uint PoolSize = 2;

	[SerializeField] public LevelTemplate[] Templates;

	public LevelBuilder[] LevelBuilders { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		if (PoolSize < 2)
		{
			PoolSize = 2;
			Debug.LogError("Pool size MUST be larger then 1!");
		}

		LevelBuilders = new LevelBuilder[PoolSize];

		InitLevelBuilders();
	}

	private void OnPlayerMove(PlayerMovePayload payload)
	{
		//if (_activeLevelBuilder == null)
		//{
		//	Debug.LogError("Cannot get active Level Builder!");
		//	return;
		//}

		//var playerPos = payload.PlayerController.transform.position.z;
		//var levelLength = _activeLevelBuilder.LevelPool.LevelLength.z / 2f;
		//if (playerPos < levelLength) return;

		//RebuildLevel(_activeLevelBuilder);
	}

	private void RebuildLevel()
	{
		if (_activeLevelBuilder == null)
		{
			Debug.LogError("Cannot get active Level Builder!");
			return;
		}

		if (LevelBuilders.IsNullOrEmpty())
		{
			Debug.LogError("Level Builders are NULL\\Empty!");
			return;
		}

		if (LevelBuilders.Length < 2)
		{
			Debug.LogError("Level Builders must contain at least 2 items!");
			return;
		}

		var idx = LevelBuilders.GetIndexOf(_activeLevelBuilder);
		var nextIdx = idx == LevelBuilders.Length - 1 ? 0 : idx + 1;

		var nextBuilder = LevelBuilders[nextIdx];
		nextBuilder.gameObject.transform.position = new Vector3(0, 0, _activeLevelBuilder.LevelPool.LevelLength.z);
		nextBuilder.gameObject.SetActive(true);
		nextBuilder.BuildLevel(gameObject.activeInHierarchy);
	}

	protected override void Start()
	{
		base.Start();

		Subscribe<PlayerMovePayload>(OnPlayerMove);
		Subscribe<UIActionPayload>(OnUIAction);
		Subscribe<PlayerTriggerPayload>(OnPlayerTrigger);
	}

	private void OnPlayerTrigger(PlayerTriggerPayload payload)
	{
		switch (payload.TriggerSource.tag)
		{
			case GameObjectTagNames.DefaultTrack:
				var activeBuilder = payload.TriggerSource.GetComponentInParent<LevelBuilder>();
				payload.TriggerSource = null;

				if (activeBuilder == null || Equals(_activeLevelBuilder, activeBuilder)) return;

				_activeLevelBuilder = activeBuilder;
				print("Active Level: " + _activeLevelBuilder.name);

				RebuildLevel();
				break;
		}
	}

	private void OnUIAction(UIActionPayload payload)
	{
		switch (payload.Action)
		{
			case UIActionType.StartGame:
			case UIActionType.RestartGame:
				ResetLevel();
				break;

			case UIActionType.PauseGame:
				break;

			case UIActionType.ResumeGame:
				break;

			case UIActionType.QuitGame:
				break;

			case UIActionType.ShowSettings:
				break;
		}
	}

	private void ResetLevel()
	{
		// TODO
	}

	private LevelTemplate GetLevelTemplate(ref int index)
	{
		if(index < Templates.Length)
		{
			return Templates[index++];
		}

		index = 0;
		return GetLevelTemplate(ref index);
	}

	private void InitLevelBuilders()
	{
		if (LevelBuilders.IsNullOrEmpty())
		{
			Debug.LogError("Level Builders are NULL\\Empty!");
			return;
		}

		Subscribe<ScriptStateChangePayload<LevelBuilder>>(OnLevelBuilderStateChange);

		var templateIndex = 0;
		LevelBuilder prevBuilder = null;

		for (var i = 0; i < LevelBuilders.Length; i++)
		{
			var go = new GameObject("LevelPart_" + (i + 1));
			go.transform.parent = transform;

			if (prevBuilder != null)
			{
				go.SetActive(false);
				go.transform.position = new Vector3(0, 0, prevBuilder.LevelPool.LevelLength.z);
			}

			var builder = go.AddComponent<LevelBuilder>();
			builder.Prefabs = GetLevelTemplate(ref templateIndex);
			builder.InitPool();

			if (prevBuilder == null)
			{
				_activeLevelBuilder = builder;
				_activeLevelBuilder.BuildLevel();
			}

			LevelBuilders[i] = builder;
			prevBuilder = builder;
		}

		RebuildLevel();
	}

	private void OnLevelBuilderStateChange(ScriptStateChangePayload<LevelBuilder> payload)
	{
		//TODO

		//if (payload.State != ScriptStateType.Enabled) return;

		//_activeLevelBuilder = payload.Source;
		//Log("Active Level Builder: {0}", _activeLevelBuilder);
	}
}