using UnityEngine;

public class StoreListCell : SUIScrollList.Cell
{
	private const float kAlphaLocked = 0.4f;

	private const int kTitleMaxWidth = 200;

	private const float kPriority = 2f;

	private const float kRecommendedGlowPriority = 1.9f;

	private const float kRecommendedPriority = 2.1f;

	private const float kNoveltyPriority = 2.2f;

	private readonly Vector2 kIconOffset = new Vector2(110f, 60f);

	private Vector2 kCostOffset = new Vector2(110f, 120f);

	private Vector2 kTitleOffset = new Vector2(110f, 6f);

	private readonly Vector2 kRecommendedGlowOffset = new Vector2(110f, 60f);

	private readonly Vector2 kRecommendedOffset = new Vector2(110f, 90f);

	private readonly Vector2 kNoveltyOffset = new Vector2(56f, 50f);

	private readonly Vector2 kExtraPulseOffset = new Vector2(164f, 30f);

	private Vector2 mExtraOffset = new Vector2(0f, 16f);

	private SUISprite mIcon;

	private SUILabel mDescription;

	private NoveltyWidget mNoveltyWidget;

	private NoveltyWidget mOnSalePulse;

	private IconGlowWidget mRecommendedGlow;

	private RecommendedWidget mRecommendedSprite;

	private bool mIsNovelty;

	private bool mHasOnSalePulse;

	private bool mIsRecommended;

	private CostDisplay mCost;

	private Vector2 mPosition = new Vector2(0f, 0f);

	private SUISprite m_gwCost;

	private bool m_isGWItem;

	public override Vector2 position
	{
		set
		{
			mPosition = value;
			UpdatePosition();
		}
	}

	public override bool visible
	{
		set
		{
			mDescription.visible = value;
			mIcon.visible = value;
			mCost.visible = value;
			mNoveltyWidget.visible = value && mIsNovelty;
			mOnSalePulse.visible = value && mHasOnSalePulse;
			mRecommendedGlow.visible = value && mIsRecommended;
			mRecommendedSprite.visible = value && mIsRecommended;
			m_gwCost.visible = value && m_isGWItem;
		}
	}

	public Vector2 extraOffset
	{
		get
		{
			return mExtraOffset;
		}
		set
		{
			mExtraOffset = value;
			UpdatePosition();
		}
	}

	public StoreListCell()
	{
		mDescription = new SUILabel("default18");
		if (Screen.width > 480)
		{
			mDescription.shadowColor = new Color(0f, 0f, 0f);
			mDescription.shadowOffset = new Vector2(2f, 2f);
		}
		mDescription.alignment = TextAlignment.Center;
		mDescription.anchor = TextAnchor.UpperCenter;
		mDescription.maxWidth = 200;
		mDescription.visible = false;
		mDescription.priority = 2f;
		mIcon = new SUISprite();
		mIcon.priority = 2f;
		mCost = new CostDisplay();
		mCost.priority = 2f;
		mNoveltyWidget = new NoveltyWidget();
		mNoveltyWidget.priority = 2.2f;
		mOnSalePulse = new NoveltyWidget();
		mOnSalePulse.priority = 2.2f;
		mRecommendedGlow = new IconGlowWidget();
		mRecommendedGlow.priority = 1.9f;
		mRecommendedGlow.position = kRecommendedGlowOffset;
		mRecommendedSprite = new RecommendedWidget();
		mRecommendedSprite.priority = 2.1f;
		mRecommendedSprite.position = kRecommendedOffset;
		m_gwCost = new SUISprite();
		m_gwCost.priority = 2f;
	}

	public override void Destroy()
	{
		mDescription.Destroy();
		mDescription = null;
		mIcon.Destroy();
		mIcon = null;
		mCost.Destroy();
		mCost = null;
		mNoveltyWidget.Destroy();
		mNoveltyWidget = null;
		mOnSalePulse.Destroy();
		mOnSalePulse = null;
		mRecommendedGlow.Destroy();
		mRecommendedGlow = null;
		mRecommendedSprite.Destroy();
		mRecommendedSprite = null;
		base.Destroy();
		m_gwCost.Destroy();
		m_gwCost = null;
	}

	public override void Update()
	{
		if (mNoveltyWidget.visible)
		{
			mNoveltyWidget.Update();
		}
		if (mOnSalePulse.visible)
		{
			mOnSalePulse.Update();
		}
		if (mRecommendedGlow.visible)
		{
			mRecommendedGlow.Update();
		}
		if (mRecommendedSprite.visible)
		{
			mRecommendedSprite.Update();
		}
	}

