using UnityEngine;

public class PlayArea : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Object.Destroy(other.gameObject);
		Singleton<Profile>.instance.pachinkoBalls++;
	}
}
