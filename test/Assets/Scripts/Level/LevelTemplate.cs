using UnityEngine;

[CreateAssetMenu(fileName = "LevelTemplate", menuName = "Level Template")]
public class LevelTemplate : ScriptableObject
{
	[SerializeField]
	public GameObject DefaultPrefab;

	public GameObject[] DefaultPrefabs { get; set; }

	[SerializeField]
	public GameObject[] TrapPrefabs;

	[SerializeField]
	public GameObject[] GapPrefabs;

	[SerializeField]
	public GameObject[] StartPrefabs;

	[SerializeField]
	public GameObject[] StarPrefabs;

	[SerializeField]
	public GameObject[] CoinPrefabs;

	[SerializeField]
	public Vector3 PrefabPosition;

	public GameObject[] LevelGameObjects { get; set; }

	public Vector3 LevelLength { get; set; }
}
