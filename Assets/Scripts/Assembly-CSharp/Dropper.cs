using UnityEngine;

public class Dropper : MonoBehaviour
{
	private Transform myTransform;

	public GameObject ball;

	private void Start()
	{
		myTransform = base.transform;
	}

	private void Update()
	{
		if (Input.anyKeyDown)
		{
			Object.Instantiate(ball, new Vector3(myTransform.position.x, myTransform.position.y, -0.6f), Quaternion.identity);
		}
	}
}
