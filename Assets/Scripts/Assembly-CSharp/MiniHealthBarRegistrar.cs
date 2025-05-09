using System.Collections.Generic;

public class MiniHealthBarRegistrar : WeakGlobalInstance<MiniHealthBarRegistrar>
{
	private const int kMaxBarsAtATime = 6;

	private List<MiniHealthBar> mMiniHealthBarRefs = new List<MiniHealthBar>(30);

	private List<MiniHealthBar> mWorkingList = new List<MiniHealthBar>(30);

	public MiniHealthBarRegistrar()
	{
		SetUniqueInstance(this);
	}

	public void Register(MiniHealthBar mhb)
	{
		mMiniHealthBarRefs.Add(mhb);
	}

	public void Unregister(MiniHealthBar mhb)
	{
		mMiniHealthBarRefs.Remove(mhb);
	}

	public void UpdateCulling()
	{
		try
		{
			foreach (MiniHealthBar mMiniHealthBarRef in mMiniHealthBarRefs)
			{
				if (mMiniHealthBarRef.visible)
				{
					mWorkingList.Add(mMiniHealthBarRef);
				}
			}
			if (mWorkingList.Count <= 6)
			{
				return;
			}
			mWorkingList.Sort(CompareImportance);
			mWorkingList.RemoveRange(0, 6);
			foreach (MiniHealthBar mWorking in mWorkingList)
			{
				mWorking.visible = false;
			}
		}
		finally
		{
			mWorkingList.Clear();
		}
	}

	private static int CompareImportance(MiniHealthBar o1, MiniHealthBar o2)
	{
		float num = o2.importance - o1.importance;
		if (num > 0f)
		{
			return 1;
		}
		if (num < 0f)
		{
			return -1;
		}
		return 0;
	}
}
