using UnityEngine;

public class HUDBloodPulse
{
	private const float kMinLevel = 0.4f;

	private const float kMaxLevel = 0.6f;

	private const float kPulseSpeed = 1f;

	private SUIFader mFader;

	private Hero mHeroRef;

	private Base mGateRef;

	private float mPulseDir = 1f;

	public HUDBloodPulse()
	{
		mFader = new SUIFader(Color.red);
		mFader.priority = 0f;
		mFader.fadeLevel = 0f;
	}

	public void Update()
	{
		AcquireHero();
		if (mHeroRef.health == 0f)
		{
			mFader.fadeLevel = Mathf.Max(0f, mFader.fadeLevel - Time.deltaTime * 1f);
		}
		else if (mGateRef.health > 0f && mHeroRef.isInLeniencyMode)
		{
			if (mFader.fadeLevel == 0f)
			{
				mFader.fadeLevel = 0.6f;
				mPulseDir = -1f;
			}
			else
			{
				mFader.fadeLevel = Mathf.Clamp(mFader.fadeLevel + mPulseDir * (Time.deltaTime * 1f), 0.4f, 0.6f);
				if (mFader.fadeLevel == 0.4f)
				{
					mPulseDir = 1f;
				}
				else if (mFader.fadeLevel == 0.6f)
				{
					mPulseDir = -1f;
				}
			}
		}
		else
		{
			mFader.fadeLevel = 0f;
		}
		mFader.Update();
	}

	private void AcquireHero()
	{
		if (mHeroRef == null)
		{
			mHeroRef = WeakGlobalSceneBehavior<InGameImpl>.instance.hero;
		}
		if (mGateRef == null)
		{
			mGateRef = WeakGlobalSceneBehavior<InGameImpl>.instance.gate;
		}
	}
}
