using UnityEngine;

public class ListCells
{
	public class IconNameCounter : SUIScrollList.Cell
	{
		private const int kTitleMaxWidth = 164;

		private const float kRecommendedGlowPriority = 0.1f;

		private const float kRecommendedPriority = 2f;

		public Vector2 cursorOffset = new Vector2(0f, 0f);

		public Vector2 iconOffset = new Vector2(83f, 60f);

		public Vector2 labelOffset = new Vector2(83f, 122f);

		public Vector2 counterOffset = new Vector2(120f, 90f);

		public Vector2 kRecommendedGlowOffset = new Vector2(83f, 70f);

		public Vector2 kRecommendedOffset = new Vector2(83f, 100f);

		public SUISprite icon;

		public SUILabel title;

		public SUILabel counter;

		public SUISprite cursorRef;

		public bool isSelected;

		private float mPriority;

		private Vector2 mExtraOffset = new Vector2(0f, 0f);

		private bool mIsRecommended;

		private RecommendedWidget mRecommendedSprite;

		public override Vector2 position
		{
			set
			{
				icon.position = value + iconOffset + mExtraOffset;
				title.position = value + labelOffset + mExtraOffset;
				counter.position = value + counterOffset + mExtraOffset;
				mRecommendedSprite.position = value + kRecommendedOffset;
				if (cursorRef != null && isSelected)
				{
					cursorRef.position = value + cursorOffset;
				}
			}
		}

		public override bool visible
		{
			set
			{
				title.visible = value;
				icon.visible = value;
				counter.visible = value && counter.text != string.Empty;
				mRecommendedSprite.visible = value && mIsRecommended;
				if (cursorRef != null && isSelected)
				{
					cursorRef.visible = value;
				}
			}
		}

		public string counterText
		{
			get
			{
				return counter.text;
			}
			set
			{
				counter.text = value;
				visible = title.visible;
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
				icon.priority = mPriority + 0.1f;
				title.priority = mPriority + 0.1f;
				counter.priority = mPriority + 0.2f;
				if (cursorRef != null && isSelected)
				{
					cursorRef.priority = mPriority;
				}
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
			}
		}

		public bool isRecommended
		{
			get
			{
				return mIsRecommended;
			}
			set
			{
				mIsRecommended = value;
				if (mIsRecommended)
				{
					mRecommendedSprite.visible = true;
				}
				else
				{
					mRecommendedSprite.visible = false;
				}
			}
		}

		public IconNameCounter(string titleFont, SUISprite cursor, bool useShadow)
		{
			cursorRef = cursor;
			title = new SUILabel(titleFont);
			if (useShadow)
			{
				title.shadowOffset = new Vector2(2f, 2f);
				title.shadowColor = Color.black;
				title.fontColor = Color.white;
			}
			else
			{
				title.fontColor = Color.black;
			}
			title.alignment = TextAlignment.Center;
			title.anchor = TextAnchor.UpperCenter;
			title.maxWidth = 164;
			title.visible = false;
			counter = new SUILabel("default18");
			counter.alignment = TextAlignment.Center;
			counter.anchor = TextAnchor.MiddleCenter;
			counter.shadowOffset = new Vector2(2f, 2f);
			counter.shadowColor = new Color(0f, 0f, 0f);
			counter.visible = false;
			icon = new SUISprite();
			priority = 0f;
			mRecommendedSprite = new RecommendedWidget();
			mRecommendedSprite.priority = 2f;
			mRecommendedSprite.position = kRecommendedOffset;
		}

		public override void Destroy()
		{
			title.Destroy();
			title = null;
			icon.Destroy();
			icon = null;
			counter.Destroy();
			counter = null;
			cursorRef = null;
			mRecommendedSprite.Destroy();
			mRecommendedSprite = null;
			base.Destroy();
		}

		public override void Update()
		{
			base.Update();
			if (cursorRef != null && isSelected)
			{
				cursorRef.Update();
			}
			if (mRecommendedSprite.visible)
			{
				mRecommendedSprite.Update();
			}
		}

		public void CheckIfRecommended(string id)
		{
			SDFTreeNode waveData = WaveManager.GetWaveData(Singleton<Profile>.instance.waveToBeat);
			SDFTreeNode sDFTreeNode = waveData.to("RecommendedUseForThisWave");
			if (sDFTreeNode == null)
			{
				return;
			}
			for (int i = 0; i < sDFTreeNode.attributeCount; i++)
			{
				if (sDFTreeNode[i] == id)
				{
					isRecommended = true;
					break;
				}
			}
		}

		protected override void ApplyCombinedAlpha()
		{
			float num = mAlpha * mTransitionAlpha;
			title.alpha = num;
			icon.alpha = num;
			counter.alpha = num;
			if (mRecommendedSprite.visible)
			{
				mRecommendedSprite.alpha = num;
			}
			if (cursorRef != null && isSelected)
			{
				cursorRef.alpha = num;
			}
		}
	}
}
