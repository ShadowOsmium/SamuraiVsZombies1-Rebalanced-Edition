using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Profile : Singleton<Profile>
{
	public enum EItemType
	{
		eItemType_Charm = 0,
		eItemType_Potion = 1
	}

	private const int kMaxCurrencies = 999999999;

	private const int kMaxWaveLevel = 99;

	private const int kMoneyBagsAchievementAmount = 15000;

	private const int kDontSpendOnePlaceAchievementAmount = 10000;

	private const int kNumberOfFarmersToUpGradeForEveryAbleBodyAchievment = 25;

	private const int kNumberOfArchersToUpGradeForWeNeedShotgunsAchievment = 25;

	private const int kUndeadKilledViaTroopTrampleForClearTheRoomAchievement = 250;

	private const int kUndeadKilledViaKatanaSlashForRazorSharpAchievement = 100;

	private const int kWavesWithNoGateDamageForMustProtectHouseAchievment = 10;

	private const int kNumberOfOniToKillForSendThemBackAchievement = 50;

	private const int kNumberOfChochinobakeToKillForGetBackHereAchievement = 50;

	private const int kWaveToUnlockZombieMode = 10;

	private SavedData mSavedData;

	private SavedData mCloudSavedData;

	private SavedData mBonusWaveData;

	private bool mForcingBaseSaveData;

	private bool m_needSave;

	private bool m_inSaving;

	public bool neverShowRateMeAlertAgain
	{
		get
		{
			return mSavedData.GetValueBool("neverShowRateMeAlertAgain");
		}
		set
		{
			mSavedData.SetValueBool("neverShowRateMeAlertAgain", value);
		}
	}

	public bool wasBasicGameBeaten
	{
		get
		{
			return GetWaveLevel(maxBaseWave) > 1;
		}
	}

	public float timeStamp
	{
		get
		{
			return mSavedData.GetValueFloat("timeStamp");
		}
		set
		{
			mSavedData.SetValueFloat("timeStamp", value);
		}
	}

	public int timeRemainingUntilNotification
	{
		get
		{
			return mSavedData.GetValueInt("timeRemainingUntilNotification");
		}
		set
		{
			mSavedData.SetValueInt("timeRemainingUntilNotification", value);
		}
	}

	public int lastYearPlayed
	{
		get
		{
			return mSavedData.GetValueInt("lastYearPlayed");
		}
		set
		{
			mSavedData.SetValueInt("lastYearPlayed", value);
		}
	}

	public int lastMonthPlayed
	{
		get
		{
			return mSavedData.GetValueInt("lastMonthPlayed");
		}
		set
		{
			mSavedData.SetValueInt("lastMonthPlayed", value);
		}
	}

	public int lastDayPlayed
	{
		get
		{
			return mSavedData.GetValueInt("lastDayPlayed");
		}
		set
		{
			mSavedData.SetValueInt("lastDayPlayed", value);
		}
	}

	public string profileName
	{
		get
		{
			return mSavedData.GetValue("profileName");
		}
		set
		{
			mSavedData.SetValue("profileName", value);
		}
	}

	public string latestDetectedOnlineVersion
	{
		get
		{
			return mSavedData.GetValue("latestDetectedOnlineVersion");
		}
		set
		{
			mSavedData.SetValue("latestDetectedOnlineVersion", value);
		}
	}

	public bool newVersionDetected
	{
		get
		{
			return activeSavedData.GetValueBool("newVersionDetected");
		}
		set
		{
			mSavedData.SetValueBool("newVersionDetected", value);
		}
	}

	public int currentDailyRewardNumber
	{
		get
		{
			return mSavedData.GetValueInt("currentDailyRewardNumber");
		}
		set
		{
			mSavedData.SetValueInt("currentDailyRewardNumber", value);
		}
	}

	public bool tutorialIsComplete
	{
		get
		{
			return GetWaveLevel(3) > 0;
		}
	}

	public bool hasReportedUser
	{
		get
		{
			return mSavedData.GetValueBool("hasReportedUser");
		}
		set
		{
			mSavedData.SetValueBool("hasReportedUser", value);
		}
	}

	public bool hasReportedGemPurchase
	{
		get
		{
			return mSavedData.GetValueBool("hasReportedGemPurchase");
		}
		set
		{
			mSavedData.SetValueBool("hasReportedGemPurchase", value);
		}
	}

	public int latestOnlineBundleVersion
	{
		get
		{
			return mSavedData.GetValueInt("onlineBundleVersion");
		}
		set
		{
			mSavedData.SetValueInt("onlineBundleVersion", value);
		}
	}

	public bool hasWonPachinkoBefore
	{
		get
		{
			return mSavedData.GetValueBool("hasWonPachinkoBefore");
		}
		set
		{
			mSavedData.SetValueBool("hasWonPachinkoBefore", value);
		}
	}

	public int coins
	{
		get
		{
			return mSavedData.GetValueInt("coins");
		}
		set
		{
			if (value > 15000)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_MONEY_BAGS");
			}
			else if (value > 10000)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_DONT_SPEND_ONE_PLACE");
			}
			PlayerPrefs.SetInt("asvz.coins", Mathf.Clamp(value, 0, 999999999));
			mSavedData.SetValueInt("coins", Mathf.Clamp(value, 0, 999999999));
		}
	}

	public int purchasedCoins
	{
		get
		{
			return mSavedData.GetValueInt("purchasedCoins");
		}
		set
		{
			mSavedData.SetValueInt("purchasedCoins", Mathf.Clamp(value, 0, 999999999));
		}
	}

	public bool earnsDoubleCoins
	{
		get
		{
			return mSavedData.GetValueBool("earnsDoubleCoins");
		}
		set
		{
			mSavedData.SetValueBool("earnsDoubleCoins", value);
		}
	}

	public int gems
	{
		get
		{
			return mSavedData.GetValueInt("gems");
		}
		set
		{
			PlayerPrefs.SetInt("asvz.gems", Mathf.Clamp(value, 0, 999999999));
			mSavedData.SetValueInt("gems", Mathf.Clamp(value, 0, 999999999));
		}
	}

	public int purchasedGems
	{
		get
		{
			return mSavedData.GetValueInt("purchasedGems");
		}
		set
		{
			mSavedData.SetValueInt("purchasedGems", Mathf.Clamp(value, 0, 999999999));
		}
	}

	public int pachinkoBalls
	{
		get
		{
			return mSavedData.GetValueInt("pachinkoBalls");
		}
		set
		{
			mSavedData.SetValueInt("pachinkoBalls", Mathf.Clamp(value, 0, 999999999));
		}
	}

	public int pachinkoBallsLaunched
	{
		get
		{
			return mSavedData.GetValueInt("pachinkoBallsLaunched");
		}
		set
		{
			mSavedData.SetValueInt("pachinkoBallsLaunched", Mathf.Clamp(value, 0, 999999999));
		}
	}

	public int freeBoosterPacks
	{
		get
		{
			return mSavedData.GetValueInt("fbp");
		}
		set
		{
			mSavedData.SetValueInt("fbp", value);
		}
	}

	public int storeVisitCount
	{
		get
		{
			return mSavedData.GetValueInt("storeVisitsCount");
		}
		set
		{
			mSavedData.SetValueInt("storeVisitsCount", value);
		}
	}

	public int wavesSinceLastBonusWave
	{
		get
		{
			return mSavedData.GetValueInt("wavesSinceLastBonusWave");
		}
		set
		{
			mSavedData.SetValueInt("wavesSinceLastBonusWave", value);
		}
	}

	public bool readyToStartBonusWave
	{
		get
		{
			return Singleton<PlayModesManager>.instance.selectedMode == "classic" && wavesSinceLastBonusWave >= int.Parse(Singleton<Config>.instance.root.to("Game")["wavesPerBonusWave"]);
		}
	}

	public int bonusWaveToBeat
	{
		get
		{
			return Mathf.Max(1, mSavedData.GetValueInt("bonusWaveToBeat"));
		}
		set
		{
			int val = ValidateBonusWaveIndex(value);
			mSavedData.SetValueInt("bonusWaveToBeat", val);
		}
	}

	public int waveToBeat
	{
		get
		{
			if (inBonusWave)
			{
				return bonusWaveToBeat;
			}
			return Mathf.Max(1, mSavedData.GetValueInt("waveToBeat", playModeSubSection));
		}
		set
		{
			int num = ValidateWaveIndex(value);
			mSavedData.SetValueInt("waveToBeat", num, playModeSubSection);
			if (GetWaveLevel(num) == 0)
			{
				SetWaveLevel(num, 1);
			}
		}
	}

	public bool inBonusWave
	{
		get
		{
			return mBonusWaveData != null;
		}
	}

	public SavedData activeSavedData
	{
		get
		{
			return (!inBonusWave || mForcingBaseSaveData) ? mSavedData : mBonusWaveData;
		}
	}

	public SavedData saveData
	{
		get
		{
			return mSavedData;
		}
	}

	public SavedData cloudSaveData
	{
		get
		{
			return mCloudSavedData;
		}
	}

	public int heroLevel
	{
		get
		{
			return Mathf.Max(1, mSavedData.GetValueInt("heroLevel", playModeSubSection));
		}
		set
		{
			mSavedData.SetValueInt("heroLevel", Mathf.Max(0, value), playModeSubSection);
		}
	}

	public string heroID
	{
		get
		{
			string value = mSavedData.GetValue("heroID", playModeSubSection);
			if (value == string.Empty)
			{
				return Singleton<PlayModesManager>.instance.selectedModeData["defaultHeroID"];
			}
			return value;
		}
		set
		{
			mSavedData.SetValue("heroID", value, playModeSubSection);
		}
	}

	public int initialSmithyLevel
	{
		get
		{
			return Mathf.Max(1, activeSavedData.GetValueInt("initialLeadershipLevel", playModeSubSection));
		}
		set
		{
			mSavedData.SetValueInt("initialLeadershipLevel", Mathf.Max(1, value), playModeSubSection);
		}
	}

	public int swordLevel
	{
		get
		{
			return Mathf.Max(1, mSavedData.GetValueInt("swordLevel", playModeSubSection));
		}
		set
		{
			mSavedData.SetValueInt("swordLevel", Mathf.Max(0, value), playModeSubSection);
		}
	}

	public string swordID
	{
		get
		{
			string value = mSavedData.GetValue("swordID", playModeSubSection);
			if (value == string.Empty)
			{
				return Singleton<PlayModesManager>.instance.selectedModeData["defaultMeleeWeaponID"];
			}
			return value;
		}
		set
		{
			mSavedData.SetValue("swordID", value, playModeSubSection);
		}
	}

	public int bowLevel
	{
		get
		{
			return Mathf.Max(1, mSavedData.GetValueInt("bowLevel", playModeSubSection));
		}
		set
		{
			mSavedData.SetValueInt("bowLevel", Mathf.Max(0, value), playModeSubSection);
		}
	}

	public string bowID
	{
		get
		{
			string value = mSavedData.GetValue("bowID", playModeSubSection);
			if (value == string.Empty)
			{
				return Singleton<PlayModesManager>.instance.selectedModeData["defaultRangeWeaponID"];
			}
			return value;
		}
		set
		{
			mSavedData.SetValue("bowID", value, playModeSubSection);
		}
	}

	public int archerLevel
	{
		get
		{
			return activeSavedData.GetValueInt("archerLevel", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("archerLevel", value, playModeSubSection);
		}
	}

	public int baseLevel
	{
		get
		{
			return Mathf.Max(1, activeSavedData.GetValueInt("baseLevel", playModeSubSection));
		}
		set
		{
			mSavedData.SetValueInt("baseLevel", Mathf.Max(0, value), playModeSubSection);
		}
	}

	public int bellLevel
	{
		get
		{
			return activeSavedData.GetValueInt("bellLevel", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("bellLevel", value, playModeSubSection);
		}
	}

	public int undeathLevel
	{
		get
		{
			return activeSavedData.GetValueInt("undeathLevel", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("undeathLevel", value, playModeSubSection);
		}
	}

	public int legionOnTheLooseLevel
	{
		get
		{
			return activeSavedData.GetValueInt("legionOnTheLoose", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("legionOnTheLoose", value, playModeSubSection);
		}
	}

	public int deathFromAboveLevel
	{
		get
		{
			return activeSavedData.GetValueInt("deathFromAbove", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("deathFromAbove", value, playModeSubSection);
		}
	}

	public bool fromTheShadows
	{
		get
		{
			return activeSavedData.GetValueBool("fromTheShadows", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueBool("fromTheShadows", value, playModeSubSection);
			if (value)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_WITHOUT_A_TRACE");
			}
		}
	}

	public List<string> novelties
	{
		get
		{
			return mSavedData.GetSimpleList("novelties", playModeSubSection);
		}
		set
		{
			mSavedData.SetSimpleList("novelties", value, playModeSubSection);
		}
	}

	public List<string> alreadyEarnedAchievements
	{
		get
		{
			return mSavedData.GetSimpleList("alreadyEarnedAchievements");
		}
		set
		{
			mSavedData.SetSimpleList("alreadyEarnedAchievements", value);
		}
	}

	public List<string> giveOnceCompleted
	{
		get
		{
			return mSavedData.GetSimpleList("giveOnce");
		}
		set
		{
			mSavedData.SetSimpleList("giveOnce", value);
		}
	}

	public bool zombieModeUnlocked
	{
		get
		{
			if (Singleton<PlayModesManager>.instance.selectedMode != "classic")
			{
				return true;
			}
			if (activeSavedData.GetValueBool("zombieMode"))
			{
				return true;
			}
			return highestUnlockedWave > 10;
		}
	}

	public bool zombieModeUnlockedMessageShown
	{
		get
		{
			return activeSavedData.GetValueBool("zombieModeUnlockedMessageShown");
		}
		set
		{
			mSavedData.SetValueBool("zombieModeUnlockedMessageShown", value);
		}
	}

	public int upgradedFarmers
	{
		get
		{
			return mSavedData.GetValueInt("upgradedFarmers", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("upgradedFarmers", value, playModeSubSection);
			if (mSavedData.GetValueInt("upgradedFarmers", playModeSubSection) > 25)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_EVERY_ABLE_BODY");
			}
		}
	}

	public int upgradedArchers
	{
		get
		{
			return mSavedData.GetValueInt("upgradedArchers", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("upgradedArchers", value, playModeSubSection);
			if (mSavedData.GetValueInt("upgradedArchers", playModeSubSection) > 25)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_WE_NEED_SHOTGUNS");
			}
		}
	}

	public int maxSelectedHelpers
	{
		get
		{
			return 5;
		}
	}

	public int UndeadKilledViaTroopTrample
	{
		get
		{
			return mSavedData.GetValueInt("UndeadKilledViaTroopTrample", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("UndeadKilledViaTroopTrample", value, playModeSubSection);
			if (value > 250)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_CLEAR_THE_ROOM");
			}
		}
	}

	public int UndeadKilledViaKatanaSlash
	{
		get
		{
			return mSavedData.GetValueInt("UndeadKilledViaKatanaSlash", playModeSubSection);
		}
		set
		{
			mSavedData.SetValueInt("UndeadKilledViaKatanaSlash", value, playModeSubSection);
			if (value > 100)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_RAZOR_SHARP");
			}
		}
	}

	public int maxSelectedAbilities
	{
		get
		{
			return 2;
		}
	}

	public int maxSelectedCharms
	{
		get
		{
			return 1;
		}
	}

	public string selectedCharm
	{
		get
		{
			return activeSavedData.GetValue("selectedCharm", playModeSubSection);
		}
		set
		{
			mSavedData.SetValue("selectedCharm", value, playModeSubSection);
		}
	}

	public int highestUnlockedWave
	{
		get
		{
			for (int i = 1; i < 1000; i++)
			{
				if (GetWaveLevel(i) == 0)
				{
					return Mathf.Max(1, i - 1);
				}
			}
			Debug.Log("WARNING: Something REALLY strange just happened.");
			return 1;
		}
	}

	public bool hasSummonedFarmer
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedFarmer");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedFarmer", value);
		}
	}

	public bool hasSummonedSwordWarrior
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedSwordWarrior");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedSwordWarrior", value);
		}
	}

	public bool hasSummonedSpearWarrior
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedSpearWarrior");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedSpearWarrior", value);
		}
	}

	public bool hasSummonedBowman
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedBowman");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedBowman", value);
		}
	}

	public bool hasSummonedPanzerSamurai
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedPanzerSamurai");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedPanzerSamurai", value);
		}
	}

	public bool hasSummonedPriest
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedPriest");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedPriest", value);
		}
	}

	public bool hasSummonedNobunaga
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedNobunaga");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedNobunaga", value);
		}
	}

	public bool hasSummonedSpearHorseman
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedSpearHorseman");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedSpearHorseman", value);
		}
	}

	public bool hasSummonedTakeda
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedTakeda");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedTakeda", value);
		}
	}

	public bool hasSummonedRifleman
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedRifleman");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedRifleman", value);
		}
	}

	public bool hasSummonedFrostie
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedFrostie");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedFrostie", value);
		}
	}

	public bool hasSummonedSwordsmith
	{
		get
		{
			return mSavedData.GetValueBool("hasSummonedSwordsmith");
		}
		set
		{
			mSavedData.SetValueBool("hasSummonedSwordsmith", value);
		}
	}

	public int wavesWithZeroGateDamage
	{
		get
		{
			return mSavedData.GetValueInt("wavesWithZeroGateDamage");
		}
		set
		{
			mSavedData.SetValueInt("wavesWithZeroGateDamage", value);
			if (value >= 10)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_MUST_PROTECT_HOUSE");
			}
		}
	}

	public int maxBaseWave
	{
		get
		{
			return int.Parse(Singleton<PlayModesManager>.instance.selectedModeData["maxBaseWave"]);
		}
	}

	public string playModeSubSection
	{
		get
		{
			string selectedMode = Singleton<PlayModesManager>.instance.selectedMode;
			if (selectedMode == "classic")
			{
				return null;
			}
			return selectedMode;
		}
	}

	public string saveFilePath
	{
		get
		{
			return Path.Combine(Application.persistentDataPath, "save.data");
		}
	}

	public int tapFeatureAdsCnt
	{
		get
		{
			return mSavedData.GetValueInt("tapFeatureAdsCnt");
		}
		set
		{
			mSavedData.SetValueInt("tapFeatureAdsCnt", value);
		}
	}

	public static string prevSaveFilePath
	{
		get
		{
			return Path.Combine(Application.persistentDataPath, "prevsave.data");
		}
	}

	public Profile()
	{
		m_needSave = false;
		m_inSaving = false;
		mSavedData = new SavedData(saveFilePath);
		Load();
		ClearBonusWaveData();
	}

	public void Save()
	{
		m_needSave = true;
	}

	public void SaveCloudFile(string cloudSavePath)
	{
		mCloudSavedData = new SavedData(cloudSavePath);
		mCloudSavedData.SetSDFTreeNode(mSavedData.saveData.Clone());
		mCloudSavedData.Save();
	}

	private void Load()
	{
		if (!mSavedData.Load())
		{
			Debug.Log("***** RESETTING DATA ****");
			ResetData();
		}
		PostLoadSyncing();
	}

	public void LoadBonusWaveData(SDFTreeNode data)
	{
		if (data == null)
		{
			ClearBonusWaveData();
			return;
		}
		mBonusWaveData = new SavedData(null);
		mBonusWaveData.SetSDFTreeNode(data);
	}

	public void ClearBonusWaveData()
	{
		mBonusWaveData = null;
	}

	public void ForceActiveSaveData(bool bonusWaveData)
	{
		mForcingBaseSaveData = !bonusWaveData;
	}

	public void ResetData()
	{
		int num = latestOnlineBundleVersion;
		List<string> achievementsDisplayed = GetAchievementsDisplayed();
		int num2 = tapFeatureAdsCnt;
		mSavedData.Clear();
		latestOnlineBundleVersion = num;
		SetAchievementsDisplayed(achievementsDisplayed);
		tapFeatureAdsCnt = num2;
		SetNumCharms("power", 1);
		SetNumCharms("luck", 1);
		PostLoadSyncing();
		mSavedData.Save();
	}

	public bool IsEventRewardsGiven(string eventRewardID)
	{
		return activeSavedData.GetDictionaryValue<bool>("eventRewards", eventRewardID);
	}

	public void SetEventRewardsGiven(string eventRewardID)
	{
		activeSavedData.SetDictionaryValue("eventRewards", eventRewardID, true);
	}

	public int GetHelperLevel(string helperID)
	{
		return Mathf.Clamp(activeSavedData.GetDictionaryValue<int>("helperLevels", helperID, playModeSubSection), 0, Singleton<HelpersDatabase>.instance.GetMaxLevel(helperID));
	}

	public void SetHelperLevel(string helperID, int val, bool autoFillSelectedHelpers)
	{
		val = Mathf.Clamp(val, 0, Singleton<HelpersDatabase>.instance.GetMaxLevel(helperID));
		if (autoFillSelectedHelpers && GetHelperLevel(helperID) == 0 && val > 0)
		{
			List<string> selectedHelpers = GetSelectedHelpers();
			if (selectedHelpers.Count < maxSelectedHelpers)
			{
				selectedHelpers.Add(helperID);
				SetSelectedHelpers(selectedHelpers);
			}
		}
		activeSavedData.SetDictionaryValue("helperLevels", helperID, val, playModeSubSection);
	}

	public List<string> GetSelectedHelpers()
	{
		return activeSavedData.GetSubNodeValueList("selectedHelpers", playModeSubSection);
	}

	public void SetSelectedHelpers(List<string> helpers)
	{
		activeSavedData.SetSubNodeValueList("selectedHelpers", helpers, playModeSubSection);
	}

	public int GetAbilityLevel(string abilityID)
	{
		return activeSavedData.GetDictionaryValue<int>("abilityLevels", abilityID, playModeSubSection);
	}

	public void SetAbilityLevel(string abilityID, int val)
	{
		activeSavedData.SetDictionaryValue("abilityLevels", abilityID, val, playModeSubSection);
	}

	public List<string> GetSelectedAbilities()
	{
		return activeSavedData.GetSubNodeValueList("selectedAbilities", playModeSubSection);
	}

	public void SetSelectedAbilities(List<string> abilities)
	{
		activeSavedData.SetSubNodeValueList("selectedAbilities", abilities, playModeSubSection);
	}

	public int GetNumCharms(string charmID)
	{
		return mSavedData.GetDictionaryValue<int>("numCharms", charmID);
	}

	public void SetNumCharms(string charmID, int val)
	{
		mSavedData.SetDictionaryValue("numCharms", charmID, val);
	}

	public int GetNumPotions(string potionID)
	{
		return mSavedData.GetDictionaryValue<int>("numPotions", potionID);
	}

	public void SetNumPotions(string potionID, int val)
	{
		mSavedData.SetDictionaryValue("numPotions", potionID, val);
	}

	public bool IsChapterAvailable(string chapterID)
	{
		return mSavedData.GetDictionaryValue<bool>("chapterUnlock", chapterID, playModeSubSection);
	}

	public void SetChapterAvailable(string chapterID, bool val)
	{
		mSavedData.SetDictionaryValue("chapterUnlock", chapterID, val, playModeSubSection);
	}

	public int GetWaveLevel(int waveIndex)
	{
		return (!inBonusWave) ? mSavedData.GetDictionaryValue<int>("waves", "w" + waveIndex, playModeSubSection) : 0;
	}

	public void SetWaveLevel(int waveIndex, int level)
	{
		if (GetWaveLevel(waveIndex) >= level)
		{
			return;
		}
		level = Mathf.Clamp(level, 0, 99);
		mSavedData.SetDictionaryValue("waves", "w" + waveIndex, level, playModeSubSection);
		if (level == 99)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_FINALLY");
		}
		int num = maxBaseWave;
		for (int i = 1; i <= num && GetWaveLevel(i) == 99; i++)
		{
			if (i == num)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_THAT_TOOK_FOREVER");
			}
		}
	}

	public bool IsTutorialDone(string module, string id)
	{
		return mSavedData.GetDictionaryValue<bool>("tutorials", module + "_" + id);
	}

	public void SetTutorialDone(string module, string id)
	{
		mSavedData.SetDictionaryValue("tutorials", module + "_" + id, true);
	}

	public List<string> GetAchievementsDisplayed()
	{
		return mSavedData.GetSubNodeValueList("achievementsDisplayed");
	}

	public void SetAchievementsDisplayed(List<string> achievementsDisplayed)
	{
		mSavedData.SetSubNodeValueList("achievementsDisplayed", achievementsDisplayed);
	}

	public int GetNumKillsOfEnemyType(string enemyID)
	{
		return mSavedData.GetDictionaryValue<int>("stats_enemiesKilled", enemyID);
	}

	public void IncNumKillsOfEnemyType(string enemyID)
	{
		int num = GetNumKillsOfEnemyType(enemyID) + 1;
		mSavedData.SetDictionaryValue("stats_enemiesKilled", enemyID, num.ToString());
		switch (enemyID)
		{
		case "Oni":
		case "Oni_boss":
		case "Oni_small":
			if (GetNumKillsOfEnemyType("Oni") + GetNumKillsOfEnemyType("Oni_boss") + GetNumKillsOfEnemyType("Oni_small") > 50)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_SEND_THEM_BACK");
			}
			break;
		case "Chochinobake":
		case "Chochinobake_alt":
			if (GetNumKillsOfEnemyType("Chochinobake") + GetNumKillsOfEnemyType("Chochinobake_alt") > 50)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_GET_BACK_HERE");
			}
			break;
		case "Jurogumo":
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_SPIDER_DEMON");
			break;
		case "Shogun":
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_SO_LONG");
			break;
		}
	}

	public void PostLoadSyncing()
	{
		if (Singleton<PlayModesManager>.instance.selectedMode == "classic")
		{
			UpgradeWaveRecycling();
			UpgradeToExtraWaves();
		}
		waveToBeat = ValidateWaveIndex(waveToBeat);
		if (GetWaveLevel(1) == 0)
		{
			SetWaveLevel(1, 1);
		}
	}

	private int ValidateWaveIndex(int index)
	{
		if (index < 1)
		{
			return 1;
		}
		string path = "Registry/" + Singleton<PlayModesManager>.instance.selectedModeData["wavesPath"] + string.Format("{0:000}", index);
		if (SingletonMonoBehaviour<ResourcesManager>.instance.Open(path) == null)
		{
			index = 1;
		}
		return index;
	}

	private int ValidateBonusWaveIndex(int index)
	{
		if (index < 1)
		{
			return 1;
		}
		string path = "Registry/BonusWaves/BonusWave" + string.Format("{0:000}", index);
		if (SingletonMonoBehaviour<ResourcesManager>.instance.Open(path) == null)
		{
			index = 1;
		}
		return index;
	}

	private void UpgradeWaveRecycling()
	{
		int num = waveToBeat;
		int valueInt = mSavedData.GetValueInt("waveRecycle");
		if (GetWaveLevel(num) != 0)
		{
			return;
		}
		for (int i = 1; i <= 20; i++)
		{
			if (i < num)
			{
				SetWaveLevel(i, valueInt + 2);
			}
			else if (i == num)
			{
				SetWaveLevel(i, valueInt + 1);
			}
			else if (valueInt > 0)
			{
				SetWaveLevel(i, valueInt + 1);
			}
		}
		if (valueInt > 0)
		{
			mSavedData.SetValueInt("waveRecycle", 0);
			waveToBeat = 21;
		}
	}

	public void UpgradeToExtraWaves()
	{
		if (!ResourcesManager.initialized)
		{
			return;
		}
		int num = highestUnlockedWave;
		if (GetWaveLevel(num) > 1)
		{
			string path = "Registry/Waves/Wave" + string.Format("{0:000}", num + 1);
			if (SingletonMonoBehaviour<ResourcesManager>.instance.Open(path) != null)
			{
				SetWaveLevel(num + 1, 1);
				waveToBeat = num + 1;
			}
		}
	}

	public void ConvertLocalGemsToGWallet()
	{
	}

	public void Update()
	{
		SaveNow();
	}

	private void SaveNow()
	{
		if (m_needSave && !m_inSaving)
		{
			m_inSaving = true;
			if (Debug.isDebugBuild)
			{
				Debug.Log("ASYNCH_SAVING");
			}
			mSavedData.Save();
			m_inSaving = false;
			m_needSave = false;
		}
	}
}
