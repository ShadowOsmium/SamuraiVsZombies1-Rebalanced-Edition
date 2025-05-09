using UnityEngine;

public class SlotGroup
{
	private const float kSlotNormalScale = 0.75f;

	private const float kSlotSelectedScale = 1f;

	public OnSUIIntCallback onSelectionChanged;

	private int kNumSlots;

	private SUISprite[] mSlotRef;

	private string[] mSlotValues;

	private int mSelectedSlot;

	private float mSelectedSlotAnim;

	public int selected
	{
		get
		{
			return mSelectedSlot;
		}
	}

	public int numSlots
	{
		get
		{
			return kNumSlots;
		}
	}

	public SlotGroup(SUILayout layoutRef)
	{
		int num = 0;
		while (true)
		{
			string name = "slot" + num + "Touch";
			if (!layoutRef.Exists(name))
			{
				break;
			}
			int index = num;
			kNumSlots++;
			SUIProcess sUIProcess = layoutRef[name];
			((SUITouchArea)sUIProcess).onAreaTouched = delegate
			{
				OnTouchSlot(index);
			};
			num++;
		}
		mSlotValues = new string[kNumSlots];
		mSlotRef = new SUISprite[kNumSlots];
		for (int i = 0; i < kNumSlots; i++)
		{
			mSlotValues[i] = string.Empty;
			mSlotRef[i] = (SUISprite)layoutRef["slot" + i];
		}
	}

	public void Update()
	{
		UpdateAnimSelectSlot();
	}

	public void ResetSlot(int index)
	{
		SetSlot(index, string.Empty, Singleton<PlayModesManager>.instance.selectedModeData["equipSlot"]);
	}

	public void SetSlot(int index, string val, string spriteFile)
	{
		mSlotValues[index] = val;
		mSlotRef[index].texture = spriteFile;
		for (int i = 0; i < kNumSlots; i++)
		{
			if (i != index && string.Compare(mSlotValues[i], val, true) == 0)
			{
				mSlotValues[i] = string.Empty;
				mSlotRef[i].texture = Singleton<PlayModesManager>.instance.selectedModeData["equipSlot"];
			}
		}
	}

	public string GetSlotValue(int index)
	{
		return mSlotValues[index];
	}

	private void OnTouchSlot(int index)
	{
		if (mSelectedSlot != index)
		{
			mSelectedSlot = index;
			mSelectedSlotAnim = 0f;
			if (onSelectionChanged != null)
			{
				onSelectionChanged(index);
			}
			Singleton<SUISoundManager>.instance.Play("selectSlot", mSlotRef[mSelectedSlot].gameObject);
		}
	}

	private void UpdateAnimSelectSlot()
	{
		mSelectedSlotAnim = Mathf.Min(1f, mSelectedSlotAnim + Time.deltaTime * 2f);
		float num = Ease.BackInOut(mSelectedSlotAnim, 0.75f, 0.25f, 5f);
		mSlotRef[mSelectedSlot].scale = new Vector2(num, num);
		for (int i = 0; i < mSlotRef.Length; i++)
		{
			if (i != mSelectedSlot && mSlotRef[i].scale.x > 0.75f)
			{
				float num2 = Mathf.Max(0.75f, mSlotRef[i].scale.x - Time.deltaTime);
				mSlotRef[i].scale = new Vector2(num2, num2);
			}
		}
	}
}
