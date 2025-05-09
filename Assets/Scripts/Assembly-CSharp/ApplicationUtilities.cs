using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationUtilities : MonoBehaviour
{
	public enum AdPosition
	{
		Bottom = 1,
		Top = 2,
		Left = 4,
		Right = 8,
		Center = 0x10
	}

	public enum AdPositionCombinations
	{
		TopLeft = 6,
		TopRight = 10,
		BottomLeft = 5,
		BottomRight = 9,
		TopCenter = 18,
		BottomCenter = 17,
		RightCenter = 24,
		LeftCenter = 20
	}

	private const string google_market_url = "market://details?id=com.glu.samuzombie";

	private const string amazon_market_url = "amzn://apps/android?p=com.glu.samuzombie";

	private const string google_game_sku = "SAMURAI_ZOMBIES_GOOGLE";

	private const string amazon_game_sku = "SAMURAI_ZOMBIES_AMAZON";

	private const string server_url_live_mask = "http://gluservices.s3.amazonaws.com/PushNotifications/{0}/notifications.txt";

	private const string server_url_stage = "http://griptonite.s3.amazonaws.com/svz/android/server_notifications/";

	private const string google_tj_app_id = "aebed7dc-779c-4c99-83c7-a1b6cd2c0449";

	private const string amazon_tj_app_id = "f426b9b1-50f0-44ca-93c5-80e270e56702";

	private const string google_tj_secret_key = "8NPEIRNflfy2dfVQGTIL";

	private const string amazon_tj_secret_key = "PfbLntqseDDjeTBUqo8C";

	private const string ph_amazon_token = "8c7c0730d91a429aaab28f683001eecd";

	private const string ph_google_token = "31873c6e4392466284232fd9b5b6a9a5";

	private const string ph_amazon_secret = "792a2ff54c0748b09f029fdf1a16f714";

	private const string ph_google_secret = "cdaea7e9ab934703b28b9b1d37c090b6";

	private const float DAILY_REWARDS_CHECK_TIME = 10f;

	private static ApplicationUtilities This;

	public bool bIsApplicationPaused;

	public bool bShouldTimeoutIAPs;

	private List<string> processedIDs = new List<string>();

	private bool mShowGameLauchPlayHavenAds;

	private static bool mFirstGameLaunch = true;

	private bool mIsInitialized;

	private bool mGameCenterLoggedIn;

	private bool mShouldPromptForInternetConnection;

	private float mTimeTillCheckDailyRewardsAgain = 3f;

	public int receivedPoints;

	public static string MarketURL
	{
		get
		{
			if (IsBuildType("amazon"))
			{
				return "amzn://apps/android?p=com.glu.samuzombie";
			}
			return "market://details?id=com.glu.samuzombie";
		}
	}

	public static string GAME_SKU
	{
		get
		{
			if (IsBuildType("amazon"))
			{
				return "SAMURAI_ZOMBIES_AMAZON";
			}
			return "SAMURAI_ZOMBIES_GOOGLE";
		}
	}

	public static string SERVER_NOTIFICATION_URL
	{
		get
		{
			if (IsBuildType("amazon"))
			{
				if (Debug.isDebugBuild)
				{
					return "http://griptonite.s3.amazonaws.com/svz/android/server_notifications/amazon_sn-debug.txt";
				}
				return string.Format("http://gluservices.s3.amazonaws.com/PushNotifications/{0}/notifications.txt", GAME_SKU);
			}
			if (Debug.isDebugBuild)
			{
				return "http://griptonite.s3.amazonaws.com/svz/android/server_notifications/sn-debug.txt";
			}
			return string.Format("http://gluservices.s3.amazonaws.com/PushNotifications/{0}/notifications.txt", GAME_SKU);
		}
	}

	public static string TJ_APP_ID
	{
		get
		{
			if (IsBuildType("amazon"))
			{
				return "f426b9b1-50f0-44ca-93c5-80e270e56702";
			}
			return "aebed7dc-779c-4c99-83c7-a1b6cd2c0449";
		}
	}

	public static string TJ_SECRET_KEY
	{
		get
		{
			if (IsBuildType("amazon"))
			{
				return "PfbLntqseDDjeTBUqo8C";
			}
			return "8NPEIRNflfy2dfVQGTIL";
		}
	}

	public static string lockToken
	{
		get
		{
			if (IsBuildType("amazon"))
			{
				return "8c7c0730d91a429aaab28f683001eecd";
			}
			if (IsBuildType("google"))
			{
				return "31873c6e4392466284232fd9b5b6a9a5";
			}
			return string.Empty;
		}
	}

	public static string lockSecret
	{
		get
		{
			if (IsBuildType("amazon"))
			{
				return "792a2ff54c0748b09f029fdf1a16f714";
			}
			if (IsBuildType("google"))
			{
				return "cdaea7e9ab934703b28b9b1d37c090b6";
			}
			return string.Empty;
		}
	}

	public static ApplicationUtilities instance
	{
		get
		{
			if (This == null)
			{
				GameObject gameObject = new GameObject("ApplicationUtilities", typeof(ApplicationUtilities));
				This = gameObject.GetComponent<ApplicationUtilities>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				Screen.sleepTimeout = 0;
				if (Debug.isDebugBuild)
				{
					Debug.Log("ApplicationUtilities instance");
				}
			}
			return This;
		}
	}

	public bool mustShowGameLauchPlayHavenAds
	{
		get
		{
			return mShowGameLauchPlayHavenAds || mFirstGameLaunch;
		}
		set
		{
			mShowGameLauchPlayHavenAds = value;
			if (!value)
			{
				mFirstGameLaunch = false;
			}
		}
	}

	public bool ShouldPromptForInternetConnection
	{
		get
		{
			return mShouldPromptForInternetConnection;
		}
		set
		{
			mShouldPromptForInternetConnection = value;
		}
	}

	public bool GameCenterIsLoggedIn
	{
		get
		{
			return mGameCenterLoggedIn;
		}
		set
		{
			mGameCenterLoggedIn = value;
		}
	}

	private bool allowTapJoyVideos
	{
		get
		{
			return true;
		}
	}

	public event OnSUIBoolCallback onApplicationPause;

	public event OnSUIGenericCallback onApplicationQuit;

	public static bool IsBuildType(string bt)
	{
		return false;
	}

	public int GGN_BUG()
	{
		return 0;
	}

	public static bool IsGWalletAvailable()
	{
		return false;
	}

	public static void GWalletBalance(int change, string desc, string activity)
	{
		PlayerPrefs.SetInt("asvz.gems", Singleton<Profile>.instance.gems);
		Singleton<Profile>.instance.gems += change;
		Debug.Log("Added Gems: " + desc + ", " + activity);
	}

	public static bool GWalletIsSubscriberToPlan(string plan)
	{
		return false;
	}

	public static OnlineItemsManager.Item GWalletGetSubscriptionRecommendationAtIndex(int idx)
	{
		OnlineItemsManager.Item item = null;
		return item;
	}

	public void Init()
	{
		if (mIsInitialized)
		{
			return;
		}
		mIsInitialized = true;
		if (Debug.isDebugBuild)
		{
			string text = SystemInfo.processorType + ", " + SystemInfo.processorCount + ", " + SystemInfo.systemMemorySize + ", " + SystemInfo.graphicsMemorySize + ", " + SystemInfo.graphicsDeviceName + ", " + SystemInfo.graphicsDeviceVendor + ", " + SystemInfo.graphicsDeviceID + ", " + SystemInfo.graphicsDeviceVendorID + ", " + SystemInfo.graphicsDeviceVersion + ", " + SystemInfo.graphicsShaderLevel + ", " + SystemInfo.graphicsPixelFillrate + ", " + SystemInfo.deviceName + ", " + SystemInfo.deviceModel;
			Debug.Log(string.Concat("SystemInfo: ", text, "\nDefault Quality Level: ", QualitySettings.currentLevel, "\nShader.globalMaximumLOD: ", Shader.globalMaximumLOD, "\nQualitySettings.antiAliasing: ", QualitySettings.antiAliasing));
			if (IsBuildType("amazon"))
			{
				Debug.Log("AMAZON build");
			}
			else if (IsBuildType("google"))
			{
				Debug.Log("GOOGLE build");
			}
			else
			{
				Debug.Log("? build");
			}
		}
		Singleton<Analytics>.instance.LogEvent("CoinsOnStartup", Singleton<Profile>.instance.coins.ToString());
		Singleton<Analytics>.instance.LogEvent("GemsOnStartup", Singleton<Profile>.instance.gems.ToString());
		StartCoroutine(Init3rdPartyPlugins());
		PushNotification(false);
	}

	public void IAPSyncTransactions()
	{
	}

	private IEnumerator Init3rdPartyPlugins()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Initalizing ApplicationUtilities");
			}
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Initalizing ApplicationUtilities");
			}
			if (Debug.isDebugBuild)
			{
				Debug.Log("SERVER_NOTIFICATION_URL = " + SERVER_NOTIFICATION_URL);
			}
		}
		yield return null;
	}

	public void ShowAchievments()
	{
	}

	public void PromptGameCenterLogin()
	{
	}

	private void OnApplicationPause(bool state)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log("OnApplicationPause: " + state);
		}
		if (this.onApplicationPause != null)
		{
			this.onApplicationPause(state);
		}
		if (!state)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				Singleton<Analytics>.instance.LogEvent("CoinsOnResume", Singleton<Profile>.instance.coins.ToString());
				Singleton<Analytics>.instance.LogEvent("GemsOnResume", Singleton<Profile>.instance.gems.ToString());
				if (Debug.isDebugBuild)
				{
					Debug.Log("Game is being launched/resumed from the background, Sending game_launch from ApplicationUtilities::OnApplicationPause()");
				}
				mShowGameLauchPlayHavenAds = true;
				TapjoyInterface.OnResume();
				PushNotification(false);
				StartCoroutine(CheckVersion());
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				if (Singleton<Profile>.instance.tutorialIsComplete)
				{
					if (Debug.isDebugBuild)
					{
						Debug.Log("Game is being launched/resumed from the background, Sending game_launch from ApplicationUtilities::OnApplicationPause()");
					}
					mShowGameLauchPlayHavenAds = true;
					StartCoroutine(CheckVersion());
				}
				instance.IAPSyncTransactions();
				if (PurchaseCurrencyDialog.instance != null)
				{
					PurchaseCurrencyDialog.instance.startTime = Time.realtimeSinceStartup;
					PurchaseCurrencyDialog.instance.m_onResume = true;
				}
				Singleton<Analytics>.instance.StartSession();
				bIsApplicationPaused = state;
				StartCoroutine(CheckForTJPoints());
			}
			PushNotification(false);
		}
		else
		{
			Singleton<Analytics>.instance.LogEvent("CoinsOnSuspend", Singleton<Profile>.instance.coins.ToString());
			Singleton<Analytics>.instance.LogEvent("GemsOnSuspend", Singleton<Profile>.instance.gems.ToString());
			Singleton<Analytics>.instance.LogEvent("CurrentLevelOnSuspend", Application.loadedLevelName);
			PushNotification(true);
			Singleton<Analytics>.instance.EndSession();
			bIsApplicationPaused = state;
			if (Singleton<Profile>.IsAlive())
			{
				Singleton<Profile>.instance.Update();
			}
		}
	}

	private IEnumerator CheckVersion()
	{
		WWW www = (IsBuildType("amazon") ? ((!Debug.isDebugBuild) ? new WWW(Singleton<Localizer>.instance.Get("amazon_version_file_location_live")) : new WWW(Singleton<Localizer>.instance.Get("amazon_version_file_location_stage"))) : ((!Debug.isDebugBuild) ? new WWW(Singleton<Localizer>.instance.Get("version_file_location_live")) : new WWW(Singleton<Localizer>.instance.Get("version_file_location_stage"))));
		yield return www;
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			yield break;
		}
		Singleton<Profile>.instance.latestDetectedOnlineVersion = www.text;
		if (!string.IsNullOrEmpty(Singleton<Profile>.instance.latestDetectedOnlineVersion))
		{
			string[] version = Singleton<Profile>.instance.latestDetectedOnlineVersion.Split('.');
			if (version.Length >= 3 && (Singleton<GameVersion>.instance.major < int.Parse(version[0]) || (Singleton<GameVersion>.instance.major == int.Parse(version[0]) && Singleton<GameVersion>.instance.minor < int.Parse(version[1])) || (Singleton<GameVersion>.instance.major == int.Parse(version[0]) && Singleton<GameVersion>.instance.minor == int.Parse(version[1]) && Singleton<GameVersion>.instance.revision < int.Parse(version[2]))) && Application.loadedLevelName != "MainMenu")
			{
				Application.LoadLevel("MainMenu");
			}
		}
	}

	private void CheckForDailyReward()
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
		TimeSpan timeSpan = now.Subtract(value);
		if (timeSpan.Days == 1)
		{
			if (Application.loadedLevel != 0)
			{
				Application.LoadLevel("MainMenu");
			}
		}
		else if (timeSpan.Days > 1)
		{
			profile.currentDailyRewardNumber = 0;
			profile.Save();
			profile.lastYearPlayed = now.Year;
			profile.lastMonthPlayed = now.Month;
			profile.lastDayPlayed = now.Day;
		}
	}

	private void Update()
	{
		mTimeTillCheckDailyRewardsAgain -= Time.deltaTime;
		if (mTimeTillCheckDailyRewardsAgain <= 0f && WeakGlobalSceneBehavior<InGameImpl>.instance != null)
		{
			if (Application.loadedLevelName == "Store")
			{
				CheckForDailyReward();
			}
			StartCoroutine(CheckVersion());
			mTimeTillCheckDailyRewardsAgain = 10f;
		}
		Singleton<Profile>.instance.ConvertLocalGemsToGWallet();
		if (Singleton<Profile>.IsAlive())
		{
			Singleton<Profile>.instance.Update();
		}
	}

	private void FixedUpdate()
	{
		TapjoyInterface.Update();
	}

	private void OnDestroy()
	{
		DestroyPlugins();
		Singleton<Analytics>.instance.LogEvent("CoinsOnExit", PlayerPrefs.GetInt("asvz.coins").ToString());
		Singleton<Analytics>.instance.LogEvent("GemsOnExit", PlayerPrefs.GetInt("asvz.gems").ToString());
	}

	private void OnApplicationQuit()
	{
		Singleton<Analytics>.instance.LogEvent("CurrentLevelOnQuit", Application.loadedLevelName);
		if (this.onApplicationQuit != null)
		{
			this.onApplicationQuit();
		}
		DestroyPlugins();
		Singleton<Analytics>.instance.EndSession();
		UnityEngine.Object.Destroy(base.gameObject);
		PushNotification(true);
	}

	public void DestroyPlugins()
	{
	}

	public bool OnPlatform()
	{
		return Application.platform == RuntimePlatform.IPhonePlayer;
	}

	public bool OnInternet()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	public void ShowAds(AdPosition adPosition)
	{
	}

	public void ShowAds(int x, int y)
	{
	}

	public void HideAds()
	{
	}

	public void PrecacheAd()
	{
	}

	public bool IsAdOpen()
	{
		return false;
	}

	private void OnPlayerLoggedIn()
	{
	}

	private void OnPlayerFailedToLogin(string action)
	{
		GameCenterIsLoggedIn = false;
		Singleton<Profile>.instance.profileName = "Guest";
	}

	private void OnPlayerLoggedOut()
	{
	}

	private void PushNotification(bool state)
	{
	}

	private void SaveTimeStampAndPushNotification()
	{
		Singleton<Profile>.instance.timeStamp = Time.time;
		Singleton<Profile>.instance.Save();
		int time = 86400;
	}

	private void onBillingSupported(string supported)
	{
		Debug.Log("Unity: onBillingSupported: " + supported);
	}

	private void onSubscriptionSupported(string supported)
	{
		Debug.Log("Unity: onSubscriptionSupported: " + supported);
	}

	private void onGetUserIdResponse(string userID)
	{
		Debug.Log("Unity: onGetUserIdResponse: " + userID);
	}

	private void onPurchaseStateChange(string response)
	{
	}

	private void onRequestPurchaseResponse(string response)
	{
	}

	private void onRestoreTransactionsResponse(string response)
	{
		Debug.Log("onRestoreTransactionsResponse: " + response);
		if (response.Equals("RESULT_OK") || response.Equals("SUCCESSFUL"))
		{
			Debug.Log("TODO HANDLE RESTORE TRANSACTIONS SUCCESSFUL");
		}
		else
		{
			Debug.Log("TODO HANDLE RESTORE TRANSACTIONS FAILED");
		}
	}

	private void onTapjoyPointsReceived(string info)
	{
	}

	private void onCustomTap(string info)
	{
		string[] array = info.Split('|');
		string text = array[0];
		string text2 = array[1];
		float num = float.Parse(array[2]);
		float num2 = float.Parse(array[3]);
		Debug.Log("Unity: onCustomTap [" + text + "] " + text2 + " at (" + num + "," + num2 + ")");
	}

	private void onBrandBoostItemReady(string itemKey)
	{
		Debug.Log("Unity: onBrandBoostItemReady: " + itemKey);
	}

	private IEnumerator CheckForTJPoints()
	{
		yield break;
	}

	private void onPlayHavenShouldMakePurchase(string info)
	{
	}
}
