using System.Collections.Generic;
using UnityEngine;

public class DailyRewardDialog : IDialog
{
	private const int kDailyRewardDialogPriority = 1001;

	private const int kIconDelta = 140;

	private SUILayout mLayout;

	private bool mDone;

	private bool mInChargeOfFadeOut;

	private bool mUseDayLabels = true;

	private int mGiftIndexToOpen = -1;

	private List<SpoilsDisplay.Entry> mDailyRewards = new List<SpoilsDisplay.Entry>();

	public OnSUIGenericCallback onYesPressed
	{
		get
		{
			return ((SUIButton)mLayout["yesBtn"]).onButtonPressed;
		}
		set
		{
			((SUIButton)mLayout["yesBtn"]).onButtonPressed = delegate
			{
				if (value != null)
				{
					value();
				}
				Close();
			};
		}
	}

	public bool isDone
	{
		get
		{
			return mDone && !mLayout.isAnimating;
		}
	}

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public float priority
	{
		get
		{
			return mLayout.basePriority;
		}
		set
		{
			mLayout.basePriority = value;
		}
	}

	public DailyRewardDialog(string title, string subtitle, string bodytext, OnSUIGenericCallback onYes)
	{
		Init(title, subtitle, bodytext, onYes, null);
	}

	public DailyRewardDialog(string title, string subtitle, string bodytext, OnSUIGenericCallback onYes, List<SpoilsDisplay.Entry> rewards)
	{
		Init(title, subtitle, bodytext, onYes, rewards);
	}

	private void Init(string title, string subtitle, string bodytext, OnSUIGenericCallback onYes, List<SpoilsDisplay.Entry> rewards)
	{
		mLayout = new SUILayout("Layouts/DailyRewardDialog");
		mInChargeOfFadeOut = true;
		((SUILabel)mLayout["title64"]).text = title;
		((SUILabel)mLayout["title32"]).text = subtitle;
		((SUILabel)mLayout["title18"]).text = bodytext;
		if (rewards != null)
		{
			mGiftIndexToOpen = 1;
			mDailyRewards = rewards;
			mUseDayLabels = false;
		}
		else
		{
			mGiftIndexToOpen = Singleton<Profile>.instance.currentDailyRewardNumber;
			BuildDailyRewardsFromRegistry();
		}
		int num = 0;
		Vector2 vector = new Vector2(SUIScreen.width / 2f, SUIScreen.height / 2f - 20f);
		Vector2 pos = new Vector2(vector.x - (float)((mDailyRewards.Count - 1) * 140 / 2), vector.y);
		foreach (SpoilsDisplay.Entry mDailyReward in mDailyRewards)
		{
			num++;
			DrawEntry(mLayout, "default18", pos, 1001f, 140f, mDailyReward, num);
			pos.x += 140f;
		}
		onYesPressed = onYes;
		((SUIButton)mLayout["yesBtn"]).text = Singleton<Localizer>.instance.Get("ok");
		((SUIButton)mLayout["back"]).visible = false;
		mLayout.AnimateIn();
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
	}

	public void Update()
	{
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Close();
		}
	}

	private void Close()
	{
		((SUIButton)mLayout["yesBtn"]).onButtonPressed = null;
		mDone = true;
		mLayout.AnimateOut();
		if (mInChargeOfFadeOut && WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
	}

	private void BuildDailyRewardsFromRegistry()
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/DailyRewards");
		for (int i = 1; i <= sDFTreeNode.childCount; i++)
		{
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to("day" + i);
			if (sDFTreeNode2 == null)
			{
				break;
			}
			foreach (KeyValuePair<string, string> attribute in sDFTreeNode2.attributes)
			{
				AddToSpoil(attribute.Key, int.Parse(attribute.Value));
			}
		}
	}

	private void DrawEntry(SUILayout layout, string fontName, Vector2 pos, float priority, float delta, SpoilsDisplay.Entry entry, int numPresentsSoFar)
	{
		string text = string.Empty;
		if (numPresentsSoFar >= mGiftIndexToOpen)
		{
			switch (numPresentsSoFar)
			{
			case 1:
			case 2:
				text = "Sprites/Icons/present_red";
				break;
			case 3:
			case 4:
				text = "Sprites/Icons/present_blue";
				break;
			case 5:
				text = "Sprites/Icons/present_yellow";
				break;
			}
		}
		int num = entry.count;
		string text2 = entry.text;
		if (entry.id == "coins" || entry.id == "gems" || entry.id == "balls")
		{
			text2 = entry.count.ToString();
			num = 1;
		}
		if (mUseDayLabels)
		{
			SUISprite sUISprite = (SUISprite)layout["day" + numPresentsSoFar];
			Vector2 vector = new Vector2(25f, 0f);
			sUISprite.position = vector + pos + new Vector2(0f, 120f);
			sUISprite.visible = true;
		}
		PresentOpener presentOpener = new PresentOpener(text, entry.icon, text2, fontName, (int)delta - 16, num);
		if (presentOpener.label.shownLines > 3)
		{
			presentOpener.label.maxLines = 3;
		}
		presentOpener.priority = priority;
		presentOpener.position = pos;
		if (text != string.Empty)
		{
			if (numPresentsSoFar == mGiftIndexToOpen)
			{
				presentOpener.Open(0.7f, 1f + 0.4f * (float)numPresentsSoFar);
				string id = entry.id;
				switch (id)
				{
				case "coins":
				case "gems":
				case "balls":
					CashIn.From(id, text2, "Daily Bonus", "CREDIT_IN_GAME_AWARD");
					break;
				default:
					CashIn.From(id, 1, "Daily Bonus", "CREDIT_IN_GAME_AWARD");
					break;
				}
				Singleton<Profile>.instance.Save();
			}
		}
		else
		{
			presentOpener.frame = 1f;
		}
		AddObjectWithFade("spoils_" + entry.id, layout, presentOpener);
	}

	private void AddObjectWithFade(string name, SUILayout layout, SUIProcess objToAdd)
	{
		SUILayout.ObjectData objectData = new SUILayout.ObjectData();
		objectData.obj = objToAdd;
		objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
		layout.Add(name, objectData);
	}

	private void AddToSpoil(string id, int count)
	{
		SpoilsDisplay.Entry item = SpoilsDisplay.BuildEntry(id, count);
		mDailyRewards.Add(item);
	}
}
