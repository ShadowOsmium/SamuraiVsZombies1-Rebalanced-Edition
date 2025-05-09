using UnityEngine;

public class TransformAsyncMoveBehavior : MonoBehaviour
{
	private Vector3 baseTransform;

	private float myTimeX;

	private float myTimeY;

	private float myTimeZ;

	public float rateX = 1f;

	public AnimationCurve TranslateX;

	public float rateY = 1f;

	public AnimationCurve TranslateY;

	public float rateZ = 1f;

	public AnimationCurve TranslateZ;

	private void Start()
	{
		TranslateX.preWrapMode = WrapMode.Loop;
		TranslateY.preWrapMode = WrapMode.Loop;
		TranslateZ.preWrapMode = WrapMode.Loop;
		TranslateX.postWrapMode = WrapMode.Loop;
		TranslateY.postWrapMode = WrapMode.Loop;
		TranslateZ.postWrapMode = WrapMode.Loop;
		baseTransform = base.transform.localPosition;
	}

	private void Update()
	{
		base.transform.localPosition = baseTransform + new Vector3(TranslateX.Evaluate(myTimeX * rateX), TranslateY.Evaluate(myTimeY * rateY), TranslateZ.Evaluate(myTimeZ * rateZ));
		myTimeX += rateX * Time.deltaTime;
		myTimeY += rateY * Time.deltaTime;
		myTimeZ += rateZ * Time.deltaTime;
	}
}
