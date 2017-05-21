using UnityEngine;

[CreateAssetMenu(fileName = "LevelTemplate", menuName = "Ethan Runner/Level Template")]
public class LevelTemplate : ScriptableObject
{
	public GameObject DefaultPrefab;

	public GameObject[] TrapPrefabs;

	public GameObject[] GapPrefabs;

	public GameObject[] StartPrefabs;

	public GameObject[] StarPrefabs;

	public GameObject[] CoinPrefabs;

	public Vector3 PrefabPosition;

	public Vector3 LevelLength { get; set; }

	public GameObject[] LevelGameObjects { get; set; }

	public GameObject[] DefaultPrefabs { get; set; }
}
