using UnityEngine;

[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Effects/SetRenderQueue")]
public class SetRenderQueue : MonoBehaviour
{
	public int queue = 1;

	public int[] queues;

	protected void Start()
	{
		if ((bool)base.GetComponent<Renderer>() && (bool)base.GetComponent<Renderer>().sharedMaterial && queues != null)
		{
			base.GetComponent<Renderer>().sharedMaterial.renderQueue = queue;
			for (int i = 0; i < queues.Length && i < base.GetComponent<Renderer>().sharedMaterials.Length; i++)
			{
				base.GetComponent<Renderer>().sharedMaterials[i].renderQueue = queues[i];
			}
		}
	}
}
