using UnityEngine;

[CreateAssetMenu(fileName = "LevelTemplate", menuName = "Level Template")]
public class LevelTemplate : ScriptableObject
{
	[SerializeField]
	public GameObject DefaultPrefab;

	internal GameObject[] DefaultPrefabs;

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
}
