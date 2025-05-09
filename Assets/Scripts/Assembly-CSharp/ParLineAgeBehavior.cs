using System.Collections.Generic;
using UnityEngine;

public class ParLineAgeBehavior : MonoBehaviour
{
	private int x;

	private void Start()
	{
	}

	private void Update()
	{
		Particle[] particles = base.GetComponent<ParticleEmitter>().particles;
		List<Particle> list = new List<Particle>();
		list.AddRange(particles);
		list.Sort((Particle thing1, Particle thing2) => thing1.energy.CompareTo(thing2.energy));
		LineRenderer component = GetComponent<LineRenderer>();
		component.SetVertexCount(particles.Length);
		x = 0;
		foreach (Particle item in list)
		{
			component.SetPosition(x, item.position);
			x++;
		}
	}
}
