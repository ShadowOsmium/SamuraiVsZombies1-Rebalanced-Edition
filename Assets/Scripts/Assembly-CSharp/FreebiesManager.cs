using System;
using System.Collections.Generic;

public class FreebiesManager : Singleton<FreebiesManager>
{
	private class WelcomeBackData
	{
		public int minDays;

		public SDFTreeNode gifts;
	}

	private WelcomeBackData mWelcomeBack;

	private string mSceneToReturnToFromBoosterStore = string.Empty;

	private Dictionary<string, List<KeyValuePair<string, int>>> mGiveOnce = new Dictionary<string, List<KeyValuePair<string, int>>>();

	public bool isTimeForWelcomeBackGift
	{
		get
		{
			Profile profile = Singleton<Profile>.instance;
			DateTime now = DateTime.Now;
			if (profile.lastYearPlayed == 0 || profile.lastMonthPlayed == 0 || profile.lastDayPlayed == 0)
			{
				profile.lastYearPlayed = now.Year;
				profile.lastMonthPlayed = now.Month;
				profile.lastDayPlayed = now.Day;
			}
			DateTime value = new DateTime(profile.lastYearPlayed, profile.lastMonthPlayed, profile.lastDayPlayed);
			return now.Subtract(value).Days >= mWelcomeBack.minDays;
		}
	}

	public string sceneToReturnToFromBoosterStore
	{
		get
		{
			return mSceneToReturnToFromBoosterStore;
		}
		set
		{
			mSceneToReturnToFromBoosterStore = value;
		}
	}

	public FreebiesManager()
	{
		ResetCachedData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public IDialog CheckForWelcomeBackGift()
	{
		if (isTimeForWelcomeBackGift)
		{
			Singleton<Profile>.instance.currentDailyRewardNumber = 0;
			SaveCurrentDate();
			KeyValuePair<string, int> randomWelcomeGift = GetRandomWelcomeGift();
			return SpawnFreebiesDialog(randomWelcomeGift);
		}
		return null;
	}

	public IDialog CheckForGiveOnceGift()
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		List<string> giveOnceCompleted = Singleton<Profile>.instance.giveOnceCompleted;
		foreach (KeyValuePair<string, List<KeyValuePair<string, int>>> item in mGiveOnce)
		{
			bool flag = false;
			foreach (string item2 in giveOnceCompleted)
			{
				if (item2 == item.Key)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			giveOnceCompleted.Add(item.Key);
			foreach (KeyValuePair<string, int> item3 in item.Value)
			{
				list.Add(item3);
			}
		}
		if (list.Count > 0)
		{
			Singleton<Profile>.instance.giveOnceCompleted = giveOnceCompleted;
			return SpawnGiveOnceDialog(list);
		}
		return null;
	}

	private void ResetCachedData()
	{
		mWelcomeBack = new WelcomeBackData();
		mGiveOnce.Clear();
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Freebies");
		if (sDFTreeNode == null)
		{
			return;
		}
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to("welcomeBack");
		if (sDFTreeNode2 != null)
		{
			mWelcomeBack.minDays = int.Parse(sDFTreeNode2["minDays"]);
			mWelcomeBack.gifts = sDFTreeNode2.to("randomGift");
		}
		SDFTreeNode sDFTreeNode3 = sDFTreeNode.to("giveOnce");
		if (sDFTreeNode3 == null)
		{
			return;
		}
		foreach (KeyValuePair<string, SDFTreeNode> child in sDFTreeNode3.childs)
		{
			List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
			foreach (KeyValuePair<string, string> attribute in child.Value.attributes)
			{
				list.Add(new KeyValuePair<string, int>(attribute.Key, int.Parse(attribute.Value)));
			}
			if (list.Count > 0)
			{
				mGiveOnce.Add(child.Key, list);
			}
		}
	}

	private void SaveCurrentDate()
	{
		Profile profile = Singleton<Profile>.instance;
		DateTime now = DateTime.Now;
		profile.lastYearPlayed = now.Year;
		profile.lastMonthPlayed = now.Month;
		profile.lastDayPlayed = now.Day;
		profile.Save();
	}

	private KeyValuePair<string, int> GetRandomWelcomeGift()
	{
		int num = RandomRangeInt.between(0, mWelcomeBack.gifts.attributeCount - 1);
		foreach (KeyValuePair<string, string> attribute in mWelcomeBack.gifts.attributes)
		{
			if (num == 0)
			{
				return new KeyValuePair<string, int>(attribute.Key, int.Parse(attribute.Value));
			}
			num--;
		}
		throw new Exception("Random Welcome Back Gift index out of range.");
	}

	private IDialog SpawnFreebiesDialog(KeyValuePair<string, int> reward)
	{
		List<SpoilsDisplay.Entry> list = new List<SpoilsDisplay.Entry>();
		list.Add(SpoilsDisplay.BuildEntry(reward.Key, reward.Value));
		if (CashIn.GetType(reward.Key) == CashIn.ItemType.BoosterPack)
		{
			sceneToReturnToFromBoosterStore = "MainMenu";
		}
		return new DailyRewardDialog(Singleton<Localizer>.instance.Get("welcomeBack_title"), Singleton<Localizer>.instance.Get("welcomeBack_subtitle"), Singleton<Localizer>.instance.Get("welcomeBack_body"), null, list);
	}

	private IDialog SpawnGiveOnceDialog(List<KeyValuePair<string, int>> rewardsList)
	{
		List<SpoilsDisplay.Entry> list = new List<SpoilsDisplay.Entry>();
		foreach (KeyValuePair<string, int> rewards in rewardsList)
		{
			list.Add(SpoilsDisplay.BuildEntry(rewards.Key, rewards.Value));
		}
		return new DailyRewardDialog(Singleton<Localizer>.instance.Get("giveonce_title"), Singleton<Localizer>.instance.Get("giveonce_subtitle"), Singleton<Localizer>.instance.Get("giveonce_body"), null, list);
	}
}
