using UnityEngine;

public class ParSelfAttractBehavior : MonoBehaviour
{
	private int x;

	private Vector3 gVector;

	public float attractionForce = 0.001f;

	public float minExclusion = 0.001f;

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
			Particle[] array2 = particles;
			foreach (Particle particle2 in array2)
			{
				gVector = particle.position - particle2.position;
				if (gVector.sqrMagnitude > minExclusion * minExclusion)
				{
					gVector *= attractionForce / gVector.sqrMagnitude;
					particles[x].velocity -= gVector;
				}
			}
			x++;
		}
		base.GetComponent<ParticleEmitter>().particles = particles;
	}
}
