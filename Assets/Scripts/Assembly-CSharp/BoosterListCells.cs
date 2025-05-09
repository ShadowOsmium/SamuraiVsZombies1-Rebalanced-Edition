using UnityEngine;

public class BoosterListCells
{
	public class Booster : SUIScrollList.Cell
	{
		private const float kOnSalePriorityOffset = 1f;

		private readonly Vector2 kPackSpriteOffset = new Vector2(80f, 124f);

		private readonly Vector2 kOnSalePositionOffset = new Vector2(130f, 40f);

		private readonly Vector2 kExtraTickerPositionOffset = new Vector2(30f, 40f);

		public SUISprite cursorRef;

		public bool isSelected;

		private BoosterPackSprite mPackSprite = new BoosterPackSprite();

		private NoveltyWidget mOnSalePulse;

		private NoveltyWidget mExtraTicker;

		public override Vector2 position
		{
			set
			{
				mPackSprite.position = value + kPackSpriteOffset;
				if (cursorRef != null && isSelected)
				{
					cursorRef.position = mPackSprite.position;
				}
				if (mOnSalePulse != null && mOnSalePulse.visible)
				{
					mOnSalePulse.position = value + kOnSalePositionOffset;
				}
				if (mExtraTicker != null && mExtraTicker.visible)
				{
					mExtraTicker.position = value + kExtraTickerPositionOffset;
				}
			}
		}

		public override bool visible
		{
			set
			{
				mPackSprite.visible = value;
				if (cursorRef != null && isSelected)
				{
					cursorRef.visible = value;
				}
				if (mOnSalePulse != null && mOnSalePulse.visible)
				{
					mOnSalePulse.visible = value;
				}
				if (mExtraTicker != null && mExtraTicker.visible)
				{
					mExtraTicker.visible = value;
				}
			}
		}

		public float priority
		{
			get
			{
				return mPackSprite.priority;
			}
			set
			{
				mPackSprite.priority = value;
				if (cursorRef != null && isSelected)
				{
					cursorRef.priority = value - 0.1f;
				}
				if (mOnSalePulse != null && mOnSalePulse.visible)
				{
					mOnSalePulse.priority = value + 1f;
				}
				if (mExtraTicker != null && mExtraTicker.visible)
				{
					mExtraTicker.priority = value + 1f;
				}
			}
		}

		public Vector2 scale
		{
			get
			{
				return mPackSprite.scale;
			}
			set
			{
				mPackSprite.scale = value;
			}
		}

		public Booster(SUISprite cursor)
		{
			cursorRef = cursor;
		}

		public override void Destroy()
		{
			cursorRef = null;
			mPackSprite.Destroy();
			mPackSprite = null;
			if (mOnSalePulse != null)
			{
				mOnSalePulse.Destroy();
				mOnSalePulse = null;
			}
			if (mExtraTicker != null)
			{
				mOnSalePulse.Destroy();
				mOnSalePulse = null;
			}
		}

		public override void Update()
		{
			base.Update();
			if (cursorRef != null && isSelected)
			{
				cursorRef.Update();
			}
			mPackSprite.Update();
			if (mOnSalePulse != null && mOnSalePulse.visible)
			{
				mOnSalePulse.Update();
			}
			if (mExtraTicker != null && mExtraTicker.visible)
			{
				mExtraTicker.Update();
			}
		}

		public void Render(int packIndex)
		{
			mPackSprite.Render(Singleton<BoosterPackCodex>.instance.GetPackData(packIndex).to("render"));
			if (Singleton<BoosterPackCodex>.instance.IsPackOnSale(packIndex))
			{
				if (mOnSalePulse != null)
				{
					mOnSalePulse.visible = true;
				}
				else
				{
					mOnSalePulse = new NoveltyWidget();
					mOnSalePulse.texture = "Sprites/Localized/on_sale";
					mOnSalePulse.priority = mPackSprite.priority + 1f;
				}
			}
			else if (mOnSalePulse != null)
			{
				mOnSalePulse.visible = false;
			}
			string text = Singleton<BoosterPackCodex>.instance.GetPackData(packIndex)["extraTicker"];
			if (text != string.Empty)
			{
				if (mExtraTicker != null)
				{
					mExtraTicker.texture = text;
					mExtraTicker.visible = true;
				}
				else
				{
					mExtraTicker = new NoveltyWidget();
					mExtraTicker.texture = text;
					mExtraTicker.priority = mPackSprite.priority + 1f;
				}
			}
			else if (mExtraTicker != null)
			{
				mExtraTicker.visible = false;
			}
		}

