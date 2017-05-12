using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Core;
using TMS.Common.Extensions;
using Object = UnityEngine.Object;

public class LevelBuilder : MonoBehaviourBase
{
	[SerializeField] public LevelTemplate Prefabs;

	public LevelTemplate LevelPool { get; private set; }

	public void Init()
	{
		InitPool();
		BuildLevel();
	}

	public void InitPool()
	{
		LevelPool = Clone(Prefabs, gameObject.transform);
	}

	public void BuildLevel()
	{
		BuildLevel(LevelPool, gameObject.transform.position);
	}

	private static void BuildLevel(LevelTemplate template, Vector3 initPos)
	{
		var startPrefabs = template.StartPrefabs.RandomShuffle();
		var gapPrefabs = template.GapPrefabs.RandomShuffle();
		var trapPrefabs = template.TrapPrefabs.RandomShuffle();

		var gapIdx = 0;
		var defIdx = 0;
		var levelPrefabs = new List<GameObject>(startPrefabs);
		foreach (var prefab in trapPrefabs)
		{
			levelPrefabs.Add(prefab);

			if (defIdx < template.DefaultPrefabs.Length)
			{
				levelPrefabs.Add(template.DefaultPrefabs[defIdx++]);
			}

			if(gapIdx >= gapPrefabs.GetLength()) continue;
			levelPrefabs.Add(gapPrefabs.ElementAt(gapIdx++));

			if (defIdx >= template.DefaultPrefabs.Length) continue;
			levelPrefabs.Add(template.DefaultPrefabs[defIdx++]);
		}

		if (gapIdx < gapPrefabs.GetLength() - 1)
		{
			for (; gapIdx < gapPrefabs.GetLength(); gapIdx++)
			{
				levelPrefabs.Add(gapPrefabs.ElementAt(gapIdx));
			}
		}

		if (defIdx < template.DefaultPrefabs.Length - 1)
		{
			for (; defIdx < template.DefaultPrefabs.Length; defIdx++)
			{
				levelPrefabs.Add(template.DefaultPrefabs[defIdx]);
			}
		}

		template.LevelGameObjects = levelPrefabs.ToArray();
		template.LevelLength = Arrange(levelPrefabs, initPos, template.PrefabPosition);
	}

	private static LevelTemplate Clone(LevelTemplate source, Transform parentTransform)
	{
		var clone = ScriptableObject.CreateInstance<LevelTemplate>();

		clone.TrapPrefabs = Clone(source.TrapPrefabs, parentTransform);
		clone.GapPrefabs = Clone(source.GapPrefabs, parentTransform);
		clone.StartPrefabs = Clone(source.StartPrefabs, parentTransform);
		clone.DefaultPrefabs = Clone(source.DefaultPrefab, 
			clone.TrapPrefabs.Length + clone.GapPrefabs.Length, parentTransform);

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

	private static GameObject[] Clone(Object prefab, int count,
		Transform parentTransform)
	{
		var clonedPrefabs = new GameObject[count];
		for (var i = 0; i < clonedPrefabs.Length; i++)
		{
			clonedPrefabs[i] = (GameObject)Instantiate(prefab, parentTransform);
		}
		return clonedPrefabs;
	}

	private static Vector3 Arrange(IEnumerable<GameObject> prefabs, 
		Vector3 initPos, Vector3 prefabPosition)
	{
		var prevPrefabPos = initPos;

		foreach (var prefab in prefabs)
		{
			prefab.transform.position = prevPrefabPos;
			prefab.transform.localPosition = prevPrefabPos;

			prevPrefabPos += prefabPosition;
		}

		return prevPrefabPos;
	}
}
