using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Transform myTransform;

	private Transform cameraTransform;

	private void Start()
	{
		myTransform = base.transform;
		cameraTransform = Camera.main.transform;
	}

	private void LateUpdate()
	{
		if (cameraTransform == null)
		{
			cameraTransform = Camera.main.transform;
		}
		myTransform.LookAt(myTransform.position + cameraTransform.rotation * new Vector3(0f, 0f, 1f));
	}
}
