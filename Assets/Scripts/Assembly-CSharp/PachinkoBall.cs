using UnityEngine;

public class PachinkoBall : MonoBehaviour
{
	private const float kMax_Rest_Time = 2f;

	private float mAtRestTime;

	private Transform myTransform;

	private float originalZ;

	private void Start()
	{
		myTransform = base.transform;
		originalZ = myTransform.position.z;
	}

	private void Update()
	{
		if (myTransform.position.z != originalZ)
		{
			myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, originalZ);
		}
	}

	private void FixedUpdate()
	{
		if (base.GetComponent<Rigidbody>().IsSleeping())
		{
			mAtRestTime += Time.deltaTime;
			if (mAtRestTime >= 2f)
			{
				Singleton<Profile>.instance.pachinkoBalls++;
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			mAtRestTime = 0f;
		}
	}
}
