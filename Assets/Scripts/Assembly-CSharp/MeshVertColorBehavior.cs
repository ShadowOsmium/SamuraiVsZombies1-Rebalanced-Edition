using UnityEngine;

public class MeshVertColorBehavior : MonoBehaviour
{
	public Color ColorRGBA;

	private void Start()
	{
	}

	private void Update()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Color[] array = new Color[mesh.vertices.Length];
		for (int i = 0; i < mesh.colors.Length; i++)
		{
			array[i] = ColorRGBA;
		}
		mesh.colors = array;
	}
}
