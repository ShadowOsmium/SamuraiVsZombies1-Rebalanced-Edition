using System.Collections.Generic;
using UnityEngine;

public class AbilityEquipController : SUIScrollList.IController
{
	public class AbilityCell : ListCells.IconNameCounter
	{
		public AbilityCell(SUISprite cursor)
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

	public AbilityEquipController(Rect listArea, Vector2 cellSize)
	{
		mListCursor = new IconGlowWidget();
		mListCursor.priority = 0.5f;
		mListCursor.visible = false;
		string[] allIDs = Singleton<AbilitiesDatabase>.instance.allIDs;
		foreach (string text in allIDs)
		{
			if (Singleton<Profile>.instance.GetAbilityLevel(text) > 0)
			{
				Data item = new Data
				{
					id = text,
					displayName = Singleton<AbilitiesDatabase>.instance.GetAttribute(text, "displayName"),
					iconFile = Singleton<AbilitiesDatabase>.instance.GetAttribute(text, "icon"),
					desc = Singleton<AbilitiesDatabase>.instance.GetAttribute(text, "desc")
				};
				mData.Add(item);
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

	public int FindIndex(string abilityID)
	{
		int num = 0;
		foreach (Data mDatum in mData)
		{
			if (mDatum.id == abilityID)
			{
				return num;
			}
			num++;
		}
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
		AbilityCell abilityCell = new AbilityCell(mListCursor);
		abilityCell.priority = 1f;
		abilityCell.title.maxWidth = 156;
		return abilityCell;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		AbilityCell abilityCell = (AbilityCell)c;
		if (isSelected)
		{
			abilityCell.cursorRef.visible = true;
		}
		else if (abilityCell.isSelected)
		{
			abilityCell.cursorRef.visible = false;
		}
		abilityCell.isSelected = isSelected;
		abilityCell.icon.texture = mData[dataIndex].iconFile;
		abilityCell.title.text = mData[dataIndex].displayName;
		abilityCell.CheckIfRecommended(mData[dataIndex].id);
	}
}
