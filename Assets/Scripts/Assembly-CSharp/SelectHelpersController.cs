using System.Collections.Generic;
using UnityEngine;

public class SelectHelpersController : SUIScrollList.IController
{
	public class HelperCell : ListCells.IconNameCounter
	{
		public HelperCell(SUISprite cursor)
			: base("default18", cursor, true)
		{
			Vector2 vector = new Vector2(0f, 10f);
			iconOffset += vector;
			labelOffset += vector;
			counterOffset += vector;
			cursorOffset = iconOffset;
		}
	}

	private class Data
	{
		public string id;

		public string displayName;

		public string iconFile;

		public string desc;
	}

	private const float kCellPriority = 1f;

	private const float kCursorPriority = 0.5f;

	private const int kTitleMaxWidth = 156;

	private List<Data> mData = new List<Data>();

	private IconGlowWidget mListCursor;

	public SelectHelpersController(Rect listArea, Vector2 cellSize)
	{
		mListCursor = new IconGlowWidget();
		mListCursor.priority = 0.5f;
		mListCursor.visible = false;
		string[] allIDs = Singleton<HelpersDatabase>.instance.allIDs;
		foreach (string text in allIDs)
		{
			if (Singleton<Profile>.instance.GetHelperLevel(text) > 0)
			{
				Data item = new Data
				{
					id = text,
					displayName = Singleton<HelpersDatabase>.instance.GetAttribute(text, "displayName"),
					iconFile = Singleton<HelpersDatabase>.instance.GetAttribute(text, "HUDIcon"),
					desc = Singleton<HelpersDatabase>.instance.GetAttribute(text, "desc")
				};
				mData.Add(item);
			}
			else if (Debug.isDebugBuild)
			{
				Debug.Log(text + ": " + Singleton<Profile>.instance.GetHelperLevel(text));
			}
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

	public int FindIndex(string helperID)
	{
		if (helperID == string.Empty)
		{
			return -1;
		}
		int num = 0;
		foreach (Data mDatum in mData)
		{
			if (string.Compare(mDatum.id, helperID, true) == 0)
			{
				return num;
			}
			num++;
		}
		Debug.Log("WARNING: Could not find HelperID: " + helperID);
		return -1;
	}

	public string FindID(int index)
	{
		try
		{
			return mData[index].id;
		}
		catch
		{
			return string.Empty;
		}
	}

	public string GetIcon(int index)
	{
		try
		{
			return mData[index].iconFile;
		}
		catch
		{
			return string.Empty;
		}
	}

	public string GetDescription(int index)
	{
		try
		{
			return mData[index].desc;
		}
		catch
		{
			return string.Empty;
		}
	}

	public int ScrollList_NumEntries()
	{
		return mData.Count;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		HelperCell helperCell = new HelperCell(mListCursor);
		helperCell.priority = 1f;
		helperCell.title.maxWidth = 156;
		return helperCell;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		HelperCell helperCell = (HelperCell)c;
		if (isSelected)
		{
			helperCell.cursorRef.visible = true;
		}
		else if (helperCell.isSelected)
		{
			helperCell.cursorRef.visible = false;
		}
		helperCell.isSelected = isSelected;
		helperCell.icon.texture = mData[dataIndex].iconFile;
		helperCell.title.text = mData[dataIndex].displayName;
		helperCell.CheckIfRecommended(mData[dataIndex].id);
	}
}
