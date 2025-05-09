using UnityEngine;

public class DiffuseFadeInOutShaderEvent : ShaderEvent
{
	private enum FadeState
	{
		eFadeIn = 0,
		eHold = 1,
		eFadeOut = 2
	}

	private Color mTargetColor;

	private FadeInOutEvent mFadeEvent;

	public DiffuseFadeInOutShaderEvent(GameObject obj, Color targetColor, float fadeInTime, float holdTime, float fadeOutTime)
		: base(obj)
	{
		mTargetColor = targetColor;
		mFadeEvent = new FadeInOutEvent(fadeInTime, holdTime, fadeOutTime);
	}

	public override void resetToBaseValues()
	{
		SetAllMaterialsToColor(Color.white);
	}

	public override void update()
	{
		base.update();
		mFadeEvent.update();
		if (base.shouldDie || mFadeEvent.isComplete)
		{
			base.shouldDie = true;
			return;
		}
		Color currentObjectColor = GetCurrentObjectColor();
		Color allMaterialsToColor = Color.Lerp(currentObjectColor, mTargetColor, mFadeEvent.interpolant);
		SetAllMaterialsToColor(allMaterialsToColor);
	}
}
