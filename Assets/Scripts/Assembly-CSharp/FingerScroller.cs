using UnityEngine;

public class FingerScroller
{
	private const int kVelocityHistoryNum = 3;

	private const float kScrollSlowDownFactor = 0.1f;

	private Vector2 kInvalid = new Vector2(-9999f, -9999f);

	private float kScrollTreshold = 12f;

	private Vector2 mScrollPosition;

	private Rect mArea;

	private float mExtraTouchBorder;

	private Vector2 mTouchStart;

	private bool mDragMode;

	private bool mUseOvershoot = true;

	private OnSUIVector2Callback mOnSimpleTouch;

	private Vector2 mMaxScroll;

	private int mSnapToGrid;

	private bool mTouchesEnabled = true;

	private float mBounceFactor;

	private Vector2[] mPreviousVelocities;

	private Vector2 mSwipeVelocity;

	private Vector2 mAutoScrollTarget;

	public Rect area
	{
		get
		{
			return mArea;
		}
		set
		{
			mArea = value;
		}
	}

	public float extraTouchBorder
	{
		get
		{
			return mExtraTouchBorder;
		}
		set
		{
			mExtraTouchBorder = value;
		}
	}

	public Vector2 scrollMax
	{
		get
		{
			return mMaxScroll;
		}
		set
		{
			mMaxScroll = new Vector2(Mathf.Max(0f, value.x), Mathf.Max(0f, value.y));
		}
	}

	public Vector2 scrollPosition
	{
		get
		{
			return mScrollPosition;
		}
		set
		{
			mScrollPosition = value;
		}
	}

	public OnSUIVector2Callback onSimpleTouch
	{
		get
		{
			return mOnSimpleTouch;
		}
		set
		{
			mOnSimpleTouch = value;
		}
	}

	public bool useOverShoot
	{
		get
		{
			return mUseOvershoot;
		}
		set
		{
			mUseOvershoot = value;
		}
	}

	public Vector2 visualScrollPosition
	{
		get
		{
			Vector2 result = mScrollPosition;
			if (result.x < 0f)
			{
				result.x /= 3f;
			}
			if (result.y < 0f)
			{
				result.y /= 3f;
			}
			if (result.x > mMaxScroll.x)
			{
				result.x = (result.x - mMaxScroll.x) / 3f + mMaxScroll.x;
			}
			if (result.y > mMaxScroll.y)
			{
				result.y = (result.y - mMaxScroll.y) / 3f + mMaxScroll.y;
			}
			return result;
		}
	}

	public int snapToGrid
	{
		get
		{
			return mSnapToGrid;
		}
		set
		{
			mSnapToGrid = value;
		}
	}

	public bool touchesEnabled
	{
		get
		{
			return mTouchesEnabled;
		}
		set
		{
			mTouchesEnabled = value;
		}
	}

	public FingerScroller()
	{
		if (SUIScreen.isDeviceRetina)
		{
			kScrollTreshold *= 2f;
		}
		mTouchStart = kInvalid;
		mAutoScrollTarget = kInvalid;
		mPreviousVelocities = new Vector2[3];
		ClearVelocityHistory();
		mScrollPosition.x = 5f;
	}

	public void Update()
	{
		mBounceFactor = Mathf.Clamp(1f - SUIScreen.deltaTime * 6f, 0f, 0.99f);
		if (mTouchesEnabled && WeakGlobalInstance<SUIScreen>.instance.inputs.isTouching)
		{
			Vector2 position = WeakGlobalInstance<SUIScreen>.instance.inputs.position;
			if (mTouchStart == kInvalid)
			{
				BeginTouch(position);
			}
			else
			{
				ContinueTouch(position);
			}
			return;
		}
		if (mTouchStart != kInvalid)
		{
			EndTouch();
		}
		if (mAutoScrollTarget != kInvalid)
		{
			UpdateAutoScroll();
			return;
		}
		UpdateAutoBouncing();
		if (mSnapToGrid != 0)
		{
			UpdateSnapTo();
		}
	}

	public void ScrollTo(Vector2 target)
	{
		mAutoScrollTarget = target;
	}

	private void ClearVelocityHistory()
	{
		for (int i = 0; i < 3; i++)
		{
			mPreviousVelocities[i] = kInvalid;
		}
	}

	private void BeginTouch(Vector2 touchPos)
	{
		Rect rect = new Rect(mArea.xMin - mExtraTouchBorder, mArea.yMin - mExtraTouchBorder, mArea.width + mExtraTouchBorder * 2f, mArea.height + mExtraTouchBorder * 2f);
		if (WeakGlobalInstance<SUIScreen>.instance.inputs.justTouched && rect.Contains(touchPos))
		{
			mTouchStart = touchPos;
			mDragMode = mOnSimpleTouch == null;
			ClearVelocityHistory();
		}
	}

