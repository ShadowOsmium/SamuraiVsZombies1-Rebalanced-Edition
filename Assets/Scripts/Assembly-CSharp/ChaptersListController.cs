using System.Collections.Generic;
using UnityEngine;

public class ChaptersListController : SUIScrollList.IController
{
	public class ChapterCell : ListCells.IconNameCounter
	{
		public ChapterCell(SUISprite cursor)
			: base("default18", cursor, false)
		{
			base.extraOffset = new Vector2(14f, 20f);
			title.fontColor = new Color(1f, 1f, 1f);
		}
	}

	private class Data
	{
		public string id;

		public string displayName;

		public string iconFile;
	}

	private const float kCellPriority = 1f;

	private const float kCursorPriority = 0.5f;

	private Color kCursorColor = new Color(0f, 0.2f, 0.8f, 0.3f);

	private List<Data> mData = new List<Data>();

	private SUISprite mListCursor;

	public ChaptersListController(Rect listArea, Vector2 cellSize)
	{
		mListCursor = new SUISprite(kCursorColor, 8, 8);
		mListCursor.autoscaleKeepAspectRatio = false;
		mListCursor.priority = 0.5f;
		mListCursor.visible = false;
		mListCursor.scale = new Vector2(cellSize.x / 8f, cellSize.y / 8f);
		mListCursor.hotspot = new Vector2(0f, 0f);
		string[] allChapterIDs = Singleton<ChaptersDatabase>.instance.allChapterIDs;
		foreach (string text in allChapterIDs)
		{
			Data item = new Data
			{
				id = text,
				displayName = Singleton<ChaptersDatabase>.instance.GetAttribute(text, "displayName"),
				iconFile = Singleton<ChaptersDatabase>.instance.GetAttribute(text, "icon")
			};
			mData.Add(item);
		}
	}

	public void Update()
	{
	}

	public void Destroy()
	{
		mListCursor.Destroy();
		mListCursor = null;
	}

	public int FindIndex(string chapterID)
	{
		int num = 0;
		foreach (Data mDatum in mData)
		{
			if (mDatum.id == chapterID)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public string FindID(int index)
	{
		return mData[index].id;
	}

	public string GetIcon(int index)
	{
		return mData[index].iconFile;
	}

	public int ScrollList_NumEntries()
	{
		return mData.Count;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		ChapterCell chapterCell = new ChapterCell(mListCursor);
		chapterCell.priority = 1f;
		chapterCell.iconOffset = new Vector2(104f, chapterCell.iconOffset.y - 12f);
		chapterCell.labelOffset = new Vector2(104f, chapterCell.labelOffset.y - 12f);
		return chapterCell;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		ChapterCell chapterCell = (ChapterCell)c;
		if (isSelected)
		{
			chapterCell.cursorRef.visible = true;
		}
		else if (chapterCell.isSelected)
		{
			chapterCell.cursorRef.visible = false;
		}
		chapterCell.isSelected = isSelected;
		chapterCell.icon.texture = mData[dataIndex].iconFile;
		chapterCell.title.text = Singleton<Localizer>.instance.Parse(mData[dataIndex].displayName);
		chapterCell.title.visible = true;
	}
}
