using System.Collections.Generic;
using UnityEngine;

public class StoreListController : SUIScrollList.IController
{
	private List<StoreData.Item> mItems;

	public List<StoreData.Item> items
	{
		get
		{
			return mItems;
		}
	}

	public List<StoreData.Item> data
	{
		get
		{
			return mItems;
		}
	}

	public StoreListController(Rect listArea, Vector2 cellSize, List<StoreData.Item> items)
	{
		ReloadDataSource(items);
	}

	public void Update()
	{
	}

	public void Destroy()
	{
	}

	public void ReloadDataSource(List<StoreData.Item> items)
	{
		mItems = items;
	}

	public int ScrollList_NumEntries()
	{
		return mItems.Count;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		return new StoreListCell();
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		StoreListCell storeListCell = (StoreListCell)c;
		StoreData.Item itemData = mItems[dataIndex];
		storeListCell.DrawContent(itemData);
	}
}
