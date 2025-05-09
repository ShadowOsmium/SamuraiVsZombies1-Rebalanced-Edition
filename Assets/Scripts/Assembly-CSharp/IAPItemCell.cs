using UnityEngine;

internal class IAPItemCell : SUIScrollList.Cell
{
	private OnlineItemsManager.Item mItemData;

	private SUIButton mButton;

	private SUISprite mIcon;

	private Vector2 iconOffsetNormal = new Vector2(70f, -4f);

	private Vector2 iconOffsetVIP = new Vector2(0f, 4f);

	private Vector2 mPosition;

	private PurchaseCurrencyDialog mDialogHandle;

	public override bool visible
	{
		set
		{
			if (mButton != null)
			{
				mButton.visible = value;
			}
			if (mIcon != null)
			{
				mIcon.visible = value;
			}
		}
	}

	public override Vector2 position
	{
		set
		{
			mPosition = value;
			UpdatePosition();
		}
	}

	public IAPItemCell(PurchaseCurrencyDialog DialogHandle)
	{
		mDialogHandle = DialogHandle;
		mPosition = new Vector2(0f, 0f);
	}

	public override void Destroy()
	{
	}

	public override void Update()
	{
	}

	public void DrawContent(OnlineItemsManager.Item itemData, int index)
	{
		Debug.Log("*** DrawContent||index= " + index);
		mItemData = itemData;
		mButton = (SUIButton)mDialogHandle.mLayout["btn" + index];
		mIcon = (SUISprite)mDialogHandle.mLayout["icon" + index];
		mButton.onButtonPressed = delegate
		{
			mDialogHandle.StartPurchase(itemData.id, itemData.type);
		};
		mButton.label.alignment = TextAlignment.Left;
		mButton.label.anchor = TextAnchor.MiddleLeft;
		if (itemData.type == OnlineItemsManager.ItemType.Vip_Gold || itemData.type == OnlineItemsManager.ItemType.Vip_Silver)
		{
			mButton.text = " ";
			if (itemData.type == OnlineItemsManager.ItemType.Vip_Gold)
			{
				mIcon.texture = "Sprites/Localized/svz_bank_VIPGoldbutton";
				mIcon.scale = new Vector2(0.95f, 0.95f);
			}
			else
			{
				mIcon.texture = "Sprites/Localized/svz_bank_VIPSilverbutton";
				mIcon.scale = new Vector2(0.95f, 0.95f);
			}
			return;
		}
		mButton.text = itemData.amount.ToString();
		mIcon.position = mPosition - iconOffsetNormal;
		if (itemData.type == OnlineItemsManager.ItemType.Coins)
		{
			mIcon.texture = "Sprites/Icons/win_coins";
			mIcon.scale = new Vector2(0.7f, 0.7f);
		}
		else if (itemData.type == OnlineItemsManager.ItemType.Gems)
		{
			mIcon.texture = "Sprites/Icons/win_gems";
			mIcon.scale = new Vector2(0.7f, 0.7f);
		}
	}

	private void UpdatePosition()
	{
		if (mButton != null && mIcon != null)
		{
			mButton.position = mPosition;
			if (mItemData.type == OnlineItemsManager.ItemType.Vip_Silver || mItemData.type == OnlineItemsManager.ItemType.Vip_Gold)
			{
				mIcon.position = mPosition + iconOffsetVIP;
			}
			else
			{
				mIcon.position = mPosition - iconOffsetNormal;
			}
		}
	}

	protected override void ApplyCombinedAlpha()
	{
		float num = mAlpha * mTransitionAlpha;
		mButton.alpha = num;
		mIcon.alpha = num;
	}
}
