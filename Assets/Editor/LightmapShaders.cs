using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LightmapShaders : Editor
{
	[MenuItem("Lightmap Shaders/Search")]
	public static void Search()
	{
		foreach (MeshRenderer renderer in FindObjectsOfType<MeshRenderer>())
		{
			if (renderer.isPartOfStaticBatch)
			{
				foreach (Material material in renderer.sharedMaterials)
				{
					Debug.LogError(material.shader.name);
				}
			}
		}
	}
}
