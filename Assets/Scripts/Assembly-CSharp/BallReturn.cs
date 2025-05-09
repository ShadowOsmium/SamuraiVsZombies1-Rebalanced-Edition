using UnityEngine;

public class BallReturn : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Object.Destroy(other.gameObject);
		Singleton<Profile>.instance.pachinkoBalls++;
	}
}
