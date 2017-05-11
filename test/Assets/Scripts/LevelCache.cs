using UnityEngine;

[CreateAssetMenu(fileName = "LevelCache", menuName = "Level Cache")]
public class LevelCache : ScriptableObject
{
	[SerializeField]
	public GameObject[] Prefabs;

	[SerializeField]
	public GameObject[] DefaultPrefabs;

	[SerializeField]
	public Vector3 PrefabPosition;
}