	private void ContinueTouch(Vector2 pos)
	{
		mAutoScrollTarget = kInvalid;
		if (!mDragMode)
		{
			Vector2 vector = pos - mTouchStart;
			if (Mathf.Abs(vector.y) >= kScrollTreshold || Mathf.Abs(vector.x) >= kScrollTreshold)
			{
				mDragMode = true;
			}
		}
		if (mDragMode)
		{
			Vector2 vector2 = pos - mTouchStart;
			mScrollPosition -= vector2;
			if (!mUseOvershoot)
			{
				mScrollPosition = SnapToMax(mScrollPosition);
			}
			mTouchStart = pos;
			for (int i = 0; i < 2; i++)
			{
				mPreviousVelocities[i + 1] = mPreviousVelocities[i];
			}
			mPreviousVelocities[0] = new Vector2((0f - vector2.x) / SUIScreen.deltaTime, (0f - vector2.y) / SUIScreen.deltaTime);
		}
	}

	private void EndTouch()
	{
		mAutoScrollTarget = kInvalid;
		Vector2 v = mTouchStart;
		mTouchStart = kInvalid;
		if (mDragMode)
		{
			mSwipeVelocity = Vector2.zero;
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				if (mPreviousVelocities[i] != kInvalid)
				{
					num++;
					mSwipeVelocity += mPreviousVelocities[i];
				}
			}
			if (num > 0)
			{
				mSwipeVelocity.x /= num;
				mSwipeVelocity.y /= num;
			}
		}
		else if (mOnSimpleTouch != null)
		{
			mOnSimpleTouch(v);
		}
	}

	private void UpdateAutoScroll()
	{
		mSwipeVelocity = Vector2.zero;
		mScrollPosition.x = (mAutoScrollTarget.x - mScrollPosition.x) * SUIScreen.deltaTime + mScrollPosition.x;
		mScrollPosition.y = (mAutoScrollTarget.y - mScrollPosition.y) * SUIScreen.deltaTime + mScrollPosition.y;
		if (Mathf.Abs(mScrollPosition.x - mAutoScrollTarget.x) < 1f && Mathf.Abs(mScrollPosition.y - mAutoScrollTarget.y) < 1f)
		{
			mScrollPosition = mAutoScrollTarget;
			mAutoScrollTarget = kInvalid;
		}
	}

	private void UpdateAutoBouncing()
	{
		if (mScrollPosition.x < 0f)
		{
			mScrollPosition.x *= mBounceFactor;
			if (mScrollPosition.x > -1f)
			{
				mScrollPosition.x = 0f;
			}
		}
		if (mScrollPosition.y < 0f)
		{
			mScrollPosition.y *= mBounceFactor;
			if (mScrollPosition.y > -1f)
			{
				mScrollPosition.y = 0f;
			}
		}
		if (mScrollPosition.x > mMaxScroll.x)
		{
			mScrollPosition.x = (mScrollPosition.x - mMaxScroll.x) * mBounceFactor + mMaxScroll.x;
			if (mScrollPosition.x < mMaxScroll.x - 1f)
			{
				mScrollPosition.x = mMaxScroll.x;
			}
		}
		if (mScrollPosition.y > mMaxScroll.y)
		{
			mScrollPosition.y = (mScrollPosition.y - mMaxScroll.y) * mBounceFactor + mMaxScroll.y;
			if (mScrollPosition.y < mMaxScroll.y - 1f)
			{
				mScrollPosition.y = mMaxScroll.y;
			}
		}
		if (!mUseOvershoot)
		{
			mScrollPosition = SnapToMax(mScrollPosition);
		}
		if (mSwipeVelocity.x != 0f)
		{
			mScrollPosition.x += mSwipeVelocity.x * SUIScreen.deltaTime;
			mSwipeVelocity.x *= Mathf.Clamp(1f - SUIScreen.deltaTime / 0.1f, 0f, 1f);
		}
		if (mSwipeVelocity.y != 0f)
		{
			mScrollPosition.y += mSwipeVelocity.y * SUIScreen.deltaTime;
			mSwipeVelocity.y *= Mathf.Clamp(1f - SUIScreen.deltaTime / 0.1f, 0f, 1f);
		}
	}

	private void UpdateSnapTo()
	{
		mScrollPosition.x = SnapToGrid(mScrollPosition.x, mMaxScroll.x);
		mScrollPosition.y = SnapToGrid(mScrollPosition.y, mMaxScroll.y);
	}

	private float SnapToGrid(float val, float max)
	{
		if (val <= 0f || val >= max)
		{
			return val;
		}
		int num = (int)val % mSnapToGrid;
		if (num < mSnapToGrid / 2)
		{
			return mSnapToGrid * ((int)val / mSnapToGrid);
		}
		return mSnapToGrid * ((int)val / mSnapToGrid + 1);
	}

	private Vector2 SnapToMax(Vector2 pos)
	{
		Vector2 result = pos;
		result.x = Mathf.Clamp(result.x, 0f, mMaxScroll.x);
		result.y = Mathf.Clamp(result.y, 0f, mMaxScroll.y);
		return result;
	}
}
