using System.Collections.Generic;
using UnityEngine;

public class SelectHelpersImpl : MenuWith3DLevel
{
	private const int kNumListColumns = 3;

	private Rect kListArea = new Rect(34f, 116f, 492f, 340f);

	private Vector2 kCellSize = new Vector2(164f, 196f);

	private SUILayout mLayout;

	private SUIScrollList mList;

	private SelectHelpersController mListController;

	private SlotGroup mSlotGroup;

	private int initTimer = 3;

	public SelectHelpersImpl()
	{
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = true;
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_SelectHelpersMenu"]);
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = OnNextMenu;
		((SUIButton)mLayout["back"]).onButtonPressed = OnPreviousMenu;
		mSlotGroup = new SlotGroup(mLayout);
		mSlotGroup.onSelectionChanged = OnSlotSelectionChanged;
		mListController = new SelectHelpersController(kListArea, kCellSize);
		mList = new SUIScrollList(mListController, kListArea, kCellSize, SUIScrollList.ScrollDirection.Vertical, 3);
		mList.TransitInFromBelow();
		mList.onSelectionChanged = OnListSelectionChanged;
		List<string> selectedHelpers = Singleton<Profile>.instance.GetSelectedHelpers();
		for (int i = 0; i < mSlotGroup.numSlots; i++)
		{
			if (i >= selectedHelpers.Count)
			{
				continue;
			}
			int num = mListController.FindIndex(selectedHelpers[i]);
			if (num >= 0)
			{
				mSlotGroup.SetSlot(i, selectedHelpers[i], mListController.GetIcon(num));
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
				OnPreviousMenu();
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
				Singleton<Analytics>.instance.LogEvent("DeployedHelper", slotValue, Singleton<Profile>.instance.waveToBeat);
				list.Add(slotValue);
			}
		}
		Singleton<Profile>.instance.SetSelectedHelpers(list);
	}

	private void OnNextMenu()
	{
		SaveChanges();
		mLayout.onTransitionOver = delegate
		{
			WeakGlobalInstance<MenuWith3DLevelManager>.instance.LoadMenu("AbilitiesEquipImpl");
		};
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnPreviousMenu()
	{
		SaveChanges();
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.GoToMenu("Store");
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
