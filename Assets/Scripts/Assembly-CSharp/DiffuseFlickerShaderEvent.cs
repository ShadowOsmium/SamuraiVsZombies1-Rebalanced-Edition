using UnityEngine;

public class DiffuseFlickerShaderEvent : ShaderEvent
{
	private enum FadeState
	{
		eFadeIn = 0,
		eHold = 1,
		eFadeOut = 2
	}

	private float mEffectDuration;

	private Color mOriginalColor;

	private Color mTargetColor;

	private Color mCurrentColor;

	private FadeInOutEvent mFadeEvent;

	public DiffuseFlickerShaderEvent(GameObject obj, Color targetColor, float fadeInTime, float holdTime, float fadeOutTime, float effectDuration)
		: base(obj)
	{
		mTargetColor = targetColor;
		mFadeEvent = new FadeInOutEvent(fadeInTime, holdTime, fadeOutTime);
		mEffectDuration = effectDuration;
	}

	public override void resetToBaseValues()
	{
		SetAllMaterialsToColor(Color.white);
	}

	public override void update()
	{
		base.update();
		mFadeEvent.update();
		mEffectDuration -= Time.deltaTime;
		if (base.shouldDie || mEffectDuration < 0f)
		{
			base.shouldDie = true;
			return;
		}
		if (mFadeEvent.isComplete)
		{
			mFadeEvent.Reset();
		}
		Color currentObjectColor = GetCurrentObjectColor();
		Color allMaterialsToColor = Color.Lerp(currentObjectColor, mTargetColor, mFadeEvent.interpolant);
		SetAllMaterialsToColor(allMaterialsToColor);
	}
}
