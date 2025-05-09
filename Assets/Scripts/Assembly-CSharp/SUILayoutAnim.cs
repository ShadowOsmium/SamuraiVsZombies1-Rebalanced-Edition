using UnityEngine;

public class SUILayoutAnim
{
	public class AnimFloat
	{
		private float val_in;

		private float val_out;

		private Ease.Function animFunc;

		private SUILayout.NormalRange range;

		public AnimFloat(float _in, float _out, SUILayout.NormalRange _range, Ease.Function anim)
		{
			val_in = _in;
			val_out = _out;
			animFunc = anim;
			range = _range;
		}

		public float getAt(float prog)
		{
			if (animFunc != null)
			{
				prog = toDelayAdjusted(prog, range);
				return animFunc(prog, val_out, val_in - val_out);
			}
			return val_in;
		}
	}

	public class AnimVector2
	{
		private Vector2 val_in;

		private Vector2 val_out;

		private Ease.Function animFunc;

		private SUILayout.NormalRange range;

		public AnimVector2(Vector2 _in, Vector2 _out, SUILayout.NormalRange _range, Ease.Function anim)
		{
			val_in = _in;
			val_out = _out;
			animFunc = anim;
			range = _range;
		}

		public Vector2 getAt(float prog)
		{
			if (animFunc != null)
			{
				prog = toDelayAdjusted(prog, range);
				return new Vector2(animFunc(prog, val_out.x, val_in.x - val_out.x), animFunc(prog, val_out.y, val_in.y - val_out.y));
			}
			return val_in;
		}
	}

	private static float toDelayAdjusted(float prog, SUILayout.NormalRange range)
	{
		float num = 1f - (range.max - range.min);
		if (num == 0f)
		{
			return prog;
		}
		if (prog <= range.min)
		{
			return 0f;
		}
		if (prog >= range.max)
		{
			return 1f;
		}
		return (prog - range.min) / (1f - num);
	}
}
