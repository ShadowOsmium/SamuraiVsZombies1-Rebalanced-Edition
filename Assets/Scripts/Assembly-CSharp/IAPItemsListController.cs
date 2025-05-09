using System.Collections.Generic;
using UnityEngine;

public class IAPItemsListController : SUIScrollList.IController
{
	private PurchaseCurrencyDialog mDialogHandle;

	private OnlineItemsManager.Item[] mItems;

	private List<IAPItemCell> cells;

	public OnlineItemsManager.Item[] items
	{
		get
		{
			return mItems;
		}
	}

	public OnlineItemsManager.Item[] data
	{
		get
		{
			return mItems;
		}
	}

	public IAPItemsListController(Rect listArea, Vector2 cellSize, OnlineItemsManager.Item[] items, PurchaseCurrencyDialog DialogHandle)
	{
		mDialogHandle = DialogHandle;
		cells = new List<IAPItemCell>();
		ReloadDataSource(items);
	}

	public void Destroy()
	{
	}

	public void Update()
	{
	}

	public void ReloadDataSource(OnlineItemsManager.Item[] items)
	{
		mItems = items;
	}

	public int ScrollList_NumEntries()
	{
		return mItems.Length;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		IAPItemCell iAPItemCell = new IAPItemCell(mDialogHandle);
		cells.Add(iAPItemCell);
		return iAPItemCell;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		IAPItemCell iAPItemCell = (IAPItemCell)c;
		OnlineItemsManager.Item itemData = mItems[dataIndex];
		iAPItemCell.DrawContent(itemData, dataIndex);
	}

	public void HideAllCells()
	{
		foreach (IAPItemCell cell in cells)
		{
			if (cell != null)
			{
				cell.visible = false;
			}
		}
	}
}
