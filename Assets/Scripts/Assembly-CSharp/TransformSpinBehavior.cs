using UnityEngine;

public class TransformSpinBehavior : MonoBehaviour
{
	public Vector3 RotationRate;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(RotationRate * Time.deltaTime);
	}
}
