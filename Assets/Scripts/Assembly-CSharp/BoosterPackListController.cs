using UnityEngine;

public class BoosterPackListController : SUIScrollList.IController
{
	private const float kCellPriority = 1f;

	private const float kCursorPriority = 0.5f;

	private const int kTitleMaxWidth = 156;

	private readonly Vector2 kSelectedScale = new Vector2(1.05f, 1.05f);

	private readonly Vector2 kNormalScale = new Vector2(1f, 1f);

	private IconGlowWidget mListCursor;

	public BoosterPackListController(Rect listArea, Vector2 cellSize)
	{
		mListCursor = new IconGlowWidget("Sprites/BoosterPacks/booster_pack_glow", 1f, 1.1f);
		mListCursor.priority = 0.5f;
		mListCursor.visible = false;
	}

	public void Update()
	{
	}

	public void Destroy()
	{
		mListCursor.Destroy();
		mListCursor = null;
	}

	public int ScrollList_NumEntries()
	{
		return Singleton<BoosterPackCodex>.instance.numPacks;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		BoosterListCells.Booster booster = new BoosterListCells.Booster(mListCursor);
		booster.priority = 1f;
		return booster;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		BoosterListCells.Booster booster = (BoosterListCells.Booster)c;
		if (isSelected)
		{
			booster.cursorRef.visible = true;
			booster.scale = kSelectedScale;
		}
		else if (booster.isSelected)
		{
			booster.cursorRef.visible = false;
			booster.scale = kNormalScale;
		}
		booster.isSelected = isSelected;
		booster.Render(dataIndex);
	}
}