		protected override void ApplyCombinedAlpha()
		{
			float num = mAlpha * mTransitionAlpha;
			mPackSprite.alpha = num;
			if (cursorRef != null && isSelected)
			{
				cursorRef.alpha = num;
			}
			if (mOnSalePulse != null && mOnSalePulse.visible)
			{
				mOnSalePulse.alpha = num;
			}
			if (mExtraTicker != null && mExtraTicker.visible)
			{
				mExtraTicker.alpha = num;
			}
		}
	}

	public class Card : SUIScrollList.Cell
	{
		private const float kRevealLength = 0.25f;

		private Vector2 kCardSpriteOffset;

		public SUISprite cursorRef;

		public bool isSelected;

		private BoosterCardSprite mCardSprite;

		private SUILabel mRevealBehind;

		private float mScale = 1f;

		private float mPriority;

		private Vector2 mPosition;

		private bool mVisible = true;

		private float mRevealTimer;

		private float mRevealAlpha = 1f;

		public float revealTimer
		{
			get
			{
				return mRevealTimer;
			}
			set
			{
				mRevealTimer = value;
				UpdateReveal();
			}
		}

		public override Vector2 position
		{
			set
			{
				mPosition = value;
				if (mCardSprite != null)
				{
					mCardSprite.position = value + kCardSpriteOffset;
				}
				if (mRevealBehind != null)
				{
					mRevealBehind.position = value + kCardSpriteOffset;
				}
			}
		}

		public override bool visible
		{
			set
			{
				mVisible = value;
				if (mCardSprite != null)
				{
					mCardSprite.visible = value;
				}
				if (mRevealBehind != null)
				{
					mRevealBehind.visible = value;
				}
			}
		}

		public float priority
		{
			get
			{
				return mPriority;
			}
			set
			{
				mPriority = value;
				if (mCardSprite != null)
				{
					mCardSprite.priority = mPriority + 0.1f;
				}
				if (mRevealBehind != null)
				{
					mRevealBehind.priority = mPriority;
				}
			}
		}

		public Card(float scale, Vector2 cellSize)
		{
			mScale = scale;
			kCardSpriteOffset = new Vector2(cellSize.x / 2f, cellSize.y / 2f);
		}

		public override void Destroy()
		{
			mCardSprite.Destroy();
			mCardSprite = null;
			if (mRevealBehind != null)
			{
				mRevealBehind.Destroy();
				mRevealBehind = null;
			}
		}

		public override void Update()
		{
			base.Update();
			mCardSprite.Update();
			mRevealTimer = Mathf.Max(mRevealTimer - Time.deltaTime, 0f);
			UpdateReveal();
		}

		public void Render(SDFTreeNode renderData, string groupID, string cardArg)
		{
			if (mCardSprite != null)
			{
				mCardSprite.Destroy();
			}
			mCardSprite = new BoosterCardSprite(renderData, groupID, cardArg);
			mCardSprite.scale = new Vector2(mScale, mScale);
			mCardSprite.priority = mPriority;
		}

		protected override void ApplyCombinedAlpha()
		{
			if (mCardSprite != null)
			{
				mCardSprite.alpha = mAlpha * mTransitionAlpha * mRevealAlpha;
			}
			if (mRevealBehind != null)
			{
				mRevealBehind.alpha = mAlpha * mTransitionAlpha * (1f - mRevealAlpha);
			}
		}

		private void UpdateReveal()
		{
			mRevealAlpha = Mathf.Clamp((0.25f - mRevealTimer) / 0.25f, 0f, 1f);
			if (mRevealAlpha < 1f && mRevealBehind == null)
			{
				mRevealBehind = new SUILabel("default64");
				mRevealBehind.alignment = TextAlignment.Center;
				mRevealBehind.anchor = TextAnchor.MiddleCenter;
				mRevealBehind.text = "?";
				position = mPosition;
				priority = mPriority;
				visible = mVisible;
			}
			ApplyCombinedAlpha();
		}
	}
}
