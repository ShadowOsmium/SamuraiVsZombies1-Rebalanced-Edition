using UnityEngine;

public class GenericGameBanner
{
	private const float kAnimSpeed = 0.5f;

	private const float kWaitTimer = 2f;

	private const float kBannerPriority = 400f;

	private readonly Vector2 kBannerPosition = new Vector2(512f, 250f);

	private SUISprite mBanner;

	private bool mIsDone;

	private float mTimer;

	private OnSUIGenericCallback mStateFunc;

	private int mSkipNumUpdate = 2;

	public bool isDone
	{
		get
		{
			return mIsDone;
		}
	}

	public GenericGameBanner(string bannerSprite, string soundFX)
	{
		mBanner = new SUISprite(bannerSprite);
		mBanner.priority = 400f;
		mBanner.position = kBannerPosition;
		mBanner.alpha = 0f;
		mStateFunc = StateFunc_Intro;
		if (soundFX != null && soundFX != string.Empty)
		{
			Singleton<SUISoundManager>.instance.Play(soundFX, mBanner.gameObject);
		}
	}

	public void Update()
	{
		if (mSkipNumUpdate > 0)
		{
			mSkipNumUpdate--;
		}
		else if (!mIsDone)
		{
			mTimer += Time.deltaTime;
			mStateFunc();
			if (mBanner != null)
			{
				mBanner.Update();
			}
		}
	}

	public void Destroy()
	{
		if (mBanner != null)
		{
			mBanner.Destroy();
			mBanner = null;
			mIsDone = true;
		}
	}

	private void StateFunc_Intro()
	{
		float num = mTimer / 0.5f;
		if (num < 1f)
		{
			SetAnimFrame(num);
			return;
		}
		SetAnimFrame(1f);
		mTimer = 0f;
		mStateFunc = StateFunc_Wait;
	}

	private void StateFunc_Wait()
	{
		if (mTimer >= 2f)
		{
			mTimer = 0f;
			mStateFunc = StateFunc_Outro;
		}
	}

	private void StateFunc_Outro()
	{
		float num = mTimer / 0.5f;
		if (num < 1f)
		{
			SetAnimFrame(1f - num);
			return;
		}
		mBanner.Destroy();
		mBanner = null;
		mIsDone = true;
	}

	private void SetAnimFrame(float animProgress)
	{
		mBanner.alpha = animProgress;
		float num = Ease.BackOut(animProgress, 0f, 1f);
		mBanner.scale = new Vector2(num, num);
	}
}
