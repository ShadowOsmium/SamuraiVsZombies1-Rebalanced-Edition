using UnityEngine;

public class StoreItemDisplayDialog : IDialog
{
	private const float kTransitionSpeed = 0.3f;

	private const float kFaderLevel = 0.7f;

	private readonly Vector2 kCostOffsetFromButton = new Vector2(-4f, 0f);

	private StoreData.Item mItemDesc;

	private OnSUIGenericCallback mOnItemPurchased;

	private SUILayout mLayout;

	private bool mDone;

	private SpoilsDisplay mSpoilsDisplay;

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

	public OnSUIGenericCallback onItemPurchased
	{
		get
		{
			return mOnItemPurchased;
		}
		set
		{
			mOnItemPurchased = value;
		}
	}

	public StoreItemDisplayDialog(StoreData.Item itemDesc)
	{
		mItemDesc = itemDesc;
		mLayout = new SUILayout("Layouts/ShopDetails");
		RenderItem();
		mLayout.AnimateIn(0.3f);
		SUIButton sUIButton = (SUIButton)mLayout["purchaseBtn"];
		sUIButton.onButtonPressed = OnPurchase;
		((SUIButton)mLayout["back"]).onButtonPressed = Close;
		if (itemDesc.locked)
		{
			sUIButton.visible = false;
		}
		else
		{
			SetupPurchaseButton(sUIButton, mItemDesc.cost);
		}
	}

	public void Destroy()
	{
		if (mSpoilsDisplay != null)
		{
			mSpoilsDisplay.Destroy();
			mSpoilsDisplay = null;
		}
		mLayout.Destroy();
		mLayout = null;
	}

	public void Update()
	{
		mLayout.Update();
		if (mSpoilsDisplay != null)
		{
			mSpoilsDisplay.Update();
		}
		if (!mLayout.isAnimating)
		{
			UpdateInputs();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Close();
		}
	}

	private void UpdateInputs()
	{
	}

	private void RenderItem()
	{
		if (mItemDesc.locked)
		{
			try
			{
				((SUISprite)mLayout["icon"]).texture = mItemDesc.icon + "_locked";
			}
			catch
			{
				((SUISprite)mLayout["icon"]).texture = mItemDesc.icon;
			}
			((SUILabel)mLayout["unlockCondition"]).text = mItemDesc.unlockCondition;
			((SUILabel)mLayout["unlockCondition"]).visible = true;
			if (mItemDesc.isFoundInPresent)
			{
				((SUILabel)mLayout["findInPresent"]).text = Singleton<Localizer>.instance.Get("store_findInPresent");
				((SUILabel)mLayout["findInPresent"]).visible = true;
				((SUISprite)mLayout["presentIcon0"]).visible = false;
				((SUISprite)mLayout["presentIcon1"]).visible = false;
				((SUISprite)mLayout["presentIcon2"]).visible = false;
				Vector2 vector = new Vector2(128f, 0f);
				Vector2 position = ((SUISprite)mLayout["presentIcon0"]).position;
				Vector2 position2 = new Vector2(position.x - (float)(mItemDesc.containedInPresent.Count - 1) * vector.x / 2f, position.y);
				for (int i = 0; i < mItemDesc.containedInPresent.Count; i++)
				{
					((SUISprite)mLayout["presentIcon" + i]).texture = mItemDesc.containedInPresent[i];
					((SUISprite)mLayout["presentIcon" + i]).position = position2;
					position2 += vector;
					((SUISprite)mLayout["presentIcon" + i]).visible = true;
				}
			}
		}
		else
		{
			((SUISprite)mLayout["icon"]).texture = mItemDesc.icon;
		}
		((SUISprite)mLayout["icon"]).visible = true;
		((SUILabel)mLayout["title"]).text = ((!mItemDesc.locked) ? mItemDesc.title : mItemDesc.unlockTitle);
		((SUILabel)mLayout["title"]).visible = true;
		mItemDesc.details.Render(mLayout, mItemDesc.locked);
		if (mItemDesc.contentList != null)
		{
			mSpoilsDisplay = new SpoilsDisplay(mLayout, mItemDesc.contentList, "default18", new Vector2(SUIScreen.width / 2f, SUIScreen.height / 2f - 20f), 2000f, 5, 144f, string.Empty);
		}
	}

	private void OnPurchase()
	{
		if (!mItemDesc.cost.canAfford)
		{
            WeakGlobalInstance<DialogHandler>.instance.PushCreator(() => new StoreNotEnoughDialog(mItemDesc)
            {
                priority = 9999
            });
			//ShowPurchaseCurrency(mItemDesc.cost);
			Close();
			return;
		}
		mItemDesc.cost.Spend(mItemDesc.id);
		mItemDesc.Apply();
		if (mOnItemPurchased != null)
		{
			mOnItemPurchased();
		}
		Close();
	}

	private void ShowPurchaseCurrency(Cost trigger)
	{
		float dialogPriority = priority;
		WeakGlobalInstance<DialogHandler>.instance.PushCreator(() => new StoreItemDisplayDialog(mItemDesc)
		{
			priority = dialogPriority,
			onItemPurchased = onItemPurchased
		});
		WeakGlobalInstance<DialogHandler>.instance.PushCreator(() => new PurchaseCurrencyDialog(trigger)
		{
			priority = dialogPriority
		});
		Close();
	}

	private void Close()
	{
		((SUIButton)mLayout["purchaseBtn"]).onButtonPressed = null;
		((SUIButton)mLayout["back"]).onButtonPressed = null;
		mDone = true;
		mLayout.AnimateOut(0.3f);
		if (WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
	}

	private void SetupPurchaseButton(SUIButton btn, Cost cost)
	{
		CostDisplay costDisplay = new CostDisplay("default32");
		costDisplay.SetCost(cost);
		costDisplay.position = btn.position + kCostOffsetFromButton - new Vector2(costDisplay.width / 2f, costDisplay.height / 2f);
		costDisplay.priority = btn.priority + 0.1f;
		costDisplay.alpha = 0f;
		SUILayout.ObjectData objectData = new SUILayout.ObjectData();
		objectData.obj = costDisplay;
		objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
		mLayout.Add("button_currency", objectData);
	}
}
