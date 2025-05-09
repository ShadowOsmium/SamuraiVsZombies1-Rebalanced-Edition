using System.Collections.Generic;
using UnityEngine;

public class SUISlideStripList : SUIProcess
{
	public class Cell : SUIProcess
	{
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

		public virtual float alpha
		{
			set
			{
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
	}

	public interface IController
	{
		int SlideStripList_NumEntries();

		Cell SlideStripList_CreateCell(int dataIndex);
	}

	public OnSUIIntCallback onIconTouched;

	private IController mController;

	private Vector2 mCellsSize;

	private Vector2 mOrigin;

	private Rect mTouchArea;

	private bool mAllowInputs = true;

	private FingerScroller mFingerScroller = new FingerScroller();

	private List<Cell> mCells = new List<Cell>();

	private int mIconSelected;

	public bool allowInputs
	{
		get
		{
			return mAllowInputs;
		}
		set
		{
			mAllowInputs = value;
		}
	}

	public int selected
	{
		get
		{
			return mIconSelected;
		}
	}

	public Rect touchArea
	{
		get
		{
			return mFingerScroller.area;
		}
	}

	public SUISlideStripList(IController ctrl, Vector2 centerPos, Vector2 cellsSize)
	{
		mController = ctrl;
		mCellsSize = cellsSize;
		mOrigin = new Vector2(centerPos.x - cellsSize.x / 2f, centerPos.y - cellsSize.y / 2f);
		mTouchArea = new Rect(mOrigin.x, mOrigin.y, cellsSize.x, cellsSize.y);
		BuildCells();
		mFingerScroller.area = mTouchArea;
		mFingerScroller.scrollMax = new Vector2((float)(mCells.Count - 1) * cellsSize.x, 0f);
		mFingerScroller.snapToGrid = (int)cellsSize.x;
		mFingerScroller.onSimpleTouch = OnSimpleTouch;
		UpdateVisuals();
	}

	public void Update()
	{
		foreach (Cell mCell in mCells)
		{
			mCell.Update();
		}
		if (mAllowInputs)
		{
			mFingerScroller.Update();
		}
		UpdateVisuals();
	}

	public void Destroy()
	{
		if (mCells == null)
		{
			return;
		}
		foreach (Cell mCell in mCells)
		{
			mCell.Destroy();
		}
		mCells = null;
	}

	public void EditorRenderOnGUI()
	{
	}

	private void BuildCells()
	{
		int num = mController.SlideStripList_NumEntries();
		for (int i = 0; i < num; i++)
		{
			mCells.Add(mController.SlideStripList_CreateCell(i));
		}
	}

	private void UpdateVisuals()
	{
		Vector2 position = new Vector2(mOrigin.x - mFingerScroller.visualScrollPosition.x, mOrigin.y);
		int num = 0;
		foreach (Cell mCell in mCells)
		{
			float num2 = Mathf.Abs(position.x - mOrigin.x);
			if (num2 >= mCellsSize.x)
			{
				mCell.visible = false;
			}
			else
			{
				mCell.visible = true;
				mCell.position = position;
				float num4 = (mCell.alpha = Mathf.Clamp((mCellsSize.x - num2) / mCellsSize.x, 0f, 1f));
				if (num4 >= 0.5f)
				{
					mIconSelected = num;
				}
			}
			position.x += mCellsSize.x;
			num++;
		}
	}

	private void OnSimpleTouch(Vector2 pos)
	{
		if (onIconTouched != null)
		{
			onIconTouched(mIconSelected);
		}
	}
}
