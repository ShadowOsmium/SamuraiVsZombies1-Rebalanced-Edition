using UnityEngine;

public class BoosterPackStoreImpl : SceneBehaviour
{
	private const float kPurchaseDialogPriority = 500f;

	private const float kWarningDialogPriority = 99f;

	private readonly Rect kPackListArea = new Rect(104f, 99f, 820f, 250f);

	private readonly Vector2 kPackListCellSize = new Vector2(170f, 246f);

	private readonly Rect kCardListArea = new Rect(148f, 535f, 718f, 195f);

	private readonly Vector2 kCardListCellSize = new Vector2(140f, 190f);

	private readonly Vector2 kCostPositionOffset = new Vector2(0f, 0f);

	private SUILayout mLayout;

	private TutorialManager mTutorials;

	private CostDisplay mCostDisplay;

	private int mSelectedPack = -1;

	private DialogHandler mDialogHandler;

	private BoosterPackListController mPackListController;

	private SUIScrollList mPackScrollList;

	private BoosterCardListController mCardListController;

	private SUIScrollList mCardScrollList;

	private int mLastDisplayedCoins = -1;

	private int mLastDisplayedGems = -1;

	private bool mBlockNextCardPreviews;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/BoosterPackStoreLayout");
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = OnQuit;
		//((SUITouchArea)mLayout["buyCurrencyTouch"]).onAreaTouched = OnRequestCurrencyPurchase;
		((SUIButton)mLayout["buyPackButton"]).onButtonPressed = OnPurchase;
		mPackListController = new BoosterPackListController(kPackListArea, kPackListCellSize);
		mPackScrollList = new SUIScrollList(mPackListController, kPackListArea, kPackListCellSize, SUIScrollList.ScrollDirection.Horizontal, 1);
		mPackScrollList.onSelectionChanged = OnPackSelected;
		if (Singleton<Profile>.instance.freeBoosterPacks > 0)
		{
			mPackScrollList.selection = 0;
		}
		mTutorials = new TutorialManager("BoosterPacksTutorial", string.Empty);
		if (!mTutorials.isDone)
		{
			mBlockNextCardPreviews = true;
		}
	}

	private void Update()
	{
		if (SceneBehaviourUpdate())
		{
			return;
		}
		RefreshCoins();
		if (mTutorials != null)
		{
			bool isBlocking = mTutorials.isBlocking;
			mTutorials.Update();
			if (mTutorials.isDone)
			{
				mTutorials.Destroy();
				mTutorials = null;
			}
			if (isBlocking)
			{
				return;
			}
		}
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
		mPackScrollList.Update();
		if (mCardScrollList != null)
		{
			mCardScrollList.Update();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			OnQuit();
		}
	}

	private void RefreshCoins()
	{
		int coins = Singleton<Profile>.instance.coins;
		int gems = Singleton<Profile>.instance.gems;
		if (mLastDisplayedCoins != coins || mLastDisplayedGems != gems)
		{
			mLastDisplayedCoins = coins;
			mLastDisplayedGems = gems;
			((SUILabel)mLayout["currencyCounter"]).text = coins + "\n" + gems;
		}
	}

	private void OnQuit()
	{
		mLayout.AnimateOut(WeakGlobalInstance<SUIScreen>.instance.fader.speed);
		string menuToGoTo = "Store";
		if (Singleton<FreebiesManager>.instance.sceneToReturnToFromBoosterStore != string.Empty)
		{
			menuToGoTo = Singleton<FreebiesManager>.instance.sceneToReturnToFromBoosterStore;
			Singleton<FreebiesManager>.instance.sceneToReturnToFromBoosterStore = string.Empty;
		}
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			Singleton<MenusFlow>.instance.LoadScene(menuToGoTo);
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnRequestCurrencyPurchase()
	{
		PurchaseCurrencyDialog purchaseCurrencyDialog = new PurchaseCurrencyDialog();
		purchaseCurrencyDialog.priority = 500f;
		mDialogHandler = new DialogHandler(499.5f, purchaseCurrencyDialog);
	}

	private void OnPackSelected(int index)
	{
		mSelectedPack = index;
		SDFTreeNode packData = Singleton<BoosterPackCodex>.instance.GetPackData(index);
		((SUILabel)mLayout["packTitle"]).text = Singleton<Localizer>.instance.Parse(packData["title"]);
		((SUILabel)mLayout["packDesc"]).text = Singleton<Localizer>.instance.Parse(packData["desc"]);
		DrawPurchaseButton(Singleton<BoosterPackCodex>.instance.GetPackCost(index));
		SetCardPreviews(index);
	}

	private void DrawPurchaseButton(Cost cost)
	{
		SUIButton sUIButton = (SUIButton)mLayout["buyPackButton"];
		if (mCostDisplay == null)
		{
			sUIButton.visible = true;
			mCostDisplay = new CostDisplay("default32");
			SUILayout.ObjectData objectData = new SUILayout.ObjectData();
			objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
			objectData.obj = mCostDisplay;
			mLayout.Add("costDisplay", objectData);
		}
		mCostDisplay.SetCost(cost, Singleton<Localizer>.instance.Get("store_free"));
		mCostDisplay.priority = sUIButton.priority + 1f;
		mCostDisplay.position = sUIButton.position + kCostPositionOffset + new Vector2((0f - mCostDisplay.width) / 2f, (0f - mCostDisplay.height) / 2f);
		mCostDisplay.visible = true;
	}

	private void SetCardPreviews(int packIndex)
	{
		if (mCardScrollList != null)
		{
			mCardScrollList.Destroy();
		}
		mCardListController = new BoosterCardListController(kCardListArea, kCardListCellSize, packIndex);
		mCardScrollList = new SUIScrollList(mCardListController, kCardListArea, kCardListCellSize, SUIScrollList.ScrollDirection.Horizontal, 1);
		mCardScrollList.onItemTouched = OnShowCardPreview;
	}

	private void OnShowCardPreview(int cardIndex)
	{
		if (mBlockNextCardPreviews)
		{
			mBlockNextCardPreviews = false;
			return;
		}
		BoosterCardListController.CardInfo card = mCardListController.GetCard(cardIndex);
		mDialogHandler = new DialogHandler(99f, new BoosterPackCardPreviewDialog(card.data, card.groupID, card.arg));
	}

	private void OnPurchase()
	{
		if (Singleton<BoosterPackCodex>.instance.GetPackCost(mSelectedPack).canAfford)
		{
			if (NeedDisplayPrePurchaseWarning())
			{
				DisplayPrePurchaseWarning();
			}
			else
			{
				DoPurchaseSelectedPack();
			}
		}
		else
		{
			OnRequestCurrencyPurchase();
		}
	}

	private bool NeedDisplayPrePurchaseWarning()
	{
		SDFTreeNode packData = Singleton<BoosterPackCodex>.instance.GetPackData(mSelectedPack);
		if (!packData.hasAttribute("warningCheck"))
		{
			return false;
		}
		string helperID = packData["warningCheck"];
		int helperLevel = Singleton<Profile>.instance.GetHelperLevel(helperID);
		int maxLevel = Singleton<HelpersDatabase>.instance.GetMaxLevel(helperID);
		return helperLevel >= maxLevel;
	}

	private void DisplayPrePurchaseWarning()
	{
		SDFTreeNode packData = Singleton<BoosterPackCodex>.instance.GetPackData(mSelectedPack);
		if (packData.hasAttribute("warningCheck"))
		{
			SDFTreeNode sDFTreeNode = SDFTree.LoadFromResources("Registry/Helpers/" + packData["warningCheck"]);
			string text = string.Format(Singleton<Localizer>.instance.Get("boosterstore_warning_helper"), Singleton<Localizer>.instance.Parse(sDFTreeNode["displayName"]));
			YesNoDialog yesNoDialog = new YesNoDialog(text, true, DoPurchaseSelectedPack, delegate
			{
			});
			yesNoDialog.priority = 100f;
			if (mDialogHandler != null)
			{
				mDialogHandler.Destroy();
			}
			mDialogHandler = new DialogHandler(99f, yesNoDialog);
		}
	}

	private void DoPurchaseSelectedPack()
	{
		Singleton<Analytics>.instance.LogEvent("BoosterPackPurchased", Singleton<BoosterPackCodex>.instance.GetPackID(mSelectedPack));
		Cost cost = Singleton<BoosterPackCodex>.instance.GetPackCost(mSelectedPack);
		if (mDialogHandler == null)
		{
			mDialogHandler = new DialogHandler(99f, new BoosterPackUnwrapDialog(mSelectedPack, cost));
		}
		else
		{
			mDialogHandler.PushCreator(() => new BoosterPackUnwrapDialog(mSelectedPack, cost));
		}
		if (mSelectedPack == 0 && Singleton<Profile>.instance.freeBoosterPacks > 0)
		{
			Singleton<Profile>.instance.freeBoosterPacks--;
			if (Singleton<Profile>.instance.freeBoosterPacks == 0)
			{
				mPackScrollList.ForceRedrawList();
				OnPackSelected(0);
			}
		}
	}
}
