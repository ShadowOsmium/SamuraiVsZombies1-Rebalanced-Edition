using System.Collections.Generic;

public class NarrativeManager
{
	private Queue<string> mQueue = new Queue<string>();

	private NarrativeDialog mDialog;

	public bool isBlocking
	{
		get
		{
			return mQueue.Count > 0 || mDialog != null;
		}
	}

	public NarrativeManager()
	{
		if (Singleton<Profile>.instance.GetWaveLevel(Singleton<Profile>.instance.waveToBeat) > 1)
		{
			return;
		}
		SDFTreeNode sDFTreeNode = SDFTree.LoadFromResources("Registry/Narratives");
		if (sDFTreeNode == null)
		{
			return;
		}
		sDFTreeNode = sDFTreeNode.to(Singleton<PlayModesManager>.instance.selectedMode);
		if (sDFTreeNode == null)
		{
			return;
		}
		string text = "wave_" + Singleton<Profile>.instance.waveToBeat;
		if (sDFTreeNode.hasChild(text))
		{
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(text);
			for (int i = 0; i < sDFTreeNode2.attributeCount; i++)
			{
				mQueue.Enqueue(sDFTreeNode2[i]);
			}
		}
	}

	public void Update()
	{
		if (mDialog != null)
		{
			mDialog.Update();
			if (mDialog.isDone)
			{
				mDialog.Destroy();
				mDialog = null;
			}
		}
		else if (mQueue.Count > 0)
		{
			mDialog = new NarrativeDialog(mQueue.Dequeue());
		}
	}
}
