using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDConsumables : SUISlideStripList.IController
{
	private class Cell : SUISlideStripList.Cell
	{
		private readonly float kCellsPriority = 5f;

		public bool dirtyCounter = true;

		private string mPotionID;

		private SUISprite icon;

		private SUILabel countLabel;

		private int lastCount;

		public override Vector2 position
		{
			set
			{
				icon.position = value + new Vector2(40f, 55f);
				countLabel.position = value + new Vector2(60f, 80f);
			}
		}

		public override bool visible
		{
			set
			{
				icon.visible = value;
				countLabel.visible = value;
			}
		}

		public override float alpha
		{
			set
			{
				icon.alpha = value;
				countLabel.alpha = value;
			}
		}

		private int counter
		{
			get
			{
				return lastCount;
			}
			set
			{
				if (value != lastCount)
				{
					countLabel.text = value.ToString();
					lastCount = value;
				}
			}
		}

		public Cell(string potionID, string iconFile, int counterVal)
		{
			mPotionID = potionID;
			float num = ((!SUIScreen.aspectRatioStandard) ? 0.8f : 1f);
			icon = new SUISprite(iconFile);
			icon.priority = kCellsPriority;
			icon.scale = new Vector2(num, num);
			countLabel = new SUILabel("default18");
			countLabel.priority = kCellsPriority + 0.1f;
			countLabel.anchor = TextAnchor.MiddleCenter;
			countLabel.shadowColor = Color.black;
			countLabel.shadowOffset = new Vector2(2f, 2f);
			countLabel.text = counterVal.ToString();
			lastCount = counterVal;
		}

		public override void Destroy()
		{
			if (icon != null)
			{
				icon.Destroy();
				icon = null;
			}
			if (countLabel != null)
			{
				countLabel.Destroy();
				countLabel = null;
			}
		}

		public override void Update()
		{
			if (dirtyCounter)
			{
				counter = Singleton<Profile>.instance.GetNumPotions(mPotionID);
				dirtyCounter = false;
			}
		}
	}

	private readonly Vector2 kCenterPosition = new Vector2(944f, 130f);

	private readonly Vector2 kCellSize = new Vector2(80f, 110f);

	private List<string> mAvailablePotions = new List<string>();

	private SUISlideStripList mStrip;

	private SUISprite mArrowLeftRef;

	private SUISprite mArrowRightRef;

	private event OnSUIGenericCallback OnDirtyCellCounters;

	public HUDConsumables(SUILayout layout)
	{
		bool flag = WeakGlobalInstance<Smithy>.instance.numTypes > 0;
		mArrowLeftRef = (SUISprite)layout["consumArrowLeft"];
		mArrowRightRef = (SUISprite)layout["consumArrowRight"];
		string[] allIDsForActivePlayMode = Singleton<PotionsDatabase>.instance.allIDsForActivePlayMode;
		foreach (string text in allIDsForActivePlayMode)
		{
			if ((flag || !(Singleton<PotionsDatabase>.instance.GetAttribute(text, "leadership") != string.Empty)) && bool.Parse(Singleton<PotionsDatabase>.instance.GetAttribute(text, "gameHud")))
			{
				mAvailablePotions.Add(text);
			}
		}
		mStrip = new SUISlideStripList(this, kCenterPosition, kCellSize);
		mStrip.onIconTouched = OnIconTouched;
		UpdateArrows();
	}

	public void Destroy()
	{
		if (mStrip != null)
		{
			mStrip.Destroy();
			mStrip = null;
		}
	}

	public void Update()
	{
		if (mStrip != null)
		{
			mStrip.Update();
		}
		UpdateArrows();
	}

	public void UpdateVisualsOnly()
	{
		if (mStrip != null)
		{
			mStrip.allowInputs = false;
			mStrip.Update();
			mStrip.allowInputs = true;
		}
	}

	public void DirtyCellCounters()
	{
		if (this.OnDirtyCellCounters != null)
		{
			this.OnDirtyCellCounters();
		}
	}

	public bool IsTouchZone(Vector2 pos)
	{
		Rect touchArea = mStrip.touchArea;
		touchArea.x -= 200f;
		touchArea.width += 300f;
		return touchArea.Contains(pos);
	}

	private void UpdateArrows()
	{
		mArrowLeftRef.visible = mStrip.selected != 0;
		mArrowRightRef.visible = mStrip.selected != mAvailablePotions.Count - 1;
	}

	private void OnIconTouched(int index)
	{
		if (WeakGlobalSceneBehavior<InGameImpl>.instance.gameOver || WeakGlobalSceneBehavior<InGameImpl>.instance.hero.health == 0f)
		{
			return;
		}
		string text = mAvailablePotions[index];
		if (Singleton<Profile>.instance.GetNumPotions(text) == 0)
		{
			WeakGlobalSceneBehavior<InGameImpl>.instance.ShowInGameStore(text);
		}
		else if (Singleton<PotionsDatabase>.instance.Execute(text))
		{
			Singleton<Profile>.instance.SetNumPotions(text, Singleton<Profile>.instance.GetNumPotions(text) - 1);
			DirtyCellCounters();
			string attribute = Singleton<PotionsDatabase>.instance.GetAttribute(text, "pressedSound");
			if (attribute != string.Empty)
			{
				Singleton<SUISoundManager>.instance.Play(attribute);
			}
		}
	}

	public int SlideStripList_NumEntries()
	{
		return mAvailablePotions.Count;
	}

	public SUISlideStripList.Cell SlideStripList_CreateCell(int dataIndex)
	{
		string potionID = mAvailablePotions[dataIndex];
		Cell c = new Cell(potionID, Singleton<PotionsDatabase>.instance.GetAttribute(mAvailablePotions[dataIndex], "icon"), Singleton<Profile>.instance.GetNumPotions(potionID));
		this.OnDirtyCellCounters = (OnSUIGenericCallback)Delegate.Combine(this.OnDirtyCellCounters, (OnSUIGenericCallback)delegate
		{
			c.dirtyCounter = true;
		});
		return c;
	}
}
