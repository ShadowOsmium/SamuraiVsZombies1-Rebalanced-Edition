using UnityEngine;

public class ParAttractorBehavior : MonoBehaviour
{
	private int x;

	private Vector3 gVector;

	private Vector3 pVector;

	public float attractionForce = 0.1f;

	public Vector3 AttractorLoc;

	public bool UseWorldSpace;

	public float minExclusion = 0.1f;

	private void Start()
	{
	}

	private void Update()
	{
		Particle[] particles = base.GetComponent<ParticleEmitter>().particles;
		if (UseWorldSpace)
		{
			pVector = AttractorLoc;
		}
		else
		{
			pVector = base.transform.position - AttractorLoc;
		}
		x = 0;
		Particle[] array = particles;
		foreach (Particle particle in array)
		{
			gVector = particle.position - pVector;
			if (gVector.sqrMagnitude > minExclusion * minExclusion)
			{
				gVector *= attractionForce / gVector.sqrMagnitude;
				particles[x].velocity -= gVector;
			}
			x++;
		}
		base.GetComponent<ParticleEmitter>().particles = particles;
	}
}
