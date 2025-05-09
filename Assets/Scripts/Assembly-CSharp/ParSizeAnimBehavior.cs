using UnityEngine;

public class ParSizeAnimBehavior : MonoBehaviour
{
	private int x;

	private float lifePerc;

	public int maxParticles = 200;

	public float minSize = 0.5f;

	public float maxSize = 1.5f;

	public AnimationCurve SizeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	private float[] sizeArray;

	private void Start()
	{
		sizeArray = new float[maxParticles];
		for (x = 0; x < maxParticles; x++)
		{
			sizeArray[x] = Random.Range(minSize, maxSize);
		}
		base.GetComponent<ParticleEmitter>().minSize = 0f;
		base.GetComponent<ParticleEmitter>().maxSize = 0f;
	}

	private void Update()
	{
		Particle[] particles = base.GetComponent<ParticleEmitter>().particles;
		x = 0;
		Particle[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			Particle particle = array[i];
			lifePerc = 1f - particle.energy / particle.startEnergy;
			particles[x].size = sizeArray[x] * SizeCurve.Evaluate(lifePerc);
			x++;
		}
		base.GetComponent<ParticleEmitter>().particles = particles;
	}
}
