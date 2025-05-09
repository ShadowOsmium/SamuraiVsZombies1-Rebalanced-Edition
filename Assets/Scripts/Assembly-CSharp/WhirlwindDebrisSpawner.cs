using UnityEngine;

public class WhirlwindDebrisSpawner : MonoBehaviour
{
	public float SpawnerLifeTime = 3f;

	public float DebrisSpawnFrequency = 0.5f;

	public Vector3 DebrisMinForce = new Vector3(-10f, 0f, -10f);

	public Vector3 DebrisMaxForce = new Vector3(10f, 10f, 10f);

	public float DebrisMinTorque = 10f;

	public float DebrisMaxTorque = 100f;

	public float DebrisMinLifeTime = 3f;

	public float DebrisMaxLifeTime = 5f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private GameObject GetDebrisMesh()
	{
		return Resources.Load("Fx/Debris" + Random.Range(0, 2)) as GameObject;
	}
}
