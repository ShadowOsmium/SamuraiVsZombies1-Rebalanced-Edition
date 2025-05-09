using System.Collections.Generic;
using UnityEngine;

public class SUIInputManager
{
	public class TouchInfo
	{
		private Vector2 kInvalidPos = new Vector2(-1f, -1f);

		public int _id = -1;

		public bool _flag;

		public Vector2 previousPosition;

		public Vector2 currentPosition;

		public bool justTouched
		{
			get
			{
				return currentPosition != kInvalidPos && previousPosition == kInvalidPos;
			}
		}

		public bool justReleased
		{
			get
			{
				return currentPosition == kInvalidPos && previousPosition != kInvalidPos;
			}
		}

		public bool isTouching
		{
			get
			{
				return currentPosition != kInvalidPos;
			}
		}

		public Vector2 position
		{
			get
			{
				if (currentPosition != kInvalidPos)
				{
					return currentPosition;
				}
				return previousPosition;
			}
		}

		public TouchInfo(int uniqueID)
		{
			_id = uniqueID;
			previousPosition = kInvalidPos;
			currentPosition = kInvalidPos;
		}
	}

	private Vector2 kInvalidPos = new Vector2(-1f, -1f);

	private List<TouchInfo> mTouches = new List<TouchInfo>();

	private bool mProcessInputs = true;

	public bool justTouched
	{
		get
		{
			if (mTouches.Count > 0)
			{
				return mTouches[0].justTouched;
			}
			return false;
		}
	}

	public bool justReleased
	{
		get
		{
			if (mTouches.Count > 0)
			{
				return mTouches[0].justReleased;
			}
			return false;
		}
	}

	public bool isTouching
	{
		get
		{
			if (mTouches.Count > 0)
			{
				return mTouches[0].isTouching;
			}
			return false;
		}
	}

	public Vector2 position
	{
		get
		{
			if (mTouches.Count > 0)
			{
				return mTouches[0].position;
			}
			return kInvalidPos;
		}
	}

	public bool processInputs
	{
		get
		{
			return mProcessInputs;
		}
		set
		{
			if (mProcessInputs != value)
			{
				mProcessInputs = value;
				if (!value)
				{
					mTouches = new List<TouchInfo>();
				}
			}
		}
	}

	public int numTouches
	{
		get
		{
			return mTouches.Count;
		}
	}

	public TouchInfo this[int index]
	{
		get
		{
			if (index >= 0 && index < mTouches.Count)
			{
				return mTouches[index];
			}
			return null;
		}
	}

	public void Update()
	{
		if (!mProcessInputs)
		{
			return;
		}
		foreach (TouchInfo mTouch in mTouches)
		{
			mTouch._flag = true;
		}
		for (int i = 0; i < PCInput.touchCount; i++)
		{
			TouchInfo touchInfo = FindIndexFromCode(PCInput.touches[i].fingerId);
			if (touchInfo == null)
			{
				touchInfo = new TouchInfo(PCInput.touches[i].fingerId);
				mTouches.Add(touchInfo);
			}
			touchInfo._flag = false;
			touchInfo.previousPosition = touchInfo.currentPosition;
			touchInfo.currentPosition = SUIUtils.touchToUser(PCInput.touches[i].position);
		}
		for (int num = mTouches.Count - 1; num >= 0; num--)
		{
			if (mTouches[num]._flag)
			{
				if (mTouches[num].currentPosition == kInvalidPos)
				{
					mTouches.RemoveAt(num);
				}
				else
				{
					mTouches[num].previousPosition = mTouches[num].currentPosition;
					mTouches[num].currentPosition = kInvalidPos;
				}
			}
		}
	}

	private TouchInfo FindIndexFromCode(int code)
	{
		foreach (TouchInfo mTouch in mTouches)
		{
			if (mTouch._id == code)
			{
				return mTouch;
			}
		}
		return null;
	}
}