	public void DrawContent(StoreData.Item itemData)
	{
		m_isGWItem = false;
		if (itemData.locked)
		{
			mDescription.text = itemData.unlockCondition;
			try
			{
				mIcon.texture = itemData.icon + "_locked";
			}
			catch
			{
				mIcon.texture = itemData.icon;
			}
			mCost.SetCost(new Cost(0, 0, 0f));
			mIsRecommended = false;
			mRecommendedSprite.visible = false;
			mRecommendedGlow.visible = false;
		}
		else
		{
			mCost.SetCost(itemData.cost);
			mDescription.text = itemData.title;
			mIcon.texture = itemData.icon;
			if (itemData.id == "vip")
			{
				m_isGWItem = true;
				if (ApplicationUtilities.IsGWalletAvailable() && ApplicationUtilities.GWalletIsSubscriberToPlan(".silver"))
				{
					m_gwCost.texture = "Sprites/Menus/gw_cost_silver";
				}
				else
				{
					m_gwCost.texture = "Sprites/Menus/gw_cost";
				}
			}
			if (CheckIfRecommended(itemData))
			{
				mIsRecommended = true;
				mRecommendedSprite.visible = true;
				mRecommendedGlow.visible = true;
			}
			else
			{
				mIsRecommended = false;
				mRecommendedSprite.visible = false;
				mRecommendedGlow.visible = false;
			}
		}
		mIsNovelty = itemData.isNovelty || itemData.isEvent;
		mNoveltyWidget.visible = mIsNovelty;
		if (itemData.cost.isOnSale)
		{
			mHasOnSalePulse = true;
			mOnSalePulse.visible = true;
			mOnSalePulse.texture = "Sprites/Localized/on_sale";
		}
		else
		{
			mHasOnSalePulse = false;
			mOnSalePulse.visible = false;
		}
		float num = ((!itemData.locked) ? 1f : 0.4f);
		mCost.alpha = num;
		mDescription.alpha = num;
		mNoveltyWidget.alpha = num;
		mOnSalePulse.alpha = num;
		mRecommendedGlow.alpha = num;
		mRecommendedSprite.alpha = num;
	}

	private bool CheckIfRecommended(StoreData.Item itemData)
	{
		SDFTreeNode waveData = WaveManager.GetWaveData(Singleton<Profile>.instance.waveToBeat);
		SDFTreeNode sDFTreeNode = waveData.to("RecommendUpgrades");
		if (sDFTreeNode != null && sDFTreeNode.hasAttribute(itemData.id))
		{
			int num = int.Parse(sDFTreeNode[itemData.id]);
			switch (itemData.id)
			{
			case "HeroSamurai":
				if (Singleton<Profile>.instance.heroLevel < num)
				{
					return true;
				}
				return false;
			case "Katana":
				if (Singleton<Profile>.instance.swordLevel < num)
				{
					return true;
				}
				return false;
			case "Bow":
				if (Singleton<Profile>.instance.bowLevel < num)
				{
					return true;
				}
				return false;
			case "Smithy":
				if (Singleton<Profile>.instance.initialSmithyLevel < num)
				{
					return true;
				}
				return false;
			case "Base":
				if (Singleton<Profile>.instance.baseLevel < num)
				{
					return true;
				}
				return false;
			case "Bell":
				if (Singleton<Profile>.instance.bellLevel < num)
				{
					return true;
				}
				return false;
			case "VillageArchers":
				if (Singleton<Profile>.instance.archerLevel < num)
				{
					return true;
				}
				return false;
			case "KatanaSlash":
			case "SummonLightning":
			case "Lethargy":
			case "DivineIntervention":
			case "SummonTornadoes":
			case "GiantWave":
			case "GroundShock":
				if (Singleton<Profile>.instance.GetAbilityLevel(itemData.id) < num)
				{
					return true;
				}
				return false;
			}
		}
		SDFTreeNode sDFTreeNode2 = waveData.to("RecommendedAllies");
		if (sDFTreeNode2 != null && sDFTreeNode2.hasAttribute(itemData.id))
		{
			int num2 = int.Parse(sDFTreeNode2[itemData.id]);
			switch (itemData.id)
			{
			case "Farmer":
			case "Samurai":
			case "SamuraiArcher":
			case "HeavySamurai":
			case "Priest":
			case "Nobunaga":
			case "IceArcher":
			case "SpearHorseman":
			case "Takeda":
				if (Singleton<Profile>.instance.GetHelperLevel(itemData.id) < num2)
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	private void UpdatePosition()
	{
		Vector2 vector = mPosition + mExtraOffset;
		mIcon.position = vector + kIconOffset;
		mNoveltyWidget.position = vector + kNoveltyOffset;
		mOnSalePulse.position = vector + kExtraPulseOffset;
		mRecommendedGlow.position = vector + kRecommendedGlowOffset;
		mRecommendedSprite.position = vector + kRecommendedOffset;
		mCost.position = vector + kCostOffset + new Vector2(0f - mCost.width / 2f, 0f);
		if (m_isGWItem)
		{
			m_gwCost.position = vector + kCostOffset + new Vector2(0f, 16f);
			mDescription.position = new Vector2(mIcon.position.x, mCost.position.y + m_gwCost.height + kTitleOffset.y);
		}
		else
		{
			mDescription.position = new Vector2(mIcon.position.x, mCost.position.y + mCost.height + kTitleOffset.y);
		}
	}

	protected override void ApplyCombinedAlpha()
	{
		float num = mAlpha * mTransitionAlpha;
		if (mDescription.visible)
		{
			mDescription.alpha = num;
		}
		if (mIcon.visible)
		{
			mIcon.alpha = num;
		}
		if (mCost.visible)
		{
			mCost.alpha = num;
		}
		if (mNoveltyWidget.visible)
		{
			mNoveltyWidget.alpha = num;
		}
		if (mOnSalePulse.visible)
		{
			mOnSalePulse.alpha = num;
		}
		if (mRecommendedSprite.visible)
		{
			mRecommendedSprite.alpha = num;
		}
		if (mRecommendedGlow.visible)
		{
			mRecommendedGlow.alpha = num;
		}
	}
}
