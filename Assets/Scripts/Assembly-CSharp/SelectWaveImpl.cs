using UnityEngine;

public class SelectWaveImpl : SceneBehaviour
{
	private class SlotRefs
	{
		public SUILabel title;

		public SUISprite slot;

		public SUILabel level;

		public SUITouchArea trigger;

		public bool visible
		{
			get
			{
				return title.visible;
			}
			set
			{
				title.visible = value;
				slot.visible = value;
				level.visible = value;
			}
		}

		public bool locked
		{
			get
			{
				return title.alpha < 1f;
			}
			set
			{
				title.alpha = ((!value) ? 1f : 0.3f);
				slot.alpha = ((!value) ? 1f : 0.3f);
				level.visible = !value;
			}
		}

		public SlotRefs(SUILayout layout, int index)
		{
			string text = string.Format("slot{0}_", index);
			title = (SUILabel)layout[text + "title"];
			slot = (SUISprite)layout[text + "sprite"];
			level = (SUILabel)layout[text + "level"];
			trigger = (SUITouchArea)layout[text + "trigger"];
		}

		public void moveBy(Vector2 offset)
		{
			title.position += offset;
			slot.position += offset;
			level.position += offset;
			trigger.area = new Rect(trigger.area.xMin + offset.x, trigger.area.yMin + offset.y, trigger.area.width, trigger.area.height);
		}
	}

	private const int kMaxSlots = 10;

	private const int kNumChapterListColumns = 1;

	private Rect kChapterListArea = new Rect(37f, 122f, 242f, 576f);

	private Vector2 kChapterCellSize = new Vector2(242f, 180f);

	private Color kNormalColor;

	private Color kSelectedColor;

	private SUILayout mLayout;

	private SUIScrollList mChapterList;

	private ChaptersListController mChapterListController;

	private SlotRefs[] mSlotRefs;

	private SUISprite mSlotSelector;

	private int[] waveRangeOnDisplay
	{
		get
		{
			return Singleton<ChaptersDatabase>.instance.GetWavesRange(Singleton<ChaptersDatabase>.instance.allChapterIDs[mChapterList.selection]);
		}
	}

	private static Vector2 GetSlotPosition(int index)
	{
		int num = index / 5;
		int num2 = index % 5;
		return new Vector2(num2 * 126 + 396, num * 200 + 324);
	}

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.speed = 1f;
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/SelectWave");
		mLayout.AnimateIn();
		kNormalColor = SUILayoutConv.GetColor(mLayout.data["normalColor"]);
		kSelectedColor = SUILayoutConv.GetColor(mLayout.data["selectedColor"]);
		mSlotRefs = new SlotRefs[10];
		for (int i = 0; i < 10; i++)
		{
			mSlotRefs[i] = new SlotRefs(mLayout, i);
			mSlotRefs[i].moveBy(GetSlotPosition(i));
			int index = i;
			mSlotRefs[i].trigger.onAreaTouched = delegate
			{
				OnSlotTouched(index);
			};
		}
		mSlotSelector = (SUISprite)mLayout["slot_selector"];
		((SUIButton)mLayout["continue"]).onButtonPressed = delegate
		{
			GoToMenu("MenuWith3DLevel");
		};
		mChapterListController = new ChaptersListController(kChapterListArea, kChapterCellSize);
		mChapterList = new SUIScrollList(mChapterListController, kChapterListArea, kChapterCellSize, SUIScrollList.ScrollDirection.Vertical, 1);
		mChapterList.scrollPosition = new Vector2(0f, 0f - kChapterListArea.height * 4f);
		mChapterList.onSelectionChanged = OnChapterSelectionChanged;
		mChapterList.Update();
		SUILayout.ObjectData objectData = new SUILayout.ObjectData();
		objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
		objectData.obj = mChapterList;
		mLayout.Add("chapterList", objectData);
		mChapterList.alpha = 0f;
		SetInitialSelection();
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
		mChapterList.Destroy();
		mChapterList = null;
		mChapterListController = null;
	}

	public void Update()
	{
		if (SceneBehaviourUpdate())
		{
			return;
		}
		mLayout.Update();
		if (mLayout != null)
		{
			mChapterList.Update();
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				GoToMenu("MenuWith3DLevel");
			}
		}
	}

	private void SetInitialSelection()
	{
		int waveToBeat = Singleton<Profile>.instance.waveToBeat;
		int num = 0;
		string[] allChapterIDs = Singleton<ChaptersDatabase>.instance.allChapterIDs;
		foreach (string chapterID in allChapterIDs)
		{
			int[] wavesRange = Singleton<ChaptersDatabase>.instance.GetWavesRange(chapterID);
			if (waveToBeat >= wavesRange[0] && waveToBeat <= wavesRange[1])
			{
				break;
			}
			num++;
		}
		if (num < Singleton<ChaptersDatabase>.instance.allChapterIDs.Length)
		{
			mChapterList.selection = num;
		}
	}

	private void GoToMenu(string newScene)
	{
		mLayout.onTransitionOver = delegate
		{
			Singleton<MenusFlow>.instance.LoadScene(newScene);
		};
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void RedrawSlots()
	{
		int[] array = waveRangeOnDisplay;
		int num = 0;
		int num2 = array[0];
		while (num < 10)
		{
			if (num2 <= array[1])
			{
				int waveLevel = Singleton<Profile>.instance.GetWaveLevel(num2);
				mSlotRefs[num].visible = true;
				mSlotRefs[num].locked = waveLevel == 0;
				mSlotRefs[num].title.text = SUILayoutConv.GetFormattedText(string.Format(Singleton<Localizer>.instance.Get("chapter_wave_num"), num2));
				if (waveLevel > 0)
				{
					mSlotRefs[num].level.text = waveLevel.ToString();
				}
			}
			else
			{
				mSlotRefs[num].visible = false;
			}
			num++;
			num2++;
		}
		RedrawSlotSelection();
	}

	private void RedrawSlotSelection()
	{
		int[] array = waveRangeOnDisplay;
		mSlotSelector.visible = false;
		int num = 0;
		int num2 = array[0];
		while (num < 10)
		{
			if (num2 <= array[1])
			{
				Color color;
				if (num2 == Singleton<Profile>.instance.waveToBeat)
				{
					color = kSelectedColor;
					mSlotSelector.visible = true;
					mSlotSelector.position = mSlotRefs[num].slot.position + new Vector2(1f, 1f);
				}
				else
				{
					color = kNormalColor;
				}
				if (mSlotRefs[num].title.fontColor != color)
				{
					mSlotRefs[num].title.fontColor = color;
					mSlotRefs[num].level.fontColor = color;
				}
			}
			num++;
			num2++;
		}
	}

	private void OnChapterSelectionChanged(int index)
	{
		RedrawSlots();
	}

	private void OnSlotTouched(int slotIndex)
	{
		int num = slotIndex + waveRangeOnDisplay[0];
		if (Singleton<Profile>.instance.GetWaveLevel(num) != 0)
		{
			Singleton<Profile>.instance.waveToBeat = num;
			RedrawSlotSelection();
		}
	}
}
