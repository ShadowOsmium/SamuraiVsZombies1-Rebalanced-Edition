using System.Collections.Generic;
using UnityEngine;

public class StoreImpl : SceneBehaviour
{
	private class TabData
	{
		private readonly Vector2 kNoveltyOffset = new Vector2(-84f, 50f);

		public StoreAvailability.Group storeGroup = StoreAvailability.Group.All;

		private SUISprite tabSprite;

		private SUITouchArea tabTouch;

		private string mSpriteFile;

		private NoveltyWidget noveltyWidget;

		private bool mSelected;

		private List<StoreData.Item> mItems;

		public bool selected
		{
			get
			{
				return mSelected;
			}
			set
			{
				mSelected = value;
				if (mSelected)
				{
					tabSprite.texture = mSpriteFile + "_selected";
				}
				else
				{
					tabSprite.texture = mSpriteFile;
				}
			}
		}

		public float priority
		{
			get
			{
				return tabSprite.priority;
			}
			set
			{
				tabSprite.priority = value;
			}
		}

		public Rect spriteArea
		{
			get
			{
				return tabSprite.area;
			}
		}

		public Rect touchArea
		{
			get
			{
				return tabTouch.area;
			}
			set
			{
				tabTouch.area = value;
			}
		}

		public OnSUIGenericCallback onTabTouched
		{
			get
			{
				return tabTouch.onAreaTouched;
			}
			set
			{
				tabTouch.onAreaTouched = value;
			}
		}

		public List<StoreData.Item> items
		{
			get
			{
				return mItems;
			}
		}

		public TabData(SUILayout layout, int index)
		{
			tabSprite = (SUISprite)layout["tab" + index];
			tabTouch = (SUITouchArea)layout["tabTouch" + index];
			mSpriteFile = tabSprite.texture;
			noveltyWidget = new NoveltyWidget();
			noveltyWidget.position = tabSprite.position + kNoveltyOffset;
			noveltyWidget.priority = tabSprite.priority + 2f;
			noveltyWidget.visible = false;
			selected = false;
		}

		public void Update()
		{
			if (noveltyWidget.visible)
			{
				NoveltyWidget obj = noveltyWidget;
				Vector2 position = tabSprite.position;
				Vector2 vector = kNoveltyOffset;
				float x = vector.x;
				Vector2 vector2 = kNoveltyOffset;
				obj.position = position + new Vector2(x, vector2.y);
				noveltyWidget.Update();
			}
		}

		public void PlaySelectSound()
		{
			Singleton<SUISoundManager>.instance.Play("selectSlot", tabSprite.gameObject);
		}

		public void ReloadItems()
		{
			mItems = StoreAvailability.GetList(storeGroup);
			noveltyWidget.visible = false;
			bool flag = false;
			string text = string.Empty;
			foreach (StoreData.Item mItem in mItems)
			{
				if (mItem.isNovelty || mItem.isEvent)
				{
					flag = true;
				}
				if (mItem.cost.isOnSale)
				{
					text = "Sprites/Localized/on_sale";
					break;
				}
			}
			if (text != string.Empty)
			{
				noveltyWidget.visible = true;
				noveltyWidget.texture = text;
			}
			else if (flag)
			{
				noveltyWidget.visible = true;
				noveltyWidget.texture = "Sprites/Menus/new_notification";
			}
		}
	}

	private const float kPurchaseAlphaWhenDisabled = 0.3f;

	private const int kNumListColumns = 4;

	private const float kPurchaseDialogPriority = 500f;

	private const int kStoreVisitDealPackTutorialTrigger = 4;

	private const float kPachinkoButtonPulseMin = 0.8f;

	private const float kPachinkoButtonPulseMax = 0.9f;

	private const float kPachinkoButtonPulseSpeed = 0.5f;

	private Rect kPurchaseListArea = new Rect(110f, 130f, 880f, 508f);

	private Vector2 kCellSize = new Vector2(220f, 270f);

	private bool finishedStart;

	private SUILayout mLayout;

	private SUIButton mPlayHaven;

	private YesNoDialog mInternetRequired;

	private YesNoDialog mTapJoyReportedGems;

	private List<TabData> mTabs = new List<TabData>();

	private int mSelectedTab;

	private DialogHandler mDialogHandler;

	private SUIScrollList mPurchaseList;

	private StoreListController mListController;

	private TutorialManager mTutorial;

	private int mLastDisplayedCoins = -1;

	private int mLastDisplayedGems = -1;

	private int mLastBallsNumDisplay = -1;

	private int mLastReviveNumDisplay = -1;

	private bool mPlayingSpecialDealPackTutorial;

	private SUILabel m_pachinkoBalls;

	private int m_pachinkoBallsCnt;

	private bool m_showTapjoyAds;

