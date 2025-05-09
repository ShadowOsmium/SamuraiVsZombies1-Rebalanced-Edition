using System.Collections.Generic;
using UnityEngine;

public class RouteFollower : MonoBehaviour
{
	public Transform route;

	public float closeEnoughDistance = 5f;

	public int startIndex;

	public bool ignoreY;

	private int currentTargetIndex;

	private List<Vector3> mRoutePoints;

	private void Start()
	{
		mRoutePoints = new List<Vector3>();
		foreach (Transform item in route)
		{
			Vector3 position = item.position;
			if (ignoreY)
			{
				position.y = 0f;
			}
			mRoutePoints.Add(position);
		}
		currentTargetIndex = startIndex;
		if (currentTargetIndex < 0)
		{
			currentTargetIndex = 0;
		}
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		if (ignoreY)
		{
			position.y = 0f;
		}
		if (currentTargetIndex < mRoutePoints.Count && (position - mRoutePoints[currentTargetIndex]).magnitude < closeEnoughDistance)
		{
			currentTargetIndex++;
		}
	}

	public Vector3 targetDirection()
	{
		Vector3 position = base.transform.position;
		if (ignoreY)
		{
			position.y = 0f;
		}
		if (currentTargetIndex == mRoutePoints.Count)
		{
			return default(Vector3);
		}
		return (mRoutePoints[currentTargetIndex] - position).normalized;
	}
}
