public class WinLoseDisplay : IDialog
{
	private const float kAnimSpeed = 1f;

	private SUILayout mLayout;

	private OnSUIGenericCallback mOnTrigger;

	private int mSkipNumUpdate = 2;

	public OnSUIGenericCallback onPlayerPressed
	{
		get
		{
			return mOnTrigger;
		}
		set
		{
			mOnTrigger = value;
		}
	}

	public bool isDone
	{
		get
		{
			return false;
		}
	}

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public WinLoseDisplay(string layout)
	{
		mLayout = new SUILayout(layout);
		mLayout.AnimateIn(1f);
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
		mOnTrigger = null;
	}

	public void Update()
	{
		if (mSkipNumUpdate > 0)
		{
			mSkipNumUpdate--;
			return;
		}
		mLayout.Update();
		if (!mLayout.isAnimating && ((SUITouchArea)mLayout["trigger"]).onAreaTouched == null)
		{
			((SUITouchArea)mLayout["trigger"]).onAreaTouched = onTriggered;
		}
	}

	private void onTriggered()
	{
		if (mOnTrigger != null)
		{
			mOnTrigger();
		}
	}
}
