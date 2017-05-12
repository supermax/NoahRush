using TMS.Common.Core;
using TMS.Common.Extensions;
using UnityEngine;

public class LevelManager : ViewModel
{
	[SerializeField] public uint PoolSize = 2;

	[SerializeField] public LevelTemplate Template;

	public LevelBuilder[] LevelBuilders { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		if (PoolSize < 1) PoolSize = 1;
		LevelBuilders = new LevelBuilder[PoolSize];

		Subscribe<PlayerMovePayload>(OnPlayerMove);
	}

	private void OnPlayerMove(PlayerMovePayload payload)
	{
		
	}

	protected override void Start()
	{
		base.Start();

		if (LevelBuilders.IsNullOrEmpty())
		{
			Debug.LogError("Level Builders are NULL\\Empty");
			return;
		}

		LevelBuilder prevBuilder = null;

		for (var i = 0; i < LevelBuilders.Length; i++)
		{
			var go = new GameObject("LevelPart_" + (i + 1));
			go.transform.parent = transform;

			if (prevBuilder != null)
			{
				go.SetActive(false);
				go.transform.position = prevBuilder.LevelPool.LevelLength;
				//go.transform.localPosition = prevBuilder.LevelPool.LevelLength;
			}

			var builder = go.AddComponent<LevelBuilder>();
			builder.Prefabs = Template;
			builder.Init();

			LevelBuilders[i] = builder;
			prevBuilder = builder;
		}
	}
}