using UnityEngine;

[AddComponentMenu("Particles/PreloadParticles")]
[ExecuteInEditMode]
public class PreloadParticles : MonoBehaviour
{
	public float simulateTime;

	private void Awake()
	{
		ParticleEmitter particleEmitter = base.GetComponent<ParticleEmitter>();
		if (!(particleEmitter == null))
		{
			float num = Mathf.Max(Time.deltaTime, 0.01f);
			for (float num2 = simulateTime; num2 > 0f; num2 -= num)
			{
				particleEmitter.Simulate(Mathf.Min(num2, num));
			}
		}
	}
}
