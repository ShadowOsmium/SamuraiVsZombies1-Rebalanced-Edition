using System.Collections.Generic;
using UnityEngine;

public class SUIScrollList : SUIProcess
{
	public enum ScrollDirection
	{
		Vertical = 0,
		Horizontal = 1
	}

	public abstract class Cell : SUIProcess
	{
		protected float mAlpha = 1f;

		protected float mTransitionAlpha = 1f;

		public virtual Vector2 position
		{
			set
			{
			}
		}

		public virtual bool visible
		{
			set
			{
			}
		}

		public float alpha
		{
			get
			{
				return mAlpha;
			}
			set
			{
				mAlpha = Mathf.Clamp(value, 0f, 1f);
				ApplyCombinedAlpha();
			}
		}

		public float transitionAlpha
		{
			get
			{
				return mTransitionAlpha;
			}
			set
			{
				mTransitionAlpha = Mathf.Clamp(value, 0f, 1f);
				ApplyCombinedAlpha();
			}
		}

		public virtual void Update()
		{
		}

		public virtual void Destroy()
		{
		}

		public void EditorRenderOnGUI()
		{
		}

		protected abstract void ApplyCombinedAlpha();
	}

	private class CellEntry
	{
		public int dataIndex = -1;

		public bool isVisible;

		public Cell cell;

		public CellEntry(Cell c, int i)
		{
			dataIndex = i;
			cell = c;
		}
	}

	public interface IController
	{
		void Destroy();

		void Update();

		int ScrollList_NumEntries();

		Cell ScrollList_CreateCell();

		void ScrollList_DrawCellContent(Cell c, int dataIndex, bool isSelected);
	}

	public OnSUIIntCallback onSelectionChanged;

	public OnSUIIntCallback onItemTouched;

	private GameObject mObjectForSounds = new GameObject();

	private ScrollDirection mScrollDirType;

	private IController mController;

	private Rect mArea;

	private Vector2 mCellsSize;

	private int mNumColsOrRows = 1;

	private FingerScroller mFingerScroller;

	private int mSelectedDataIndex = -1;

	private int mNumDataEntries;

	private int mNumDataLines;

	private int mFirstCellShown = -1;

	private float mFirstCellXorY;

	private float mAlpha = 1f;

	private float mIgnoreTouchesTimer;

	private List<CellEntry> mCells = new List<CellEntry>();

	public int selection
	{
		get
		{
			return mSelectedDataIndex;
		}
		set
		{
			Select(value);
		}
	}

	public Vector2 scrollPosition
	{
		get
		{
			return mFingerScroller.scrollPosition;
		}
		set
		{
			mFingerScroller.scrollPosition = value;
		}
	}

