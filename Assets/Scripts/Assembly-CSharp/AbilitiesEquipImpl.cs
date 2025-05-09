using System.Collections.Generic;
using UnityEngine;

public class AbilitiesEquipImpl : MenuWith3DLevel
{
	private const int kNumListColumns = 3;

	private Rect kListArea = new Rect(34f, 116f, 492f, 340f);

	private Vector2 kCellSize = new Vector2(164f, 196f);

	private SUILayout mLayout;

	private SUIScrollList mList;

	private AbilityEquipController mListController;

	private SlotGroup mSlotGroup;

	private int initTimer = 3;

	public AbilitiesEquipImpl()
	{
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = true;
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_AbilitiesEquip"]);
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = delegate
		{
			OnChangeMenuButton("CharmEquipImpl");
		};
		((SUIButton)mLayout["back"]).onButtonPressed = delegate
		{
			OnChangeMenuButton("SelectHelpersImpl");
		};
		mSlotGroup = new SlotGroup(mLayout);
		mSlotGroup.onSelectionChanged = OnSlotSelectionChanged;
		mListController = new AbilityEquipController(kListArea, kCellSize);
		mList = new SUIScrollList(mListController, kListArea, kCellSize, SUIScrollList.ScrollDirection.Vertical, 3);
		mList.TransitInFromBelow();
		mList.onSelectionChanged = OnListSelectionChanged;
		List<string> selectedAbilities = Singleton<Profile>.instance.GetSelectedAbilities();
		for (int i = 0; i < mSlotGroup.numSlots; i++)
		{
			if (i >= selectedAbilities.Count)
			{
				continue;
			}
			int num = mListController.FindIndex(selectedAbilities[i]);
			if (num >= 0)
			{
				mSlotGroup.SetSlot(i, selectedAbilities[i], mListController.GetIcon(num));
				if (i == 0)
				{
					mList.selection = mListController.FindIndex(mSlotGroup.GetSlotValue(i));
				}
			}
		}
		mList.Update();
		SUILayout.ObjectData od = new SUILayout.ObjectData
		{
			animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear),
			obj = mList
		};
		mLayout.Add("mainList", od);
		mList.alpha = 0f;
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
		mSlotGroup = null;
		mList.Destroy();
		mList = null;
		mListController = null;
	}

	public void Update()
	{
		if (initTimer > 0)
		{
			initTimer--;
			return;
		}
		mLayout.Update();
		if (mLayout != null)
		{
			mList.Update();
			mSlotGroup.Update();
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				OnChangeMenuButton("SelectHelpersImpl");
			}
		}
	}

	public void SaveChanges()
	{
		List<string> list = new List<string>(mSlotGroup.numSlots);
		for (int i = 0; i < mSlotGroup.numSlots; i++)
		{
			string slotValue = mSlotGroup.GetSlotValue(i);
			if (slotValue.Length > 0)
			{
				list.Add(slotValue);
				Singleton<Analytics>.instance.LogEvent("AbilityEquiped", slotValue, Singleton<Profile>.instance.waveToBeat);
			}
		}
		Singleton<Profile>.instance.SetSelectedAbilities(list);
	}

	private void OnChangeMenuButton(string newMenu)
	{
		SaveChanges();
		mLayout.onTransitionOver = delegate
		{
			WeakGlobalInstance<MenuWith3DLevelManager>.instance.LoadMenu(newMenu);
		};
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnListSelectionChanged(int index)
	{
		if (index != -1)
		{
			string val = mListController.FindID(index);
			mSlotGroup.SetSlot(mSlotGroup.selected, val, mListController.GetIcon(index));
			WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = mListController.GetDescription(index);
			WeakGlobalInstance<MenuWith3DLevelManager>.instance.descriptionIcon = mListController.GetIcon(index);
		}
	}

	private void OnSlotSelectionChanged(int index)
	{
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		mList.selection = mListController.FindIndex(mSlotGroup.GetSlotValue(index));
	}
}
