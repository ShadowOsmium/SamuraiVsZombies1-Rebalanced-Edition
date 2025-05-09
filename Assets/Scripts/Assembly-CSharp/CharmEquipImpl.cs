using UnityEngine;

public class CharmEquipImpl : MenuWith3DLevel
{
	private const int kNumListColumns = 3;

	private Rect kListArea = new Rect(34f, 116f, 492f, 340f);

	private Vector2 kCellSize = new Vector2(164f, 212f);

	private SUILayout mLayout;

	private SUIScrollList mList;

	private CharmEquipController mListController;

	private SlotGroup mSlotGroup;

	private SUIButton mResetSlotButton;

	private int initTimer = 3;

	public CharmEquipImpl()
	{
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = true;
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_CharmEquip"]);
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = OnNextMenu;
		((SUIButton)mLayout["back"]).onButtonPressed = OnPreviousMenu;
		mSlotGroup = new SlotGroup(mLayout);
		mSlotGroup.onSelectionChanged = OnSlotSelectionChanged;
		mResetSlotButton = (SUIButton)mLayout["resetSlots"];
		mResetSlotButton.onButtonPressed = OnResetSlots;
		mListController = new CharmEquipController(kListArea, kCellSize);
		mList = new SUIScrollList(mListController, kListArea, kCellSize, SUIScrollList.ScrollDirection.Vertical, 3);
		mList.TransitInFromBelow();
		mList.onSelectionChanged = OnListSelectionChanged;
		string selectedCharm = Singleton<Profile>.instance.selectedCharm;
		if (selectedCharm != string.Empty)
		{
			mSlotGroup.SetSlot(0, selectedCharm, mListController.GetIcon(mListController.FindIndex(selectedCharm)));
			mList.selection = mListController.FindIndex(selectedCharm);
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
			mResetSlotButton.visible = mSlotGroup.GetSlotValue(0) != string.Empty;
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				OnPreviousMenu();
			}
		}
	}

	public void SaveChanges()
	{
		Singleton<Profile>.instance.selectedCharm = mSlotGroup.GetSlotValue(0);
		Singleton<Analytics>.instance.LogEvent("CharmEquiped", Singleton<Profile>.instance.selectedCharm, Singleton<Profile>.instance.waveToBeat);
	}

	private void OnNextMenu()
	{
		SaveChanges();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			WaveManager.LoadNextWaveLevel();
		};
	}

	private void OnPreviousMenu()
	{
		SaveChanges();
		mLayout.onTransitionOver = delegate
		{
			WeakGlobalInstance<MenuWith3DLevelManager>.instance.LoadMenu("AbilitiesEquipImpl");
		};
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnResetSlots()
	{
		mSlotGroup.ResetSlot(0);
		mList.selection = -1;
	}

	private void OnListSelectionChanged(int index)
	{
		string text = mListController.FindID(index);
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = mListController.GetDescription(index);
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.descriptionIcon = mListController.GetIcon(index);
		if (Singleton<Profile>.instance.GetNumCharms(text) != 0)
		{
			mSlotGroup.SetSlot(mSlotGroup.selected, text, mListController.GetIcon(index));
		}
	}

	private void OnSlotSelectionChanged(int index)
	{
		WeakGlobalInstance<MenuWith3DLevelManager>.instance.description = string.Empty;
		mList.selection = mListController.FindIndex(mSlotGroup.GetSlotValue(index));
	}
}
