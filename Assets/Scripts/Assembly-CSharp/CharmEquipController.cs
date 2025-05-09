using System.Collections.Generic;
using UnityEngine;

public class CharmEquipController : SUIScrollList.IController
{
	public class CharmCell : ListCells.IconNameCounter
	{
		public CharmCell(SUISprite cursor)
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
		public string id = string.Empty;

		public string displayName = string.Empty;

		public string iconFile = string.Empty;

		public string desc = string.Empty;
	}

	private const float kCellPriority = 1f;

	private const float kCursorPriority = 0.5f;

	private const int kTitleMaxWidth = 156;

	private const float kSelectedCellAlpha = 0.4f;

	private List<Data> mData = new List<Data>();

	private IconGlowWidget mListCursor;

	public CharmEquipController(Rect listArea, Vector2 cellSize)
	{
		mListCursor = new IconGlowWidget("Sprites/Icons/charm_glow", 0.95f, 1.1f);
		mListCursor.priority = 0.5f;
		mListCursor.visible = false;
		string[] allIDsForActivePlayMode = Singleton<CharmsDatabase>.instance.allIDsForActivePlayMode;
		foreach (string text in allIDsForActivePlayMode)
		{
			string attribute = Singleton<CharmsDatabase>.instance.GetAttribute(text, "eventID");
			if (!(attribute != string.Empty) || Singleton<EventsManager>.instance.IsEventActive(attribute) || Singleton<Profile>.instance.GetNumCharms(text) != 0)
			{
				Data item = new Data
				{
					id = text,
					displayName = Singleton<CharmsDatabase>.instance.GetAttribute(text, "displayName"),
					iconFile = Singleton<CharmsDatabase>.instance.GetAttribute(text, "icon"),
					desc = Singleton<CharmsDatabase>.instance.GetAttribute(text, "storeDesc")
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

	public int FindIndex(string charmID)
	{
		int num = 0;
		foreach (Data mDatum in mData)
		{
			if (mDatum.id == charmID)
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
		CharmCell charmCell = new CharmCell(mListCursor);
		charmCell.priority = 1f;
		charmCell.title.maxWidth = 156;
		return charmCell;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		CharmCell charmCell = (CharmCell)c;
		if (isSelected)
		{
			charmCell.cursorRef.visible = true;
		}
		else if (charmCell.isSelected)
		{
			charmCell.cursorRef.visible = false;
		}
		charmCell.isSelected = isSelected;
		int numCharms = Singleton<Profile>.instance.GetNumCharms(mData[dataIndex].id);
		charmCell.icon.texture = mData[dataIndex].iconFile;
		charmCell.title.text = mData[dataIndex].displayName;
		charmCell.counterText = "x" + numCharms;
		if (numCharms == 0)
		{
			charmCell.alpha = 0.4f;
		}
		else
		{
			charmCell.alpha = 1f;
		}
		charmCell.CheckIfRecommended(mData[dataIndex].id);
	}
}
