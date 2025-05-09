using System.Collections.Generic;
using UnityEngine;

public class PurchaseCurrencyDialog : IDialog
{
	private class Tab
	{
		private SUISprite mTabSpriteRef;

		private SUISprite mTabGlowRef;

		private SUITouchArea mTouchAreaRef;

		private Vector2 mOriginalPosition;

		private Vector2 mSelectedPosition;

		public OnSUIGenericCallback onTouched
		{
			get
			{
				return mTouchAreaRef.onAreaTouched;
			}
			set
			{
				mTouchAreaRef.onAreaTouched = value;
			}
		}

		public bool selected
		{
			get
			{
				return mTabGlowRef.visible;
			}
			set
			{
				mTabGlowRef.visible = value;
				if (mTabGlowRef.visible)
				{
					mTabSpriteRef.position = mSelectedPosition;
				}
				else
				{
					mTabSpriteRef.position = mOriginalPosition;
				}
			}
		}

		public GameObject gameObjectForSound
		{
			get
			{
				return mTabSpriteRef.gameObject;
			}
		}

		public Tab(SUILayout layout, int index)
		{
			mTabSpriteRef = (SUISprite)layout["tab" + index];
			mTabGlowRef = (SUISprite)layout["tabGlow" + index];
			mTouchAreaRef = (SUITouchArea)layout["tabTouch" + index];
			mOriginalPosition = mTabSpriteRef.position;
			mSelectedPosition = mOriginalPosition + new Vector2(0f, -22f);
			mTabGlowRef.position = mSelectedPosition;
		}
	}

	private const float kTransitionSpeed = 0.3f;

	private const float kPurchaseAlphaWhenDisabled = 0.3f;

	private const int kNumListColumns = 2;

	private const int kExpectedFullDataNum = 14;

	private const int kNumTabs = 2;

	public static PurchaseCurrencyDialog instance;

	private SUIScrollList mPurchaseList;

	private IAPItemsListController mListController;

	private Rect kPurchaseListArea = new Rect(350f, 244f, 650f, 220f);

	private Vector2 kCellSize = new Vector2(320f, 105f);

	public float startTime;

	private static float TimeOutInSecs;

	public SUILayout mLayout;

	private float mPriority;

	private bool mDone;

	private List<Tab> mTabs = new List<Tab>(2);

	private int mSelectedTab;

	private bool mSomethingPurchased;

	private bool mTapJoyActivated;

	private float mMusicVolume = 1f;

	public bool m_onResume;

	private int kEntriesInPreviousTabs
	{
		get
		{
			if (mSelectedTab == 1)
			{
				return 6;
			}
			return 8;
		}
	}

	public int kEntriesPerTabs
	{
		get
		{
			if (mSelectedTab == 1)
			{
				return 8;
			}
			return 6;
		}
	}

	public bool isDone
	{
		get
		{
			return mDone && !mLayout.isAnimating;
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
			mLayout.basePriority = value;
		}
	}

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	private bool buttonsVisible
	{
		get
		{
			return !((SUISprite)mLayout["waitIcon"]).visible && !((SUILabel)mLayout["errorMessage"]).visible;
		}
	}

	public PurchaseCurrencyDialog()
	{
		Init(new Cost(0, 1, 0f));
	}

	public PurchaseCurrencyDialog(Cost trigger)
	{
		Init(trigger);
	}

	private void Init(Cost trigger)
	{
		mLayout = new SUILayout("Layouts/PurchaseCurrency_GWallet");
		mLayout.AnimateIn(0.3f);
		if (trigger.hard > 0)
		{
			mSelectedTab = 1;
		}
		CreatePriceWidgets();
		for (int i = 0; i < 2; i++)
		{
			int index = i;
			mTabs.Add(new Tab(mLayout, i));
			mTabs[i].onTouched = delegate
			{
				OnTabTouched(index);
			};
		}
		mTabs[mSelectedTab].selected = true;
		((SUIButton)mLayout["back"]).onButtonPressed = Close;
		((SUIButton)mLayout["free"]).onButtonPressed = OnFreeGemsButtonPressed;
		instance = this;
		if (Application.loadedLevelName == "Store" || Application.loadedLevelName == "Pachinko")
		{
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("bank_launch");
		}
		InitOnlineStore();
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
	}

	public void Update()
	{
		mLayout.Update();
		if (mTapJoyActivated && m_onResume)
		{
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("tj_closed");
			mTapJoyActivated = false;
			Camera.main.GetComponent<AudioSource>().volume = mMusicVolume;
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Close();
		}
		if (ApplicationUtilities.instance.bShouldTimeoutIAPs || ((SUISprite)mLayout["waitIcon"]).visible)
		{
			IAPTimeOutTick();
		}
		mPurchaseList.Update();
		m_onResume = false;
	}

	private void CreatePriceWidgets()
	{
	}

	private void OnTabTouched(int index)
	{
		if (buttonsVisible && mSelectedTab != index)
		{
			mSelectedTab = index;
			for (int i = 0; i < mTabs.Count; i++)
			{
				mTabs[i].selected = i == mSelectedTab;
			}
			RedrawButtons();
			Singleton<SUISoundManager>.instance.Play("selectSlot", mTabs[0].gameObjectForSound);
		}
	}

