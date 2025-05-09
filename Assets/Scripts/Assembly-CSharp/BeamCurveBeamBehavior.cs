using UnityEngine;

public class BeamCurveBeamBehavior : MonoBehaviour
{
	private int x;

	private float xF;

	private float segcF;

	private float myTime;

	private float segmentSpace = 1f;

	public float beamLength = 5f;

	public int segmentCount = 10;

	public AnimationCurve beamPattern = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	public AnimationCurve endClamping = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	public float beamRate = 1f;

	public float beamFrequency = 3f;

	private void Start()
	{
		beamPattern.preWrapMode = WrapMode.Loop;
		beamPattern.postWrapMode = WrapMode.Loop;
	}

	private void Update()
	{
		segcF = segmentCount;
		segmentSpace = beamLength / segcF;
		LineRenderer component = GetComponent<LineRenderer>();
		component.SetVertexCount(segmentCount);
		for (x = 0; x < segmentCount; x++)
		{
			xF = x;
			component.SetPosition(x, new Vector3(segmentSpace * xF, 0f, beamPattern.Evaluate(xF / segcF * beamFrequency + myTime) * endClamping.Evaluate(xF / segcF)));
		}
		myTime += beamRate * Time.deltaTime;
	}
}
