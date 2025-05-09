using UnityEngine;

internal class NarrativeDialog : IDialog
{
	private const float kMinTimeBeforeTouch = 2f;

	private SUILayout mLayout;

	private bool mDone;

	private float mDismissTimer;

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public bool isDone
	{
		get
		{
			return mDone && !mLayout.isAnimating;
		}
	}

	public NarrativeDialog(string imageFile)
	{
		mLayout = new SUILayout("Layouts/NarrativeDialog");
		((SUISprite)mLayout["image"]).texture = imageFile;
		((SUITouchArea)mLayout["touchArea"]).onAreaTouched = OnScreenTouched;
		mLayout.basePriority = 600f;
		mLayout.AnimateIn();
	}

	public void Destroy()
	{
		if (mLayout != null)
		{
			mLayout.Destroy();
			mLayout = null;
		}
	}

	public void Update()
	{
		mDismissTimer += SUIScreen.deltaTime;
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Dismiss();
		}
	}

	private void OnScreenTouched()
	{
		if (mDismissTimer > 2f)
		{
			Dismiss();
		}
	}

	private void Dismiss()
	{
		((SUITouchArea)mLayout["touchArea"]).onAreaTouched = null;
		mDone = true;
		mLayout.AnimateOut();
	}
}
