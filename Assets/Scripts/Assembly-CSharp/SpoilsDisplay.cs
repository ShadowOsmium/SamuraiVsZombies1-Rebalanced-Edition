using System;
using System.Collections.Generic;
using UnityEngine;

public class SpoilsDisplay
{
	public class Entry
	{
		public string id;

		public string icon;

		public string text;

		public int count;

		public ECollectableType presentType;

		public Entry(string _id, string _icon, string _text)
		{
			id = _id;
			icon = _icon;
			text = _text;
			count = 1;
			presentType = ECollectableType.Count;
		}

		public bool Equals(Entry e)
		{
			return icon == e.icon && text == e.text;
		}
	}

	private const float kArrowOffset = 74f;

	private readonly Vector2 kRightArrowScale = new Vector2(0.6f, 1.5f);

	private readonly Vector2 kLeftArrowScale = new Vector2(-0.6f, 1.5f);

	private SUILayout mLayoutRef;

	private List<string> mComponentsInLayout = new List<string>();

	private Entry[] mFullContent;

	private string mFontName;

	private Vector2 mTargetPosition;

	private float mPriority;

	private int mMaxItems;

	private float mDelta;

	private int mCurrentRenderedStartIndex;

	private string mLineTag = string.Empty;

	private SUITouchArea mTouchNext;

	private SUITouchArea mTouchPrevious;

	private OnSUIGenericCallback mDelayedCalls;

	public SpoilsDisplay(SUILayout layout, List<Entry> content, string fontName, Vector2 pos, float priority, int maxItems, float delta, string lineTag)
	{
		if (fontName == string.Empty)
		{
			fontName = "default18";
		}
		if (content == null)
		{
			content = GetEntryList();
			AppendEventRewards(content);
		}
		mLayoutRef = layout;
		mFontName = fontName;
		mTargetPosition = pos;
		mPriority = priority;
		mMaxItems = maxItems;
		mDelta = delta;
		mLineTag = lineTag;
		mFullContent = new Entry[content.Count];
		content.CopyTo(mFullContent);
		RenderFrom(0);
	}

	public void Destroy()
	{
	}

	public void Update()
	{
		if (mDelayedCalls != null)
		{
			mDelayedCalls();
			mDelayedCalls = null;
		}
	}