	private void Close()
	{
		for (int i = 0; i < kEntriesPerTabs; i++)
		{
			((SUIButton)mLayout["btn" + i]).onButtonPressed = null;
		}
		mDone = true;
		mLayout.AnimateOut(0.3f);
		if (WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
	}

	public void RedrawButtons()
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log("** RedrawButtons() START >>>");
		}
		if (mLayout == null)
		{
			return;
		}
		if (mListController != null && mPurchaseList != null)
		{
			List<OnlineItemsManager.Item> list = new List<OnlineItemsManager.Item>();
			for (int i = mSelectedTab * kEntriesInPreviousTabs; i < mSelectedTab * kEntriesInPreviousTabs + kEntriesPerTabs; i++)
			{
				if (ApplicationUtilities.IsGWalletAvailable())
				{
					if (Singleton<OnlineItemsManager>.instance.items[i].type == OnlineItemsManager.ItemType.Vip_Silver || Singleton<OnlineItemsManager>.instance.items[i].type == OnlineItemsManager.ItemType.Vip_Gold)
					{
						int idx = i - mSelectedTab * kEntriesInPreviousTabs;
						OnlineItemsManager.Item item = ApplicationUtilities.GWalletGetSubscriptionRecommendationAtIndex(idx);
						if (item != null)
						{
							list.Add(item);
						}
						continue;
					}
				}
				else if (Singleton<OnlineItemsManager>.instance.items[i].type == OnlineItemsManager.ItemType.Vip_Silver || Singleton<OnlineItemsManager>.instance.items[i].type == OnlineItemsManager.ItemType.Vip_Gold)
				{
					continue;
				}
				list.Add(Singleton<OnlineItemsManager>.instance.items[i]);
			}
			mListController.HideAllCells();
			mListController.ReloadDataSource(list.ToArray());
			mPurchaseList.ForceRedrawList();
		}
		SetAllButtonsVisible(true);
		((SUILabel)mLayout["title"]).text = Singleton<Localizer>.instance.Get("store_currencytab" + mSelectedTab);
		((SUILabel)mLayout["title"]).visible = true;
		if (Debug.isDebugBuild)
		{
			Debug.Log("** RedrawButtons() FINISH >>>");
		}
	}

	private void SetAllButtonsVisible(bool v)
	{
		if (!v)
		{
			mListController.HideAllCells();
		}
		((SUIButton)mLayout["free"]).visible = v;
		((SUISprite)mLayout["waitIcon"]).visible = !v;
		((SUILabel)mLayout["errorMessage"]).visible = !v;
	}

	public void ResetButtons()
	{
	}

	private void InitOnlineStore()
	{
	}

	private void OnIAPItemsInfoReceived(IAPItemDescriptor[] items)
	{
		Singleton<OnlineItemsManager>.instance.ApplyOnlineInformations(items);
		RedrawButtons();
	}

	public void OnIAPError(int code, string msg)
	{
		ApplicationUtilities.instance.bShouldTimeoutIAPs = false;
		if (ApplicationUtilities.IsBuildType("amazon"))
		{
			RedrawButtons();
			return;
		}
		RedrawButtons();
		string text = null;
		switch (msg)
		{
		case "RESULT_OK":
			return;
		case "SUCCESSFUL":
			SetAllButtonsVisible(true);
			return;
		case "REFUNDED":
			text = "Purchase has been refunded.";
			break;
		case "CANCELED":
		case "RESULT_USER_CANCELED":
			text = "Purchase Canceled.";
			break;
		case "IAP_TIMEOUT":
			ApplicationUtilities.instance.IAPSyncTransactions();
			text = "Purchase Timed-out.";
			break;
		default:
			text = "Purchase Failed.";
			break;
		}
		SetAllButtonsVisible(false);
		((SUISprite)mLayout["waitIcon"]).visible = false;
		((SUILabel)mLayout["errorMessage"]).visible = true;
		((SUILabel)mLayout["errorMessage"]).text = text;
	}

	private void OnIAPPurchaseCompleted(string eventInfo)
	{
		SetAllButtonsVisible(true);
	}

	private void OnFreeGemsButtonPressed()
	{
		if (Application.internetReachability != 0)
		{
			mMusicVolume = Camera.main.GetComponent<AudioSource>().volume;
			Camera.main.GetComponent<AudioSource>().volume = 0f;
			mTapJoyActivated = true;
		}
	}

	public void StartPurchase(string id, OnlineItemsManager.ItemType type)
	{
		mSomethingPurchased = true;
		Singleton<PlayHavenTowerControl>.instance.inIAPMode = true;
		if (MultiLanguages.isCheatIAP)
		{
			Singleton<OnlineItemsManager>.instance.CompletePurchase(id);
			return;
		}
		startTime = Time.realtimeSinceStartup;
		if (ApplicationUtilities.IsBuildType("google"))
		{
			TimeOutInSecs = 60f;
			ApplicationUtilities.instance.bShouldTimeoutIAPs = true;
		}
		else if (ApplicationUtilities.IsBuildType("amazon"))
		{
			TimeOutInSecs = 3f;
			ApplicationUtilities.instance.bShouldTimeoutIAPs = true;
		}
		SetAllButtonsVisible(false);
	}

	private void IAPTimeOutTick()
	{
		if (Time.realtimeSinceStartup - startTime > TimeOutInSecs)
		{
			instance.OnIAPError(0, "IAP_TIMEOUT");
			startTime = 0f;
		}
		Debug.Log("*** Time elapsed: " + (Time.realtimeSinceStartup - startTime) + " startTime:" + startTime + " Time.realtimeSinceStartup:" + Time.realtimeSinceStartup);
	}
}
