using UnityEngine;

public class HUDSmithy
{
	private const float kUpgradeButtonOffset = 150f;

	private const float kBarWidth = 146f;

	private float[] kMasterScalePerLevel = new float[4]
	{
		79f / 146f,
		129f / 146f,
		1.2260274f,
		1.5616438f
	};

	private bool mActive = true;

	private SUITouchArea mTouchArea;

	private SUISprite mBarRef;

	private SUILabel mLevelRef;

	private SUILabel mRezCounterRef;

	private SUISprite mGlowRef;

	private SUISprite mUpgradeRef;

	private SUISprite mMainRef;

	private SUISprite mNotchesRef;

	private float mLastSmithResource = -1f;

	private float mLastSmithMaxResource = -1f;

	private int mLastSmithLevel = -1;

	private float mUpgradeButtonXOriginal;

	private float mUpgradeButtonAnim;

	private float mNotchesXOrigin;

	public Rect touchArea
	{
		get
		{
			return mTouchArea.area;
		}
	}

	public HUDSmithy(SUILayout layout)
	{
		mTouchArea = (SUITouchArea)layout["leadershipTouch"];
		mMainRef = (SUISprite)layout["leadershipMain"];
		mBarRef = (SUISprite)layout["leadershipBar"];
		mLevelRef = (SUILabel)layout["leadershipLvl"];
		mRezCounterRef = (SUILabel)layout["leadershipRez"];
		mGlowRef = (SUISprite)layout["leadershipGlow"];
		mUpgradeRef = (SUISprite)layout["leadershipUpgrade"];
		mNotchesRef = (SUISprite)layout["leadershipNotches"];
		mBarRef.position = new Vector2(ConvX(mBarRef.position.x, 44f), mBarRef.position.y);
		mNotchesRef.position = new Vector2(ConvX(mNotchesRef.position.x, 44f), mNotchesRef.position.y);
		mUpgradeButtonXOriginal = mUpgradeRef.position.x;
		mNotchesXOrigin = mNotchesRef.position.x;
		if (WeakGlobalInstance<Smithy>.instance.numTypes > 0)
		{
			mTouchArea.onAreaTouched = OnTouched;
			return;
		}
		mActive = false;
		mMainRef.visible = false;
		mBarRef.visible = false;
		mLevelRef.visible = false;
		mRezCounterRef.visible = false;
		mGlowRef.visible = false;
		mUpgradeRef.visible = false;
		mNotchesRef.visible = false;
	}

	public void Update()
	{
		if (WeakGlobalInstance<Smithy>.instance != null && mActive)
		{
			UpdateUpgradeState();
			UpdateBarScales();
			UpdateResourceCounter();
			UpdateLevelCounter();
		}
	}

	public bool IsTouchZone(Vector2 pos)
	{
		if (!mActive)
		{
			return false;
		}
		return touchArea.Contains(pos);
	}

	private float ConvX(float origX, float factor)
	{
		return origX - (factor * WeakGlobalInstance<SUIScreen>.instance.autoScaler.aspectRatioModifier - factor);
	}

	private void UpdateUpgradeState()
	{
		mGlowRef.visible = WeakGlobalInstance<Smithy>.instance.isUpgradable;
		if (WeakGlobalInstance<Smithy>.instance.isUpgradable && mUpgradeButtonAnim < 1f)
		{
			if (mUpgradeButtonAnim == 0f)
			{
				Singleton<SUISoundManager>.instance.Play("leadershipBecameUpgradable");
			}
			mUpgradeButtonAnim = Mathf.Min(1f, mUpgradeButtonAnim + SUIScreen.deltaTime * 2f);
			mUpgradeRef.visible = true;
		}
		else
		{
			if (WeakGlobalInstance<Smithy>.instance.isUpgradable || !(mUpgradeButtonAnim > 0f))
			{
				return;
			}
			mUpgradeButtonAnim = Mathf.Max(0f, mUpgradeButtonAnim - SUIScreen.deltaTime * 2f);
			if (mUpgradeButtonAnim == 0f)
			{
				mUpgradeRef.visible = false;
			}
		}
		mUpgradeRef.position = new Vector2(Ease.BackOut(mUpgradeButtonAnim, mUpgradeButtonXOriginal, 150f), mUpgradeRef.position.y);
	}

	private void UpdateBarScales()
	{
		float num = kMasterScalePerLevel[WeakGlobalInstance<Smithy>.instance.level - 1];
		float x = num * (WeakGlobalInstance<Smithy>.instance.resources / WeakGlobalInstance<Smithy>.instance.maxResources);
		mBarRef.scale = new Vector2(x, 1f);
	}

	private void UpdateResourceCounter()
	{
		if ((int)WeakGlobalInstance<Smithy>.instance.resources != (int)mLastSmithResource || WeakGlobalInstance<Smithy>.instance.maxResources != mLastSmithMaxResource)
		{
			mLastSmithMaxResource = WeakGlobalInstance<Smithy>.instance.maxResources;
			mLastSmithResource = WeakGlobalInstance<Smithy>.instance.resources;
			mRezCounterRef.text = ((int)mLastSmithResource).ToString();
		}
	}

	private void UpdateLevelCounter()
	{
		if (WeakGlobalInstance<Smithy>.instance.level != mLastSmithLevel)
		{
			mLastSmithLevel = WeakGlobalInstance<Smithy>.instance.level;
			mLevelRef.text = string.Format(Singleton<Localizer>.instance.Get("leadership_level"), mLastSmithLevel);
			mMainRef.texture = Singleton<PlayModesManager>.instance.selectedModeData["hudLeadershipBG"] + mLastSmithLevel;
			if (mLastSmithLevel == 4)
			{
				mNotchesRef.visible = false;
				return;
			}
			float num = WeakGlobalInstance<Smithy>.instance.levelUpThreshold / WeakGlobalInstance<Smithy>.instance.maxResources * 146f * (1f / WeakGlobalInstance<SUIScreen>.instance.autoScaler.aspectRatioModifier) * kMasterScalePerLevel[WeakGlobalInstance<Smithy>.instance.level - 1];
			mNotchesRef.position = new Vector2(mNotchesXOrigin + num, mNotchesRef.position.y);
			mNotchesRef.visible = true;
		}
	}

	private void OnTouched()
	{
		if (WeakGlobalInstance<Smithy>.instance.isUpgradable)
		{
			Singleton<SUISoundManager>.instance.Play("upgradeLeadership");
			WeakGlobalInstance<Smithy>.instance.LevelUp();
		}
	}
}
