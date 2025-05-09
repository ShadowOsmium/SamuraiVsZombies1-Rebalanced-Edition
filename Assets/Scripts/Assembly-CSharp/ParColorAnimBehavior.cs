using UnityEngine;

public class ParColorAnimBehavior : MonoBehaviour
{
	private int x;

	private float lifePerc;

	public AnimationCurve RCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve GCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve BCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve ACurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	private void Start()
	{
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
			particles[x].color = new Color(RCurve.Evaluate(lifePerc), GCurve.Evaluate(lifePerc), BCurve.Evaluate(lifePerc), ACurve.Evaluate(lifePerc));
			x++;
		}
		base.GetComponent<ParticleEmitter>().particles = particles;
	}
}
