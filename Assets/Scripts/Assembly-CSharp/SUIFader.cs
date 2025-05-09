using UnityEngine;

public class SUIFader : SUIProcess
{
	private static float kDefaultSpeed = 0.5f;

	public OnFadingDoneCallback onFadingDone;

	private SUISprite mSprite;

	private float mTargetAlpha;

	private float mSpeed = kDefaultSpeed;

	public float fadeLevel
	{
		get
		{
			return mSprite.alpha;
		}
		set
		{
			mSprite.alpha = value;
			mTargetAlpha = value;
		}
	}

	public bool isFading
	{
		get
		{
			return mSprite.alpha != mTargetAlpha;
		}
	}

	public float speed
	{
		get
		{
			return mSpeed;
		}
		set
		{
			mSpeed = value;
		}
	}

	public float priority
	{
		get
		{
			return mSprite.priority;
		}
		set
		{
			mSprite.priority = value;
		}
	}

	public SUIFader()
	{
		Init(new Color(0f, 0f, 0f));
	}

	public SUIFader(Color c)
	{
		Init(c);
	}

	private void Init(Color c)
	{
		mSprite = new SUISprite(c, 8, 8);
		mSprite.priority = 1000f;
		mSprite.scale = new Vector2(SUIScreen.width / mSprite.width, SUIScreen.height / mSprite.height);
		mSprite.position = new Vector2(SUIScreen.width / 2f, SUIScreen.height / 2f);
		mSprite.alpha = 0f;
		mSprite.autoscaleKeepAspectRatio = false;
		mTargetAlpha = 0f;
	}

	public void Update()
	{
		if (mSprite.alpha != mTargetAlpha)
		{
			float num = SUIScreen.deltaTime / mSpeed;
			if (mSprite.alpha > mTargetAlpha)
			{
				mSprite.alpha = Mathf.Max(mTargetAlpha, mSprite.alpha - num);
			}
			else
			{
				mSprite.alpha = Mathf.Min(mTargetAlpha, mSprite.alpha + num);
			}
			if (mSprite.alpha == mTargetAlpha && onFadingDone != null)
			{
				onFadingDone();
			}
		}
		mSprite.visible = mSprite.alpha > 0f;
	}

	public void Destroy()
	{
		mSprite.Destroy();
		mSprite = null;
	}

	public void FadeFromCurrent()
	{
		mTargetAlpha = 0f;
	}

	public void FadeFromBlack()
	{
		mSprite.alpha = 1f;
		mTargetAlpha = 0f;
	}

	public void FadeToBlack()
	{
		mTargetAlpha = 1f;
	}

	public void FadeToLevel(float lvl)
	{
		mTargetAlpha = Mathf.Clamp(lvl, 0f, 1f);
	}

	public void EditorRenderOnGUI()
	{
	}
}