	public static List<Entry> GetEntryList()
	{
		List<Entry> list = new List<Entry>();
		if (CollectableManager.currentWaveSpoils.coins > 0)
		{
			Entry entry = new Entry("coins", "Sprites/Icons/win_coins", "...");
			entry.count = CollectableManager.currentWaveSpoils.coins;
			list.Add(entry);
		}
		if (CollectableManager.currentWaveSpoils.gems > 0)
		{
			Entry entry2 = new Entry("gems", "Sprites/Icons/win_gems", "...");
			entry2.count = CollectableManager.currentWaveSpoils.gems;
			list.Add(entry2);
		}
		if (CollectableManager.currentWaveSpoils.balls > 0)
		{
			Entry entry3 = new Entry("balls", "Sprites/Menus/pachinko_stats", "...");
			entry3.count = CollectableManager.currentWaveSpoils.balls;
			list.Add(entry3);
		}
		int num = 0;
		foreach (CollectedPresent present in CollectableManager.currentWaveSpoils.presents)
		{
			Entry entry4 = BuildEntryFromPresent(present.type, present.contents);
			if (entry4.icon == string.Empty || entry4.text == string.Empty)
			{
				Debug.Log("ERROR: Could not figure out what present ID is: " + present.contents);
				continue;
			}
			entry4.id = "present" + num;
			bool flag = false;
			foreach (Entry item in list)
			{
				if (item.Equals(entry4))
				{
					item.count++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(entry4);
			}
			num++;
		}
		return list;
	}

	private static void AppendEventRewards(List<Entry> original)
	{
		List<string> activeRewards = Singleton<EventsManager>.instance.activeRewards;
		foreach (string item in activeRewards)
		{
			original.Insert(0, BuildEntry(item, 1));
		}
	}

	public static Entry BuildEntry(string id, int num)
	{
		Entry entry = new Entry(id, string.Empty, string.Empty);
		entry.count = num;
		switch (id)
		{
		case "coins":
			entry.icon = "Sprites/Icons/win_coins";
			entry.text = string.Format(Singleton<Localizer>.instance.Get("coinsNum"), num);
			return entry;
		case "gems":
			entry.icon = "Sprites/Icons/win_gems";
			entry.text = string.Format(Singleton<Localizer>.instance.Get("gemsNum"), num);
			return entry;
		case "balls":
			entry.icon = "Sprites/Menus/pachinko_stats";
			entry.text = string.Format(Singleton<Localizer>.instance.Get("ballsNum"), num);
			return entry;
		case "booster_free":
			entry.icon = "@" + id;
			entry.text = Singleton<Localizer>.instance.Get("boosterpack_title_free");
			return entry;
		default:
			if (Singleton<PotionsDatabase>.instance.Contains(id))
			{
				entry.icon = Singleton<PotionsDatabase>.instance.GetAttribute(id, "icon");
				entry.text = Singleton<PotionsDatabase>.instance.GetAttribute(id, "displayName");
				return entry;
			}
			if (Singleton<CharmsDatabase>.instance.Contains(id))
			{
				entry.icon = Singleton<CharmsDatabase>.instance.GetAttribute(id, "icon");
				entry.text = Singleton<CharmsDatabase>.instance.GetAttribute(id, "displayName");
				return entry;
			}
			if (Singleton<HelpersDatabase>.instance.Contains(id))
			{
				entry.icon = Singleton<HelpersDatabase>.instance.GetAttribute(id, "HUDIcon");
				entry.text = Singleton<HelpersDatabase>.instance.GetAttribute(id, "displayName");
				return entry;
			}
			if (Singleton<AbilitiesDatabase>.instance.Contains(id))
			{
				entry.icon = Singleton<AbilitiesDatabase>.instance.GetAttribute(id, "icon");
				entry.text = Singleton<AbilitiesDatabase>.instance.GetAttribute(id, "displayName");
				return entry;
			}
			Debug.Log("SpoilDisplay.BuildEntry could not find id: " + id);
			return null;
		}
	}

	private static Entry BuildEntryFromPresent(ECollectableType presentType, string contentID)
	{
		Entry entry = BuildEntry(contentID, 1);
		entry.presentType = presentType;
		return entry;
	}

	private void RenderFrom(int firstToShow)
	{
		ClearDrawnSpoils();
		mCurrentRenderedStartIndex = firstToShow;
		int num = Mathf.Min(mFullContent.Length - 1, firstToShow + mMaxItems - 1);
		int num2 = num - firstToShow + 1;
		Vector2 vector = new Vector2(mTargetPosition.x - (float)(num2 - 1) * mDelta / 2f, mTargetPosition.y);
		if (firstToShow != 0)
		{
			DrawLeftArrow(vector);
		}
		int num3 = 0;
		for (int i = firstToShow; i <= num; i++)
		{
			Entry entry = mFullContent[i];
			DrawEntry(vector, entry, i, num3);
			vector.x += mDelta;
			if (entry.presentType != ECollectableType.Count)
			{
				num3++;
			}
		}
		if (num < mFullContent.Length - 1)
		{
			vector.x -= mDelta;
			DrawRightArrow(vector);
		}
	}

	private void OnNextBlock()
	{
		int num = mCurrentRenderedStartIndex + mMaxItems;
		if (num >= mFullContent.Length)
		{
			num = 0;
		}
		if (num != mCurrentRenderedStartIndex)
		{
			RenderFrom(num);
		}
	}

	private void OnPreviousBlock()
	{
		int num = mCurrentRenderedStartIndex - mMaxItems;
		if (num < 0)
		{
			num = 0;
		}
		if (num != mCurrentRenderedStartIndex)
		{
			RenderFrom(num);
		}
	}

	private void ClearDrawnSpoils()
	{
		foreach (string item in mComponentsInLayout)
		{
			mLayoutRef.Remove(item, true);
		}
		mComponentsInLayout.Clear();
	}

	private void DrawEntry(Vector2 pos, Entry entry, int objectIndex, int numPresentsSoFar)
	{
		string text = string.Empty;
		switch (entry.presentType)
		{
		case ECollectableType.presentA:
			text = "Sprites/Icons/present_red";
			break;
		case ECollectableType.presentB:
			text = "Sprites/Icons/present_blue";
			break;
		case ECollectableType.presentC:
			text = "Sprites/Icons/present_yellow";
			break;
		}
		int num = entry.count;
		string contentTitle = entry.text;
		if (entry.id == "coins" || entry.id == "gems" || entry.id == "balls")
		{
			contentTitle = entry.count.ToString();
			num = 1;
		}
		PresentOpener presentOpener = new PresentOpener(text, entry.icon, contentTitle, mFontName, (int)mDelta - 16, num);
		if (presentOpener.label.shownLines > 3)
		{
			presentOpener.label.maxLines = 3;
		}
		presentOpener.priority = mPriority;
		presentOpener.position = pos;
		if (text != string.Empty)
		{
			presentOpener.Open(0.7f, 1f + 0.4f * (float)numPresentsSoFar);
		}
		else
		{
			presentOpener.frame = 1f;
		}
		AddObjectToLayout(mLineTag + "_spoils_" + objectIndex, true, presentOpener);
	}

	private void AddObjectToLayout(string name, bool withFade, SUIProcess objToAdd)
	{
		mComponentsInLayout.Add(name);
		SUILayout.ObjectData objectData = new SUILayout.ObjectData();
		objectData.obj = objToAdd;
		if (withFade)
		{
			objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
		}
		mLayoutRef.Add(name, objectData);
	}

	private void DrawLeftArrow(Vector2 leftMostIconPosition)
	{
		leftMostIconPosition.x -= 74f;
		SUISprite sUISprite = new SUISprite("Sprites/Menus/level_up_arrow");
		sUISprite.position = leftMostIconPosition;
		sUISprite.priority = mPriority;
		sUISprite.scale = kLeftArrowScale;
		AddObjectToLayout("spoils_leftarrow_sprite", true, sUISprite);
		SUITouchArea sUITouchArea = new SUITouchArea(new Rect(leftMostIconPosition.x - 64f, leftMostIconPosition.y - 64f, 128f, 128f));
		sUITouchArea.onAreaTouched = delegate
		{
			mDelayedCalls = (OnSUIGenericCallback)Delegate.Combine(mDelayedCalls, new OnSUIGenericCallback(OnPreviousBlock));
		};
		AddObjectToLayout("spoils_leftarrow_touch", false, sUITouchArea);
	}

	private void DrawRightArrow(Vector2 rightMostIconPosition)
	{
		rightMostIconPosition.x += 74f;
		SUISprite sUISprite = new SUISprite("Sprites/Menus/level_up_arrow");
		sUISprite.position = rightMostIconPosition;
		sUISprite.priority = mPriority;
		sUISprite.scale = kRightArrowScale;
		AddObjectToLayout("spoils_rightarrow_sprite", true, sUISprite);
		SUITouchArea sUITouchArea = new SUITouchArea(new Rect(rightMostIconPosition.x - 64f, rightMostIconPosition.y - 64f, 128f, 128f));
		sUITouchArea.onAreaTouched = delegate
		{
			mDelayedCalls = (OnSUIGenericCallback)Delegate.Combine(mDelayedCalls, new OnSUIGenericCallback(OnNextBlock));
		};
		AddObjectToLayout("spoils_rightarrow_touch", false, sUITouchArea);
	}
}
