using System;
using UnityEngine;

public class Ease
{
	public delegate float Function(float ratio, float initial, float delta);

	public static float Linear(float ratio, float initial, float delta)
	{
		return initial + delta * ratio;
	}

	public static float SineIn(float ratio, float initial, float delta)
	{
		return (0f - delta) * Mathf.Cos(ratio * ((float)Math.PI / 2f)) + delta + initial;
	}

	public static float SineOut(float ratio, float initial, float delta)
	{
		return delta * Mathf.Sin(ratio * ((float)Math.PI / 2f)) + initial;
	}

	public static float SineInOut(float ratio, float initial, float delta)
	{
		return (0f - delta) / 2f * (Mathf.Cos(ratio * (float)Math.PI) - 1f) + initial;
	}

	public static float QuadIn(float ratio, float initial, float delta)
	{
		return delta * ratio * ratio + initial;
	}

	public static float QuadOut(float ratio, float initial, float delta)
	{
		return (0f - delta) * ratio * (ratio - 2f) + initial;
	}

	public static float QuadInOut(float ratio, float initial, float delta)
	{
		float num = ratio * 2f;
		if (num < 1f)
		{
			return delta / 2f * (num * num) + initial;
		}
		num -= 1f;
		return (0f - delta) / 2f * ((num - 2f) * num - 1f) + initial;
	}

	public static float CubicIn(float ratio, float initial, float delta)
	{
		return delta * (ratio * ratio) * ratio + initial;
	}

	public static float CubicOut(float ratio, float initial, float delta)
	{
		float num = ratio - 1f;
		return delta * (num * num * num + 1f) + initial;
	}

	public static float CubicInOut(float ratio, float initial, float delta)
	{
		float num = ratio * 2f;
		if (num < 1f)
		{
			return delta / 2f * (num * num) * num + initial;
		}
		num -= 2f;
		return delta / 2f * (num * num * num + 2f) + initial;
	}

	public static float QuartIn(float ratio, float initial, float delta)
	{
		return delta * (ratio * ratio) * (ratio * ratio) + initial;
	}

	public static float QuartOut(float ratio, float initial, float delta)
	{
		float num = ratio - 1f;
		return (0f - delta) * (num * num * (num * num) - 1f) + initial;
	}

	public static float QuartInOut(float ratio, float initial, float delta)
	{
		float num = ratio * 2f;
		if (num < 1f)
		{
			return delta / 2f * (num * num) * (num * num) + initial;
		}
		num -= 2f;
		return (0f - delta) / 2f * (num * num * (num * num) - 2f) + initial;
	}

	public static float QuintIn(float ratio, float initial, float delta)
	{
		return delta * (ratio * ratio) * (ratio * ratio) * ratio + initial;
	}

	public static float QuintOut(float ratio, float initial, float delta)
	{
		float num = ratio - 1f;
		return delta * (num * num * (num * num) * num + 1f) + initial;
	}

	public static float QuintInOut(float ratio, float initial, float delta)
	{
		float num = ratio * 2f;
		if (num < 1f)
		{
			return delta / 2f * (num * num) * (num * num) * num + initial;
		}
		num -= 2f;
		return delta / 2f * (num * num * (num * num) * num + 2f) + initial;
	}

	public static float ExpoIn(float ratio, float initial, float delta)
	{
		if (ratio == 0f)
		{
			return initial;
		}
		return delta * Mathf.Pow(10f * (ratio - 1f), 2f) + initial;
	}

	public static float ExpoOut(float ratio, float initial, float delta)
	{
		if (ratio == 1f)
		{
			return initial + delta;
		}
		return delta * (0f - Mathf.Pow(-10f * ratio, 2f) + 1f) + initial;
	}

	public static float ExpoInOut(float ratio, float initial, float delta)
	{
		if (ratio == 0f)
		{
			return initial;
		}
		if (ratio == 1f)
		{
			return initial + delta;
		}
		float num = ratio * 2f;
		if (num < 1f)
		{
			return delta / 2f * Mathf.Pow(10f * (num - 1f), 2f) + initial;
		}
		num -= 1f;
		return delta / 2f * (0f - Mathf.Pow(-10f * num, 2f) + 2f) + initial;
	}

	public static float CircIn(float ratio, float initial, float delta)
	{
		return (0f - delta) * (Mathf.Sqrt(1f - ratio * ratio) - 1f) + initial;
	}

	public static float CircOut(float ratio, float initial, float delta)
	{
		float num = ratio - 1f;
		return delta * Mathf.Sqrt(1f - num * num) + initial;
	}

	public static float CircInOut(float ratio, float initial, float delta)
	{
		float num = ratio * 2f;
		if (num < 1f)
		{
			return (0f - delta) / 2f * (Mathf.Sqrt(1f - num * num) - 1f) + initial;
		}
		num -= 2f;
		return delta / 2f * (Mathf.Sqrt(1f - num * num) + 1f) + initial;
	}

	public static float BackIn(float ratio, float initial, float delta)
	{
		return BackIn(ratio, initial, delta, 1.70158f);
	}

	public static float BackIn(float ratio, float initial, float delta, float overshoot)
	{
		return delta * ratio * ratio * ((overshoot + 1f) * ratio - overshoot) + initial;
	}

	public static float BackOut(float ratio, float initial, float delta)
	{
		return BackOut(ratio, initial, delta, 1.70158f);
	}

	public static float BackOut(float ratio, float initial, float delta, float overshoot)
	{
		float num = ratio - 1f;
		return delta * (num * num * ((overshoot + 1f) * num + overshoot) + 1f) + initial;
	}

	public static float BackInOut(float ratio, float initial, float delta)
	{
		return BackInOut(ratio, initial, delta, 1.70158f);
	}

	public static float BackInOut(float ratio, float initial, float delta, float overshoot)
	{
		overshoot *= 1.525f;
		float num = ratio * 2f;
		if (num < 1f)
		{
			return delta / 2f * (num * num * ((overshoot + 1f) * num - overshoot)) + initial;
		}
		num -= 2f;
		return delta / 2f * (num * num * ((overshoot + 1f) * num + overshoot) + 2f) + initial;
	}

	public static float BounceIn(float ratio, float initial, float delta)
	{
		return delta - BounceOut(1f - ratio, 0f, delta) + initial;
	}

	public static float BounceOut(float ratio, float initial, float delta)
	{
		if (ratio < 0.36363637f)
		{
			return delta * (7.5625f * ratio * ratio) + initial;
		}
		if (ratio < 0.72727275f)
		{
			float num = ratio - 0.54545456f;
			return delta * (7.5625f * num * num + 0.75f) + initial;
		}
		if (ratio < 0.90909094f)
		{
			float num2 = ratio - 0.8181818f;
			return delta * (7.5625f * num2 * num2 + 0.9375f) + initial;
		}
		float num3 = ratio - 21f / 22f;
		return delta * (7.5625f * num3 * num3 + 63f / 64f) + initial;
	}

	public static float BounceInOut(float ratio, float initial, float delta)
	{
		if (ratio < 0.5f)
		{
			return BounceIn(ratio * 2f, 0f, delta) / 2f + initial;
		}
		return BounceOut(ratio * 2f - 1f, 0f, delta) / 2f + delta / 2f + initial;
	}
}
