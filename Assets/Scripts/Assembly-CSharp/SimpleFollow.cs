using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
	public Transform target;

	public float smooth = 0.3f;

	public float distance = 25f;

	public float heightOffset = 10f;

	private float yVelocity;

	private void Update()
	{
		float y = Mathf.SmoothDampAngle(base.transform.eulerAngles.y, target.eulerAngles.y, ref yVelocity, smooth);
		Vector3 position = target.position;
		position += Quaternion.Euler(0f, y, 0f) * new Vector3(0f, 0f, 0f - distance);
		position.y = target.position.y + heightOffset;
		base.transform.position = position;
		base.transform.LookAt(target);
	}
}
