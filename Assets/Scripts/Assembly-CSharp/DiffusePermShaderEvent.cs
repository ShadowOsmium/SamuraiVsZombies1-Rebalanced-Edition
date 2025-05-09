using UnityEngine;

public class DiffusePermShaderEvent : ShaderEvent
{
	private Color mTargetColor;

	public DiffusePermShaderEvent(GameObject obj, Color targetColor)
		: base(obj)
	{
		mTargetColor = targetColor;
	}

	public override void resetToBaseValues()
	{
		SetAllMaterialsToColor(Color.white);
	}

	public override void update()
	{
		base.update();
		if (!base.shouldDie)
		{
			SetAllMaterialsToColor(mTargetColor);
		}
	}
}
