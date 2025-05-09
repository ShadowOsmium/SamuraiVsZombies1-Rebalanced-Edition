using UnityEngine;

public class BallDropSFX : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		GetComponent<SoundThemePlayer>().PlaySoundEvent("BallDrop");
	}
}
