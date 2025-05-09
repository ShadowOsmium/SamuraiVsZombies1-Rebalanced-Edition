using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GripPolyTrail : MonoBehaviour
{
	private struct PolyTrailSection
	{
		public Vector3 originalPoint;

		public Vector3 pointA;

		public Vector3 pointB;

		public float time;
	}

	public Transform[] AttachPoint = new Transform[2];

	public Color startColor;

	public Color endColor;

	public float TrailTime = 2f;

	public float DistanceBetweenVerts = 0.1f;

	public bool ShouldAddSections;

	private List<PolyTrailSection> mSections = new List<PolyTrailSection>();

	private void LateUpdate()
	{
		while (mSections.Count > 0 && Time.time > mSections[mSections.Count - 1].time + TrailTime)
		{
			mSections.RemoveAt(mSections.Count - 1);
		}
		if (ShouldAddSections && (mSections.Count == 0 || (mSections[0].originalPoint - base.transform.position).sqrMagnitude > DistanceBetweenVerts * DistanceBetweenVerts))
		{
			PolyTrailSection item = default(PolyTrailSection);
			item.originalPoint = base.transform.position;
			item.pointA = AttachPoint[0].position;
			item.pointB = AttachPoint[1].position;
			item.time = Time.time;
			mSections.Insert(0, item);
		}
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		if (mSections.Count < 2)
		{
			return;
		}
		Vector3[] array = new Vector3[mSections.Count * 2];
		Color[] array2 = new Color[mSections.Count * 2];
		Vector2[] array3 = new Vector2[mSections.Count * 2];
		PolyTrailSection polyTrailSection = mSections[0];
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		for (int i = 0; i < mSections.Count; i++)
		{
			polyTrailSection = mSections[i];
			float num = 0f;
			if (i != 0)
			{
				num = Mathf.Clamp01((Time.time - polyTrailSection.time) / TrailTime);
			}
			array[i * 2] = worldToLocalMatrix.MultiplyPoint(polyTrailSection.pointA);
			array[i * 2 + 1] = worldToLocalMatrix.MultiplyPoint(polyTrailSection.pointB);
			array3[i * 2] = new Vector2(num, 0f);
			array3[i * 2 + 1] = new Vector2(num, 1f);
			Color color = Color.Lerp(startColor, endColor, num);
			array2[i * 2] = color;
			array2[i * 2 + 1] = color;
		}
		int[] array4 = new int[(mSections.Count - 1) * 2 * 3];
		for (int j = 0; j < array4.Length / 6; j++)
		{
			array4[j * 6] = j * 2;
			array4[j * 6 + 1] = j * 2 + 1;
			array4[j * 6 + 2] = j * 2 + 2;
			array4[j * 6 + 3] = j * 2 + 2;
			array4[j * 6 + 4] = j * 2 + 1;
			array4[j * 6 + 5] = j * 2 + 3;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
	}
}
