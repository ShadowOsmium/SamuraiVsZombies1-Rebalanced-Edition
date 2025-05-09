using System.Collections.Generic;
using UnityEngine;

public class Achievements : SingletonMonoBehaviour<Achievements>
{
	private const float kPriority = 50000f;

	private const float kYOut = -26f;

	private const float kYIn = 32f;

	private const float kSpeed = 0.3f;

	private const float kShowingTimer = 2f;

	private const int kTextMaxWidth = 390;

	private readonly Vector2 kTextOffset = new Vector2(18f, 0f);

	private SUISprite mPopup;

	private SUILabel mText;

	private float mYPos = -26f;

	private float mSlideDir;

	private float mWaitTimer;

	private List<string> mQueue = new List<string>();

	private float deltaTime
	{
		get
		{
			float num = ((Time.timeScale != 1f) ? SUIScreen.deltaTime : Time.deltaTime);
			if (num > 0.1f)
			{
				num = 0.1f;
			}
			return num;
		}
	}

	private Vector2 position
	{
		get
		{
			return mPopup.position;
		}
		set
		{
			if (mPopup != null)
			{
				mPopup.position = value;
				mText.position = value + kTextOffset;
			}
		}
	}

	public void Init()
	{
	}

	public void Update()
	{
		if (mPopup != null)
		{
			UpdatePopup();
		}
		else
		{
			CheckForNextPopup();
		}
	}

	public void Award(string gameCenterAchievementID)
	{
		if (!AddToQueue(gameCenterAchievementID))
		{
		}
	}

	private bool AddToQueue(string gameCenterAchievementID)
	{
		List<string> achievementsDisplayed = Singleton<Profile>.instance.GetAchievementsDisplayed();
		if (achievementsDisplayed.Contains(gameCenterAchievementID))
		{
			return false;
		}
		achievementsDisplayed.Add(gameCenterAchievementID);
		Singleton<Profile>.instance.SetAchievementsDisplayed(achievementsDisplayed);
		return true;
	}

	private void StartNewSlideAnimation()
	{
		mYPos = -26f;
		mSlideDir = 1f;
		mWaitTimer = 0f;
	}

	private void SpawnPopup()
	{
		mPopup = new SUISprite("Sprites/Menus/gamecenter_popup");
		mPopup.priority = 50000f;
		mText = new SUILabel("default18");
		mText.priority = 50000.1f;
		mText.maxWidth = 390;
		mText.maxLines = 1;
		mText.anchor = TextAnchor.MiddleCenter;
		mText.alignment = TextAlignment.Center;
		position = new Vector2(512f, mYPos);
	}

	private void UpdatePopup()
	{
		if (mSlideDir == 0f)
		{
			mWaitTimer += deltaTime;
			if (mWaitTimer >= 2f)
			{
				mSlideDir = -1f;
			}
			return;
		}
		Vector2 vector = mPopup.position;
		vector.y += 193.33333f * deltaTime * mSlideDir;
		if (vector.y <= -26f)
		{
			vector.y = -26f;
			CheckForNextPopup();
		}
		else if (vector.y >= 32f)
		{
			vector.y = 32f;
			mWaitTimer = 0f;
			mSlideDir = 0f;
		}
		position = vector;
	}

	private void PopAndDrawNextText()
	{
		string id = mQueue[0];
		mQueue.RemoveAt(0);
		string empty = string.Empty;
		empty = ((!Singleton<Localizer>.instance.Has(id)) ? Singleton<Localizer>.instance.Get("achievement_notext") : Singleton<Localizer>.instance.Get(id));
		mText.text = empty;
	}

	private void CheckForNextPopup()
	{
		if (mQueue.Count > 0)
		{
			StartNewSlideAnimation();
			if (mPopup == null)
			{
				SpawnPopup();
			}
			PopAndDrawNextText();
		}
		else if (mPopup != null)
		{
			mPopup.Destroy();
			mPopup = null;
			mText.Destroy();
			mText = null;
		}
	}
}
