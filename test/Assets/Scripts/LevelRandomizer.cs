﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Extensions;

public class LevelRandomizer : TMS.Common.Core.MonoBehaviourBase
{
	[SerializeField] public LevelTemplate Prefabs;

	private LevelTemplate _cache;

	protected override void Start()
	{
		base.Start();

		_cache = Clone(Prefabs, gameObject.transform);
		BuildLevel(_cache);
	}

	private void BuildLevel(LevelTemplate template)
	{
		var startPrefabs = template.StartPrefabs.RandomShuffle();
		var gapPrefabs = template.GapPrefabs.RandomShuffle();
		var trapPrefabs = _cache.TrapPrefabs.RandomShuffle();

		var gapIdx = 0;
		var levelPrefabs = new List<GameObject>(startPrefabs);
		foreach (var prefab in trapPrefabs)
		{
			levelPrefabs.Add(prefab);

			if(gapIdx >= gapPrefabs.GetLength()) continue;
			var gapPrefab = gapPrefabs.ElementAt(gapIdx++);
			levelPrefabs.Add(gapPrefab);
		}

		Arrange(levelPrefabs, gameObject.transform.position, _cache.PrefabPosition);
	}

	private static LevelTemplate Clone(LevelTemplate source, Transform parentTransform)
	{
		var clone = ScriptableObject.CreateInstance<LevelTemplate>();

		clone.TrapPrefabs = Clone(source.TrapPrefabs, parentTransform);
		clone.GapPrefabs = Clone(source.GapPrefabs, parentTransform);
		clone.StartPrefabs = Clone(source.StartPrefabs, parentTransform);

		clone.PrefabPosition = source.PrefabPosition;

		return clone;
	}

	private static GameObject[] Clone(IEnumerable<GameObject> prefabs, 
		Transform parentTransform)
	{
		var clonedPrefabs = new GameObject[prefabs.GetLength()];
		for (var i = 0; i < clonedPrefabs.Length; i++)
		{
			clonedPrefabs[i] = (GameObject)Instantiate(prefabs.GetElementAt(i), parentTransform);
		}
		return clonedPrefabs;
	}

	private static void Arrange(IEnumerable<GameObject> prefabs, 
		Vector3 initPos, Vector3 prefabPosition)
	{
		var prevPrefabPos = initPos;

		foreach (var prefab in prefabs)
		{
			prefab.transform.position = prevPrefabPos;
			prefab.transform.localPosition = prevPrefabPos;

			prevPrefabPos += prefabPosition;
		}
	}
}