	public int kTapjoyFeatureAdsInterval
	{
		get
		{
			if (Debug.isDebugBuild)
			{
				return 3;
			}
			return 10;
		}
	}

	private void Start()
	{
		SingletonMonoBehaviour<ResourcesManager>.instance.CheckOnlineUpdates();
		Singleton<Profile>.instance.ClearBonusWaveData();
		WeakGlobalInstance<SUIScreen>.instance.fader.speed = 1f;
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		string text = Singleton<PlayModesManager>.instance.selectedModeData["layout_ShopLayout"];
		mLayout = new SUILayout(text + "_Android");
		if (ApplicationUtilities.IsGWalletAvailable())
		{
			((SUIButton)mLayout["ggnButton"]).onButtonPressed = delegate
			{
				LaunchGGN();
			};
		}
		else
		{
			((SUIButton)mLayout["ggnButton"]).visible = false;
		}
		((SUISprite)mLayout["ggnBugBg"]).visible = false;
		((SUILabel)mLayout["ggnBug"]).visible = false;
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = delegate
		{
			GoToMenu("MenuWith3DLevel");
		};
		((SUIButton)mLayout["back"]).onButtonPressed = delegate
		{
			GoToMenu("MainMenu");
		};
		((SUIButton)mLayout["pachinkoButton"]).onButtonPressed = OnGamblingPressed;
		//((SUITouchArea)mLayout["buyCurrencyTouch"]).onAreaTouched = OnRequestCurrencyPurchase;
		((SUITouchArea)mLayout["reviveTouch"]).onAreaTouched = OnPurchaseReviveShortcut;
		if (Singleton<Profile>.instance.pachinkoBalls > 0)
		{
			SUILayout.ObjectData objectData = mLayout.objects["pachinkobutton"];
			SUILayoutEffect.ScalePingPong e = new SUILayoutEffect.ScalePingPong(objectData.obj as SUIButton, 0.8f, 0.9f, 0.5f);
			objectData.AddEffect(e);
		}
		mPlayHaven = (SUIButton)mLayout["playHaven"];
		//mPlayHaven.onButtonPressed = OnOtherGames;
		int num = 0;
		while (mLayout.Exists("tab" + num))
		{
			int index = num;
			TabData tabData = new TabData(mLayout, num);
			tabData.storeGroup = (StoreAvailability.Group)num;
			tabData.ReloadItems();
			if (num == mSelectedTab)
			{
				tabData.selected = true;
			}
			tabData.onTabTouched = delegate
			{
				if (WeakGlobalInstance<TutorialHookup>.instance != null)
				{
					WeakGlobalInstance<TutorialHookup>.instance.storeTabTouched[index] = true;
				}
				OnTabTouched(index);
			};
			mTabs.Add(tabData);
			num++;
		}
		UpdateTabsLayout();
		mListController = new StoreListController(kPurchaseListArea, kCellSize, mTabs[mSelectedTab].items);
		mPurchaseList = new SUIScrollList(mListController, kPurchaseListArea, kCellSize, SUIScrollList.ScrollDirection.Vertical, 4);
		mPurchaseList.TransitInFromBelow();
		mPurchaseList.onItemTouched = OnItemTouched;
		Singleton<Profile>.instance.storeVisitCount++;
		mPlayingSpecialDealPackTutorial = Singleton<Profile>.instance.storeVisitCount == 4 && !Singleton<Profile>.instance.IsTutorialDone("StoreDealPacks", "dealpacks");
		if (mPlayingSpecialDealPackTutorial)
		{
			mTutorial = new TutorialManager("StoreDealPacks", string.Empty);
			WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
		}
		else
		{
			mTutorial = new TutorialManager("Store", string.Empty);
		}
		finishedStart = true;
		Singleton<PlayHavenTowerControl>.instance.InvokeContent("store_launch");
		if (Application.internetReachability != 0)
		{
		}
		CheckForCompletionistAchievement();
		if (mPlayingSpecialDealPackTutorial)
		{
			OnTabTouched(mTabs.Count - 1);
		}
		m_showTapjoyAds = Singleton<Profile>.instance.tapFeatureAdsCnt >= kTapjoyFeatureAdsInterval;
	}

