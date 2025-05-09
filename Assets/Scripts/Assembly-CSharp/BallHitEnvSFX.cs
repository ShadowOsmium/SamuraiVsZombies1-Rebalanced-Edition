using UnityEngine;

public class BallHitEnvSFX : MonoBehaviour
{
	public string ImpactEvent;

	private void Start()
	{
	}

	public void OnCollisionEnter(Collision collision)
	{
		GetComponent<SoundThemePlayer>().PlaySoundEvent(ImpactEvent);
	}

	private void Update()
	{
	}
}
