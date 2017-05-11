using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMS.Common.Extensions;

public class LevelRandomizer : TMS.Common.Core.MonoBehaviourBase
{
	[SerializeField] public LevelCache Prefabs;

	private LevelCache _cache;

	protected override void Start()
	{
		base.Start();

		_cache = ScriptableObject.CreateInstance<LevelCache>();
		_cache.Prefabs = Clone(Prefabs.Prefabs, gameObject.transform);
		_cache.DefaultPrefabs = Clone(Prefabs.DefaultPrefabs, gameObject.transform);
		_cache.PrefabPosition = Prefabs.PrefabPosition;

		var lastPos = Arrange(_cache.DefaultPrefabs, gameObject.transform.position, _cache.PrefabPosition);
		Arrange(_cache.Prefabs, lastPos, _cache.PrefabPosition);

		//var prefabs = Randomize(_prefabs);
		//Arrange(prefabs);
	}

	private static GameObject[] Clone(IList<GameObject> prefabs, 
		Transform parentTransform)
	{
		var clonedPrefabs = new GameObject[prefabs.Count];
		for (var i = 0; i < prefabs.Count; i++)
		{
			clonedPrefabs[i] = (GameObject)Instantiate(prefabs[i], parentTransform);
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

	private static GameObject[] Randomize(GameObject[] prefabs)
	{
		if(prefabs.IsNullOrEmpty()) return null;

		//var lst = new List<GameObject>();
		//while (true)
		//{
			
		//}

		return prefabs;
	}
}
