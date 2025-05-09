using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreNotEnoughDialog : IDialog 
{
    private const float kTransitionSpeed = 0.5f;

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

    public StoreNotEnoughDialog(StoreData.Item itemDesc)
    {
        mItemDesc = itemDesc;
        mLayout = new SUILayout("Layouts/NotEnoughCurrencyDialog");
        RenderItem();
        mLayout.AnimateIn(0.3f);
        SUIButton sUIButton = (SUIButton)mLayout["back"];
        sUIButton.text = "OK";
        sUIButton.onButtonPressed = OnClick;

        ((SUILabel)mLayout["title"]).text = "Not enough currency.";
        if (itemDesc.locked)
        {
            sUIButton.visible = false;
        }
        else
        {
            //SetupPurchaseButton(sUIButton, mItemDesc.cost);
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
        mItemDesc.details.Render(mLayout, mItemDesc.locked, true);
    }

    private void OnClick()
    {
        Close();
    }

    private void Close()
    {
        ((SUIButton)mLayout["back"]).onButtonPressed = null;
        mDone = true;
        mLayout.AnimateOut(0.3f);
        if (WeakGlobalInstance<DialogHandler>.instance != null)
        {
            WeakGlobalInstance<DialogHandler>.instance.FadeOut();
        }
    }
}
