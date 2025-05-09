using UnityEngine;

public class GizmoRoute : MonoBehaviour
{
	public float pointSize = 3f;

	public bool show = true;

	private void OnDrawGizmos()
	{
		if (!show)
		{
			return;
		}
		Vector3 from = default(Vector3);
		bool flag = true;
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		foreach (Transform item in base.transform)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				Gizmos.DrawLine(from, item.position);
			}
			Gizmos.DrawCube(item.position, new Vector3(pointSize, pointSize, pointSize));
			from = item.position;
		}
	}
}
