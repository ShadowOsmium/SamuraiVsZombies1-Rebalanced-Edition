using UnityEngine;

public class SUILayoutEffect
{
	public class Effect
	{
		private TypedWeakReference<IHasVisualAttributes> mTarget;

		public IHasVisualAttributes target
		{
			get
			{
				return mTarget.ptr;
			}
		}

		public Effect(IHasVisualAttributes _target)
		{
			mTarget = new TypedWeakReference<IHasVisualAttributes>(_target);
		}

		public virtual void Update()
		{
		}
	}

	public class AlphaPingPong : Effect
	{
		private float mMin;

		private float mMax;

		private float mSpeed;

		private float mTweenDir = 1f;

		private float mTimer;

		public AlphaPingPong(IHasVisualAttributes _target, float min, float max, float speed)
			: base(_target)
		{
			mMin = min;
			mMax = max;
			mSpeed = speed;
			base.target.alpha = mMin;
		}

		public override void Update()
		{
			mTimer += SUIScreen.deltaTime / mSpeed;
			if (mTimer > 1f)
			{
				mTimer -= 1f;
				mTweenDir = 0f - mTweenDir;
			}
			if (mTweenDir == 1f)
			{
				base.target.alpha = Ease.Linear(mTimer, mMin, mMax - mMin);
			}
			else
			{
				base.target.alpha = Ease.Linear(1f - mTimer, mMin, mMax - mMin);
			}
		}
	}

	public class ScalePingPong : Effect
	{
		private float mMin;

		private float mMax;

		private float mSpeed;

		private float mTweenDir = 1f;

		private float mTimer;

		public ScalePingPong(IHasVisualAttributes _target, float min, float max, float speed)
			: base(_target)
		{
			mMin = min;
			mMax = max;
			mSpeed = speed;
			base.target.scale = new Vector2(mMin, mMin);
		}

		public override void Update()
		{
			mTimer += SUIScreen.deltaTime / mSpeed;
			if (mTimer > 1f)
			{
				mTimer -= 1f;
				mTweenDir = 0f - mTweenDir;
			}
			float num = 0f;
			num = ((mTweenDir != 1f) ? Ease.Linear(1f - mTimer, mMin, mMax - mMin) : Ease.Linear(mTimer, mMin, mMax - mMin));
			base.target.scale = new Vector2(num, num);
		}
	}

	public static Effect CreateEffect(IHasVisualAttributes _target, string effectID, SDFTreeNode data)
	{
		switch (effectID.ToLower())
		{
		case "alphapingpong":
			if (data["min"] == string.Empty || data["max"] == string.Empty || data["speed"] == string.Empty)
			{
				Debug.Log("ERROR: Missing effect attributes (min, max or speed)");
				return null;
			}
			return new AlphaPingPong(_target, float.Parse(data["min"]), float.Parse(data["max"]), float.Parse(data["speed"]));
		case "scalepingpong":
			if (data["min"] == string.Empty || data["max"] == string.Empty || data["speed"] == string.Empty)
			{
				Debug.Log("ERROR: Missing effect attributes (min, max or speed)");
				return null;
			}
			return new ScalePingPong(_target, float.Parse(data["min"]), float.Parse(data["max"]), float.Parse(data["speed"]));
		default:
			Debug.Log("ERROR: Unknown Layout effect: " + effectID);
			return null;
		}
	}
}
