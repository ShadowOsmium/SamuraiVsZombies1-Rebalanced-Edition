using UnityEngine;

public class HUDIconButton
{
	private const float kCooldDownFullScale = 13.5f;

	private const float kCoolDownAlpha = 0.4f;

	private const float kPortraitCoolDownAlpha = 0.3f;

	private const float kIconAnimOutOffset = 140f;

	private Vector2 kCooldownOffset = new Vector2(0f, 0f);

	public string tag = string.Empty;

	private Vector2 mPosition;

	private string mIconFile;

	private string mGroupName;

	private bool mVisible = true;

	private bool mCooldownVisible = true;

	private int mIndex;

	private float mClientAlpha = 1f;

	private float mIconAlpha = 1f;

	private SUISprite mIcon;

	private SUISprite mCooldown;

	private SUITouchArea mTrigger;

	private IsIconAvailableCallback mIsAvailCallback;

	private GetCooldownCallback mGetCoolDownCallback;

	private OnSUIGenericCallback mOnTouchCallback;

	public bool visible
	{
		get
		{
			return mVisible;
		}
		set
		{
			mVisible = value;
			ApplyVisible();
		}
	}

	public float alpha
	{
		get
		{
			return mClientAlpha;
		}
		set
		{
			mClientAlpha = value;
			ApplyAlpha();
		}
	}

	public OnSUIGenericCallback onTriggered
	{
		get
		{
			return mOnTouchCallback;
		}
		set
		{
			mOnTouchCallback = value;
		}
	}

	public HUDIconButton(SUILayout layoutRef, Vector2 position, string iconFile, string groupName, int index, IsIconAvailableCallback isAvailCallback, GetCooldownCallback getCoolDownCallback)
	{
		mPosition = position;
		mGroupName = groupName;
		mIconFile = iconFile;
		mIndex = index;
		mIsAvailCallback = isAvailCallback;
		mGetCoolDownCallback = getCoolDownCallback;
		CreateHUDElements(layoutRef);
	}

	public void Destroy()
	{
		if (mIcon != null)
		{
			mIcon.Destroy();
			mIcon = null;
		}
		if (mCooldown != null)
		{
			mCooldown.Destroy();
			mCooldown = null;
		}
	}

	public void UpdateCoolDown()
	{
		if (mIsAvailCallback(mIndex))
		{
			mIconAlpha = 1f;
		}
		else
		{
			mIconAlpha = 0.3f;
		}
		float num = mGetCoolDownCallback(mIndex);
		if (num >= 1f)
		{
			mCooldownVisible = false;
		}
		else
		{
			mCooldownVisible = true;
			mCooldown.texture = GetCooldownTextureForRatio(num);
		}
		ApplyAlpha();
		ApplyVisible();
	}

	public bool IsTouchZone(Vector2 pos)
	{
		return mTrigger.area.Contains(pos);
	}

	private void CreateHUDElements(SUILayout layout)
	{
		mIcon = new SUISprite(mIconFile);
		mIcon.position = mPosition;
		mIcon.priority = 0.1f;
		mCooldown = new SUISprite(GetCooldownTextureForRatio(0f));
		mCooldown.priority = 0.5f;
		mCooldown.alpha = 0.4f;
		mCooldown.visible = false;
		mCooldown.position = mPosition + kCooldownOffset;
		mTrigger = new SUITouchArea(GetTriggerRect(mIcon.position));
		mTrigger.onAreaTouched = OnTouched;
		if (layout != null)
		{
			SUILayout.ObjectData objectData = new SUILayout.ObjectData();
			objectData.obj = mIcon;
			objectData.animPosition = new SUILayoutAnim.AnimVector2(mIcon.position, new Vector2(mIcon.position.x - 140f, mIcon.position.y + 140f), new SUILayout.NormalRange(0f, 1f), Ease.BackOut);
			objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
			SUILayout.ObjectData objectData2 = new SUILayout.ObjectData();
			objectData2.obj = mCooldown;
			objectData2.animPosition = new SUILayoutAnim.AnimVector2(mCooldown.position, new Vector2(mCooldown.position.x - 140f, mCooldown.position.y + 140f), new SUILayout.NormalRange(0f, 1f), Ease.BackOut);
			objectData2.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
			layout.Add(mGroupName + "Icon" + mIndex, objectData);
			layout.Add(mGroupName + "Cooldown" + mIndex, objectData2);
			layout.Add(mGroupName + "Trigger" + mIndex, mTrigger);
		}
	}

	private string GetCooldownTextureForRatio(float ratio)
	{
		int num = Mathf.CeilToInt(Mathf.Clamp(ratio * 33f, 0f, 32f));
		return string.Format("Sprites/HUD/CoolDown/cool_down_{0:000}", num);
	}

	public void Trigger()
	{
		if (mOnTouchCallback != null)
		{
			mOnTouchCallback();
		}
	}

	private void OnTouched()
	{
		if (visible && mOnTouchCallback != null)
		{
			mOnTouchCallback();
		}
	}

	private void ApplyAlpha()
	{
		mIcon.alpha = mIconAlpha * mClientAlpha;
		mCooldown.alpha = 0.4f * mClientAlpha;
	}

	private static Rect GetTriggerRect(Vector2 pos)
	{
		return new Rect(pos.x - 64f, pos.y - 64f, 128f, 128f);
	}

	private void ApplyVisible()
	{
		mIcon.visible = mVisible;
		mCooldown.visible = mVisible && mCooldownVisible;
	}
}