	private void Update()
	{
		if (!finishedStart || SceneBehaviourUpdate())
		{
			return;
		}
		if (mTutorial != null)
		{
			if (mPlayingSpecialDealPackTutorial && mTutorial.isShowingPopup)
			{
				WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = true;
			}
			bool isBlocking = mTutorial.isBlocking;
			mTutorial.Update();
			if (mTutorial.isDone)
			{
				mTutorial.Destroy();
				mTutorial = null;
			}
			if (isBlocking)
			{
				return;
			}
		}
		RefreshCoins();
		RefreshReviveCounter();
		RefreshBallsCounter();
		if (mDialogHandler != null)
		{
			mDialogHandler.Update();
			if (mDialogHandler.isDone)
			{
				mDialogHandler.Destroy();
				mDialogHandler = null;
			}
			return;
		}
		mLayout.Update();
		mPurchaseList.Update();
		foreach (TabData mTab in mTabs)
		{
			mTab.Update();
		}
		if (m_showTapjoyAds)
		{
			Singleton<Profile>.instance.tapFeatureAdsCnt = 0;
			m_showTapjoyAds = false;
		}
		UpdatePachinkoBalls();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			GoToMenu("MainMenu");
		}
		if (ApplicationUtilities.IsGWalletAvailable())
		{
			int num = ApplicationUtilities.instance.GGN_BUG();
			((SUILabel)mLayout["ggnBug"]).text = num.ToString();
			if (num == 0)
			{
				((SUISprite)mLayout["ggnBugBg"]).visible = false;
				((SUILabel)mLayout["ggnBug"]).visible = false;
			}
			else
			{
				((SUISprite)mLayout["ggnBugBg"]).visible = true;
				((SUILabel)mLayout["ggnBug"]).visible = true;
			}
		}
	}

	private void RefreshCoins()
	{
		int coins = Singleton<Profile>.instance.coins;
		int gems = Singleton<Profile>.instance.gems;
		if (mLastDisplayedCoins != coins || mLastDisplayedGems != gems)
		{
			if (PurchaseCurrencyDialog.instance != null)
			{
				PurchaseCurrencyDialog.instance.RedrawButtons();
			}
			mLastDisplayedCoins = coins;
			mLastDisplayedGems = gems;
			((SUILabel)mLayout["currencyCounter"]).text = coins + "\n" + gems;
		}
	}

	private void OnItemTouched(int itemIndex)
	{
		if (mListController.items[itemIndex].id == "goto_boosters")
		{
			GoToMenu("BoosterPackStore");
		}
		else if (mListController.items[itemIndex].id == "vip")
		{
		}
		else
		{
			ShowPurchasePanel(mListController.items[itemIndex]);
		}
	}

	private void ShowPurchasePanel(StoreData.Item itemDesc)
	{
		StoreItemDisplayDialog storeItemDisplayDialog = new StoreItemDisplayDialog(itemDesc);
		storeItemDisplayDialog.priority = 500f;
		storeItemDisplayDialog.onItemPurchased = delegate
		{
			Singleton<Analytics>.instance.LogEvent("InStorePurchase", itemDesc.id);
			mTabs[mSelectedTab].ReloadItems();
			mListController.ReloadDataSource(mTabs[mSelectedTab].items);
			mPurchaseList.ForceRedrawList();
		};
		mDialogHandler = new DialogHandler(499.5f, storeItemDisplayDialog);
		if (itemDesc.isNovelty || itemDesc.isEvent)
		{
			List<string> novelties = Singleton<Profile>.instance.novelties;
			if (novelties.Contains(itemDesc.id))
			{
				novelties.Remove(itemDesc.id);
				Singleton<Profile>.instance.novelties = novelties;
			}
			RedrawCurrentList();
		}
	}

	private void GoToMenu(string sceneName)
	{
		mLayout.AnimateOut(WeakGlobalInstance<SUIScreen>.instance.fader.speed);
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			Singleton<MenusFlow>.instance.LoadScene(sceneName);
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void RedrawCurrentList()
	{
		mTabs[mSelectedTab].ReloadItems();
		mListController.ReloadDataSource(mTabs[mSelectedTab].items);
		mPurchaseList.ForceRedrawList();
	}

	private void OnTabTouched(int index)
	{
		if (index != mSelectedTab)
		{
			mTabs[mSelectedTab].selected = false;
			mSelectedTab = index;
			mTabs[mSelectedTab].selected = true;
			mTabs[index].PlaySelectSound();
			mPurchaseList.scrollPosition = new Vector2(0f, 0f);
			RedrawCurrentList();
			UpdateTabsLayout();
		}
	}

	private void OnOtherGames()
	{
		if (Application.internetReachability != 0)
		{
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("more_games");
			return;
		}
		mInternetRequired = new YesNoDialog(Singleton<Localizer>.instance.Get("No_Internet_Notification_Message_Text"), false, delegate
		{
		}, null);
		mInternetRequired.priority = 500f;
		StartDialog(mInternetRequired);
	}

	private void OnRequestCurrencyPurchase()
	{
		PurchaseCurrencyDialog purchaseCurrencyDialog = new PurchaseCurrencyDialog();
		purchaseCurrencyDialog.priority = 500f;
		mDialogHandler = new DialogHandler(499.5f, purchaseCurrencyDialog);
	}

	private void OnPurchaseReviveShortcut()
	{
		List<StoreData.Item> list = new List<StoreData.Item>();
		StoreAvailability.GetPotions(list);
		foreach (StoreData.Item item in list)
		{
			if (item.id == Singleton<PlayModesManager>.instance.revivePotionID)
			{
				ShowPurchasePanel(item);
				break;
			}
		}
	}

	private void OnCardCollectionPressed()
	{
		Debug.Log("OnCardCollectionPressed()");
	}

	private void OnGamblingPressed()
	{
		GoToMenu("Pachinko");
	}

	private void OnDailyRewardsPressed()
	{
		Debug.Log("OnDailyRewardsPressed()");
	}

	private void UpdateTabsLayout()
	{
		float num = 50.1f;
		foreach (TabData mTab in mTabs)
		{
			if (mTab.selected)
			{
				mTab.priority = 50.2f;
				continue;
			}
			mTab.priority = num;
			num -= 0.1f;
		}
	}

	private void RefreshReviveCounter()
	{
		int numPotions = Singleton<Profile>.instance.GetNumPotions(Singleton<PlayModesManager>.instance.revivePotionID);
		if (numPotions != mLastReviveNumDisplay)
		{
			mLastReviveNumDisplay = numPotions;
			string empty = string.Empty;
			switch (numPotions)
			{
			case 0:
				empty = Singleton<Localizer>.instance.Get("revive_none");
				break;
			case 1:
				empty = Singleton<Localizer>.instance.Get("revive_singular");
				break;
			default:
				empty = string.Format(Singleton<Localizer>.instance.Get("revive_plural"), numPotions);
				break;
			}
			((SUILabel)mLayout["reviveCounter"]).text = SUILayoutConv.GetFormattedText(empty);
		}
	}

	private void RefreshBallsCounter()
	{
		int pachinkoBalls = Singleton<Profile>.instance.pachinkoBalls;
		if (pachinkoBalls != mLastBallsNumDisplay)
		{
			string empty = string.Empty;
			empty = ((pachinkoBalls != 1) ? string.Format(Singleton<Localizer>.instance.Get("pachinkocounter_plurial"), pachinkoBalls.ToString()) : string.Format(Singleton<Localizer>.instance.Get("pachinkocounter_sing"), pachinkoBalls.ToString()));
			((SUILabel)mLayout["pachinkoCounter"]).text = SUILayoutConv.GetFormattedText(empty);
		}
	}

	private void CheckForCompletionistAchievement()
	{
		if (Singleton<PlayModesManager>.instance.selectedMode == "classic")
		{
			Profile instance = Singleton<Profile>.instance;
			if (instance.GetAbilityLevel("KatanaSlash") >= 10 && instance.GetAbilityLevel("Lethargy") >= 10 && instance.GetAbilityLevel("SummonLightning") >= 10 && instance.GetAbilityLevel("SummonTornadoes") >= 10 && instance.GetAbilityLevel("DivineIntervention") >= 10 && instance.GetAbilityLevel("GiantWave") >= 4 && instance.heroLevel >= 10 && instance.swordLevel >= 10 && instance.bowLevel >= 10 && instance.initialSmithyLevel >= 3 && instance.baseLevel >= 4 && instance.archerLevel >= 8 && instance.bellLevel >= 5 && instance.GetHelperLevel("Farmer") >= 10 && instance.GetHelperLevel("HeavySamurai") >= 10 && instance.GetHelperLevel("IceArcher") >= 10 && instance.GetHelperLevel("Nobunaga") >= 10 && instance.GetHelperLevel("Priest") >= 10 && instance.GetHelperLevel("Samurai") >= 10 && instance.GetHelperLevel("SamuraiArcher") >= 10 && instance.GetHelperLevel("SpearHorseman") >= 10 && instance.GetHelperLevel("SpearSamurai") >= 10 && instance.GetHelperLevel("Takeda") >= 10)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_COMPLETIONIST");
			}
		}
	}

	private void StartDialog(IDialog d)
	{
		if (mDialogHandler != null)
		{
			mDialogHandler.Destroy();
			mDialogHandler = null;
		}
		mDialogHandler = new DialogHandler(499f, d);
	}

	private void UpdatePachinkoBalls()
	{
		if (Singleton<Profile>.instance.pachinkoBalls > 0)
		{
			SUILayout.ObjectData objectData = mLayout.objects["pachinkobutton"];
			if (objectData.effects == null || objectData.effects.Count == 0)
			{
				SUILayoutEffect.ScalePingPong e = new SUILayoutEffect.ScalePingPong(objectData.obj as SUIButton, 0.8f, 0.9f, 0.5f);
				objectData.AddEffect(e);
			}
		}
	}

	private void LaunchGGN()
	{
	}
}
