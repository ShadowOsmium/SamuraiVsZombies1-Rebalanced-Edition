using UnityEngine;

public class TutorialPopup : IDialog
{
	private const float kFadingSpeed = 0.15f;

	private SUILayout mLayout;

	private bool mVisible;

	private float mAlpha;

	private Vector2 mOriginalPanelPosition;

	private Vector2 mCurrentPanelPosition;

	private SUISprite mPanelRef;

	private SUILabel mTextRef;

	private SUISprite mCircleRef;

	private SUISprite mArrowLeftRef;

	private SUISprite mArrowRightRef;

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
			return false;
		}
		set
		{
		}
	}

	public bool visible
	{
		get
		{
			return mVisible;
		}
	}

	public float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			mAlpha = Mathf.Clamp(value, 0f, 1f);
			mPanelRef.alpha = mAlpha;
			mTextRef.alpha = mAlpha;
			mCircleRef.alpha = mAlpha;
			mArrowLeftRef.alpha = mAlpha;
			mArrowRightRef.alpha = mAlpha;
		}
	}

	public TutorialPopup()
	{
		mLayout = new SUILayout("Layouts/TutorialPopup");
		mPanelRef = (SUISprite)mLayout["panel"];
		mTextRef = (SUILabel)mLayout["text"];
		mCircleRef = (SUISprite)mLayout["circle"];
		mArrowLeftRef = (SUISprite)mLayout["arrowLeft"];
		mArrowRightRef = (SUISprite)mLayout["arrowRight"];
		mPanelRef.scale = new Vector2(1f, WeakGlobalInstance<SUIScreen>.instance.autoScaler.aspectRatioModifier);
		mTextRef.maxWidth = (int)((float)mTextRef.maxWidth * (1f / WeakGlobalInstance<SUIScreen>.instance.autoScaler.aspectRatioModifier));
		mOriginalPanelPosition = mPanelRef.position;
		mCurrentPanelPosition = mOriginalPanelPosition;
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
		if (mLayout != null && alpha > 0f)
		{
			mLayout.Update();
		}
		UpdateFading();
	}

	public void Hide()
	{
		mVisible = false;
	}

	public void ShowPanel(string text)
	{
		mVisible = true;
		alpha = 0f;
		mPanelRef.visible = true;
		mTextRef.text = text;
		mTextRef.visible = true;
		if (mCurrentPanelPosition != mOriginalPanelPosition)
		{
			SetPanelPosition(mOriginalPanelPosition);
		}
	}

	public void SetPanelPosition(Vector2 pos)
	{
		Vector2 vector = pos - mCurrentPanelPosition;
		mPanelRef.position += vector;
		mTextRef.position += vector;
		mCurrentPanelPosition = pos;
	}

	public void ShowLeftArrow(Vector2 pos)
	{
		mArrowLeftRef.position = pos;
		mArrowLeftRef.visible = true;
	}

	public void ShowRightArrow(Vector2 pos)
	{
		mArrowRightRef.position = pos;
		mArrowRightRef.visible = true;
	}

	public void ShowCircle(Vector2 pos)
	{
		mCircleRef.position = pos;
		mCircleRef.visible = true;
	}

	private void UpdateFading()
	{
		if (mVisible)
		{
			if (alpha < 1f)
			{
				alpha = Mathf.Min(1f, alpha + SUIScreen.deltaTime / 0.15f);
			}
		}
		else if (alpha > 0f)
		{
			alpha = Mathf.Max(0f, alpha - SUIScreen.deltaTime / 0.15f);
			if (alpha == 0f)
			{
				mLayout.SetAllVisible(false);
			}
		}
	}
}
