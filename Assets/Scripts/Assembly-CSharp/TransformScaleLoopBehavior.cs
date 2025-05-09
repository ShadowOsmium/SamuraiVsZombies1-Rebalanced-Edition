using UnityEngine;

public class TransformScaleLoopBehavior : MonoBehaviour
{
	private float myTime;

	private float scaleEval;

	public float ScaleRate = 1f;

	public AnimationCurve ScaleCurve;

	private void Start()
	{
		ScaleCurve.preWrapMode = WrapMode.Loop;
		ScaleCurve.postWrapMode = WrapMode.Loop;
	}

	private void Update()
	{
		scaleEval = ScaleCurve.Evaluate(myTime);
		base.transform.localScale = new Vector3(scaleEval, scaleEval, scaleEval);
		myTime += ScaleRate * Time.deltaTime;
	}
}
