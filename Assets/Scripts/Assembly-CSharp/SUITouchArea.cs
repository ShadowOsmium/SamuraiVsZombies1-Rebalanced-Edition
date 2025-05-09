using UnityEngine;

public class SUITouchArea : SUIProcess
{
	public OnSUIGenericCallback onAreaTouched;

	private Rect mArea;

	private bool mTriggerEveryUpdates;

	private bool mIsReverse;

	public bool triggerEveryUpdates
	{
		get
		{
			return mTriggerEveryUpdates;
		}
		set
		{
			mTriggerEveryUpdates = value;
		}
	}

	public Rect area
	{
		get
		{
			return mArea;
		}
		set
		{
			mArea = value;
		}
	}

	public bool reverse
	{
		get
		{
			return mIsReverse;
		}
		set
		{
			mIsReverse = value;
		}
	}

	public SUITouchArea(Rect area)
	{
		mArea = area;
	}

	public void Update()
	{
		for (int i = 0; i < WeakGlobalInstance<SUIScreen>.instance.inputs.numTouches; i++)
		{
			SUIInputManager.TouchInfo touchInfo = WeakGlobalInstance<SUIScreen>.instance.inputs[i];
			if (((!mTriggerEveryUpdates && touchInfo.justTouched) || (mTriggerEveryUpdates && touchInfo.isTouching)) && IsTouching(touchInfo.position) && onAreaTouched != null)
			{
				onAreaTouched();
			}
		}
	}

	public void Destroy()
	{
	}

	public void EditorRenderOnGUI()
	{
	}

	private bool IsTouching(Vector2 pos)
	{
		if (mIsReverse)
		{
			return !mArea.Contains(pos);
		}
		return mArea.Contains(pos);
	}
}
