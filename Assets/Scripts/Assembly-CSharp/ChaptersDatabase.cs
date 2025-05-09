using System.Collections.Generic;

public class ChaptersDatabase : Singleton<ChaptersDatabase>
{
	private SDFTreeNode mChapters;

	private string[] mCachedChaptersIDs;

	private Dictionary<string, int[]> mCachedWaveRanges;

	public string[] allChapterIDs
	{
		get
		{
			return mCachedChaptersIDs;
		}
	}

	public ChaptersDatabase()
	{
		Init();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ResetCachedData;
	}

	public void ResetCachedData()
	{
		Init();
	}

	public string GetAttribute(string chapterID, string attribute)
	{
		SDFTreeNode sDFTreeNode = mChapters.to(chapterID);
		if (sDFTreeNode == null)
		{
			return string.Empty;
		}
		return sDFTreeNode[attribute];
	}

	public int[] GetWavesRange(string chapterID)
	{
		if (mCachedWaveRanges.ContainsKey(chapterID))
		{
			return mCachedWaveRanges[chapterID];
		}
		int[] waveRange = GetWaveRange(chapterID);
		mCachedWaveRanges.Add(chapterID, waveRange);
		return waveRange;
	}

	private void Init()
	{
		mCachedWaveRanges = new Dictionary<string, int[]>();
		mChapters = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Chapters");
		SDFTreeNode sDFTreeNode = mChapters.to("all");
		mCachedChaptersIDs = new string[sDFTreeNode.attributeCount];
		for (int i = 0; i < sDFTreeNode.attributeCount; i++)
		{
			if (sDFTreeNode.hasAttribute(i))
			{
				mCachedChaptersIDs[i] = sDFTreeNode[i];
			}
		}
	}

	private int[] GetWaveRange(string chapterID)
	{
		int[] array = new int[2];
		SDFTreeNode sDFTreeNode = mChapters.to(chapterID);
		if (sDFTreeNode != null)
		{
			string[] array2 = sDFTreeNode["wavesRange"].Split(',');
			if (array2.Length == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					if (!int.TryParse(array2[i], out array[i]))
					{
						array[0] = 0;
						array[1] = 0;
						break;
					}
				}
			}
		}
		return array;
	}
}