	public float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			if (mAlpha == value)
			{
				return;
			}
			mAlpha = value;
			foreach (CellEntry mCell in mCells)
			{
				if (mCell.isVisible)
				{
					mCell.cell.transitionAlpha = value;
				}
			}
		}
	}

	public SUIScrollList(IController ctrl, Rect area, Vector2 cellsSize, ScrollDirection dir, int numColsOrRows)
	{
		mController = ctrl;
		mArea = area;
		mCellsSize = cellsSize;
		mScrollDirType = dir;
		mNumColsOrRows = numColsOrRows;
		mFingerScroller = new FingerScroller();
		mFingerScroller.area = area;
		mFingerScroller.onSimpleTouch = OnSimpleTouch;
		ForceRedrawList();
	}

	public void Update()
	{
		if (mIgnoreTouchesTimer > 0f)
		{
			mIgnoreTouchesTimer = Mathf.Max(0f, mIgnoreTouchesTimer - Time.deltaTime);
			mFingerScroller.touchesEnabled = false;
		}
		else
		{
			mFingerScroller.touchesEnabled = true;
		}
		if (mController != null)
		{
			mController.Update();
		}
		mFingerScroller.Update();
		UpdateList(mFingerScroller.visualScrollPosition);
		foreach (CellEntry mCell in mCells)
		{
			mCell.cell.Update();
		}
	}

	public void Destroy()
	{
		if (mCells == null)
		{
			return;
		}
		foreach (CellEntry mCell in mCells)
		{
			mCell.cell.Destroy();
		}
		mCells = null;
		mController.Destroy();
		mController = null;
	}

	public void ForceRedrawList()
	{
		mNumDataEntries = mController.ScrollList_NumEntries();
		mNumDataLines = Mathf.CeilToInt((float)mNumDataEntries / (float)mNumColsOrRows);
		if (mSelectedDataIndex >= mNumDataEntries)
		{
			mSelectedDataIndex = mNumDataEntries - 1;
		}
		CreateCells();
		if (mScrollDirType == ScrollDirection.Vertical)
		{
			mFingerScroller.scrollMax = new Vector2(0f, (float)mNumDataLines * mCellsSize.y - mArea.height);
		}
		else
		{
			mFingerScroller.scrollMax = new Vector2((float)mNumDataLines * mCellsSize.x - mArea.width, 0f);
		}
		mFirstCellShown = -1;
		UpdateList(mFingerScroller.visualScrollPosition);
	}

	public void TransitInFromBelow()
	{
		if (mScrollDirType == ScrollDirection.Vertical)
		{
			scrollPosition = new Vector2(0f, 0f - mArea.height * 4f);
		}
		else
		{
			scrollPosition = new Vector2(0f - mArea.width * 4f, 0f);
		}
		mIgnoreTouchesTimer = 1f;
	}

	public void EditorRenderOnGUI()
	{
	}

	private void CreateCells()
	{
		int num = ((mScrollDirType != 0) ? Mathf.Min(mNumDataEntries, (Mathf.CeilToInt(mArea.width / mCellsSize.x) + 1) * mNumColsOrRows) : Mathf.Min(mNumDataEntries, (Mathf.CeilToInt(mArea.height / mCellsSize.y) + 1) * mNumColsOrRows));
		if (num > mCells.Count)
		{
			for (int i = mCells.Count; i < num; i++)
			{
				mCells.Add(new CellEntry(mController.ScrollList_CreateCell(), -1));
			}
		}
		else if (num < mCells.Count)
		{
			int num2 = mCells.Count - num;
			for (int j = 0; j < num2; j++)
			{
				mCells[mCells.Count - 1].cell.Destroy();
				mCells.RemoveAt(mCells.Count - 1);
			}
		}
		foreach (CellEntry mCell in mCells)
		{
			mCell.dataIndex = -1;
		}
	}

	private void UpdateList(Vector2 topVec)
	{
		int num = 0;
		num = ((mScrollDirType != 0) ? Mathf.Clamp(Mathf.FloorToInt(topVec.x / mCellsSize.x), 0, mNumDataLines - 1) : Mathf.Clamp(Mathf.FloorToInt(topVec.y / mCellsSize.y), 0, mNumDataLines - 1));
		int num2 = Mathf.Clamp(num * mNumColsOrRows, 0, mNumDataEntries - 1);
		if (mScrollDirType == ScrollDirection.Vertical)
		{
			mFirstCellXorY = 0f - (topVec.y - mCellsSize.y * (float)num);
		}
		else
		{
			mFirstCellXorY = 0f - (topVec.x - mCellsSize.x * (float)num);
		}
		if (num2 != mFirstCellShown)
		{
			ChangeShowingCells(num2);
		}
		if (mScrollDirType == ScrollDirection.Vertical)
		{
			float num3 = mFirstCellXorY;
			int num4 = 0;
			{
				foreach (CellEntry mCell in mCells)
				{
					mCell.cell.position = new Vector2(mArea.x + mCellsSize.x * (float)num4, mArea.y + num3);
					num4++;
					if (num4 >= mNumColsOrRows)
					{
						num4 = 0;
						num3 += mCellsSize.y;
					}
				}
				return;
			}
		}
		float num5 = mFirstCellXorY;
		int num6 = 0;
		foreach (CellEntry mCell2 in mCells)
		{
			mCell2.cell.position = new Vector2(mArea.x + num5, mArea.y + mCellsSize.y * (float)num6);
			num6++;
			if (num6 >= mNumColsOrRows)
			{
				num6 = 0;
				num5 += mCellsSize.x;
			}
		}
	}

	private void ChangeShowingCells(int firstCell)
	{
		int lastCell = Mathf.Min(firstCell + mCells.Count, mNumDataEntries - 1);
		ChangeShowingCells(firstCell, lastCell);
	}

	private void ChangeShowingCells(int firstCell, int lastCell)
	{
		if (mCells.Count == 0)
		{
			return;
		}
		if (mFirstCellShown >= 0)
		{
			while (firstCell != mFirstCellShown)
			{
				if (firstCell < mFirstCellShown)
				{
					mCells.Insert(0, mCells[mCells.Count - 1]);
					mCells.RemoveAt(mCells.Count - 1);
					mFirstCellShown--;
				}
				else
				{
					mCells.Add(mCells[0]);
					mCells.RemoveAt(0);
					mFirstCellShown++;
				}
			}
		}
		else
		{
			mFirstCellShown = firstCell;
		}
		int num = mFirstCellShown;
		bool flag = false;
		foreach (CellEntry mCell in mCells)
		{
			if (flag)
			{
				mCell.cell.visible = false;
				mCell.isVisible = false;
				continue;
			}
			RedrawCell(mCell, num);
			num++;
			if (num > lastCell)
			{
				flag = true;
			}
		}
	}

	private void RedrawCellCursorOnly(CellEntry e, int dataIndex)
	{
		bool isVisible = e.isVisible;
		mController.ScrollList_DrawCellContent(e.cell, dataIndex, dataIndex == mSelectedDataIndex);
		e.cell.visible = isVisible;
	}

	private void RedrawCell(CellEntry e, int dataIndex)
	{
		e.cell.visible = true;
		e.isVisible = true;
		if (e.dataIndex != dataIndex)
		{
			ForceRedrawCell(e, dataIndex);
		}
	}

	private void ForceRedrawCell(CellEntry e, int dataIndex)
	{
		e.cell.visible = true;
		e.isVisible = true;
		mController.ScrollList_DrawCellContent(e.cell, dataIndex, dataIndex == mSelectedDataIndex);
		e.cell.visible = true;
		e.isVisible = true;
		e.dataIndex = dataIndex;
	}

	private void OnSimpleTouch(Vector2 pos)
	{
		Rect rect = ((mScrollDirType != 0) ? new Rect(mFirstCellXorY + mArea.xMin, mArea.yMin, mCellsSize.x, mCellsSize.y) : new Rect(mArea.xMin, mFirstCellXorY + mArea.yMin, mCellsSize.x, mCellsSize.y));
		CellEntry cellEntry = null;
		int num = 0;
		int num2 = 0;
		foreach (CellEntry mCell in mCells)
		{
			Rect rect2 = rect;
			if (mScrollDirType == ScrollDirection.Vertical)
			{
				rect2.xMin += mCellsSize.x * (float)num2;
				rect2.xMax += mCellsSize.x * (float)num2;
				rect2.yMin += mCellsSize.y * (float)num;
				rect2.yMax += mCellsSize.y * (float)num;
			}
			else
			{
				rect2.xMin += mCellsSize.x * (float)num;
				rect2.xMax += mCellsSize.x * (float)num;
				rect2.yMin += mCellsSize.y * (float)num2;
				rect2.yMax += mCellsSize.y * (float)num2;
			}
			if (rect2.Contains(pos))
			{
				cellEntry = mCell;
				break;
			}
			num2++;
			if (num2 >= mNumColsOrRows)
			{
				num2 = 0;
				num++;
			}
		}
		if (cellEntry != null && cellEntry.isVisible)
		{
			Select(cellEntry.dataIndex, true);
			if (onItemTouched != null)
			{
				onItemTouched(cellEntry.dataIndex);
			}
		}
	}

	private void Select(int index)
	{
		Select(index, false);
	}

	private void Select(int index, bool playSoundIfChanged)
	{
		index = Mathf.Clamp(index, -1, mNumDataEntries - 1);
		if (index == mSelectedDataIndex)
		{
			return;
		}
		if (playSoundIfChanged)
		{
			Singleton<SUISoundManager>.instance.Play("scrollListSelection", mObjectForSounds);
		}
		int num = mSelectedDataIndex;
		mSelectedDataIndex = index;
		CellEntry cellEntry = null;
		foreach (CellEntry mCell in mCells)
		{
			if (mCell.dataIndex == num)
			{
				RedrawCellCursorOnly(mCell, mCell.dataIndex);
			}
			else if (mCell.dataIndex == mSelectedDataIndex)
			{
				cellEntry = mCell;
			}
		}
		if (cellEntry != null)
		{
			RedrawCellCursorOnly(cellEntry, index);
		}
		if (onSelectionChanged != null)
		{
			onSelectionChanged(mSelectedDataIndex);
		}
	}
}
