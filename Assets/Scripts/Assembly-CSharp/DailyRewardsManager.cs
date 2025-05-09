using System;
using Debug = UnityEngine.Debug;

public class DailyRewardsManager
{
	public static IDialog CheckForDailyReward()
	{
		IDialog result = null;
		Profile instance = Singleton<Profile>.instance;
		DateTime now = DateTime.Now;
		if (instance.lastYearPlayed == 0 || instance.lastMonthPlayed == 0 || instance.lastDayPlayed == 0)
		{
			instance.lastYearPlayed = now.Year;
			instance.lastMonthPlayed = now.Month;
			instance.lastDayPlayed = now.Day;
		}
		DateTime value = new DateTime(instance.lastYearPlayed, instance.lastMonthPlayed, instance.lastDayPlayed);
		TimeSpan timeSpan = now.Subtract(value);
		if (timeSpan.Days == 1)
		{
			instance.currentDailyRewardNumber++;
			if (instance.currentDailyRewardNumber > SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/DailyRewards").childCount)
			{
				instance.currentDailyRewardNumber = 1;
			}
			result = CreateDailyRewardDialog();
			instance.lastYearPlayed = now.Year;
			instance.lastMonthPlayed = now.Month;
			instance.lastDayPlayed = now.Day;
			instance.Save();
		}
		else if (timeSpan.Days > 1)
		{
			if (instance.currentDailyRewardNumber > 1)
			{
				result = CreateMissedDailyRewardDialog();
			}
			instance.currentDailyRewardNumber = 0;
			instance.Save();
			instance.lastYearPlayed = now.Year;
			instance.lastMonthPlayed = now.Month;
			instance.lastDayPlayed = now.Day;
		}
		return result;
	}

	private static IDialog CreateDailyRewardDialog()
	{
		return new DailyRewardDialog(Singleton<Localizer>.instance.Get("Daily_Rewards"), Singleton<Localizer>.instance.Get("Daily_Reward_Unlock_Subtitle"), Singleton<Localizer>.instance.Get("Daily_Reward_Unlock_Body"), delegate
		{
		});
	}

	private static IDialog CreateMissedDailyRewardDialog()
	{
		Debug.Log("ShowMissedDailyRewardScreen() called");
		return null;
	}
}
