using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class PlayModesManager : Singleton<PlayModesManager>
{
	public enum GameDirection
	{
		LeftToRight = 0,
		RightToLeft = 1
	}

	private string mModeID;

	private SDFTreeNode mData;

	private SDFTreeNode mSelectedModeData;

	private List<KeyValuePair<string, string>> mPathSubstitutions = new List<KeyValuePair<string, string>>();

	private GameDirection mGameDirection;

	public string selectedMode
	{
		get
		{
			return mModeID;
		}
		set
		{
			if (!(mModeID == value))
			{
				mModeID = value;
				ReloadData();
				Singleton<Profile>.instance.PostLoadSyncing();
				SingletonMonoBehaviour<ResourcesManager>.instance.BroadcastCacheReload();
			}
		}
	}

	public SDFTreeNode selectedModeData
	{
		get
		{
			return mSelectedModeData;
		}
	}

	public SDFChildEnumerator allModes
	{
		get
		{
			return mData.childs;
		}
	}

	public string revivePotionID
	{
		get
		{
			return mSelectedModeData["revivePotionID"];
		}
	}

	public GameDirection gameDirection
	{
		get
		{
			return mGameDirection;
		}
	}

	public PlayModesManager()
	{
		mModeID = "classic";
		ReloadData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ReloadData;
	}

	public SDFTreeNode GetModeData(string id)
	{
		return mData.to(id);
	}

	public string ApplyPathSubstitutions(string path)
	{
		foreach (KeyValuePair<string, string> mPathSubstitution in mPathSubstitutions)
		{
			if (path.Contains(mPathSubstitution.Key))
			{
				return path.Replace(mPathSubstitution.Key, mPathSubstitution.Value);
			}
		}
		return path;
	}

	public bool CheckFlag(string flagID)
	{
		string text = mSelectedModeData[flagID];
		if (text == null || text == string.Empty)
		{
			return false;
		}
		return SUILayoutConv.GetBool(text);
	}

	private void ReloadData()
	{
		mData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/PlayModes", false);
		mSelectedModeData = mData.to(mModeID);
		if (mSelectedModeData == null)
		{
			Debug.Log("WARNING: Unknown play mode: " + mModeID);
			mSelectedModeData = new SDFTreeNode();
		}
		InitPathSubstitutions();
		DetermineGameDirection();
	}

	private void InitPathSubstitutions()
	{
		mPathSubstitutions.Clear();
		if (!mSelectedModeData.hasChild("pathSubstitutions"))
		{
			return;
		}
		foreach (KeyValuePair<string, string> attribute in mSelectedModeData.to("pathSubstitutions").attributes)
		{
			string[] array = attribute.Value.Split(':');
			if (array.Length != 2)
			{
				Debug.Log("WARNING: wrong path substitution: " + attribute.Value);
				continue;
			}
			KeyValuePair<string, string> item = new KeyValuePair<string, string>(array[0].Trim(), array[1].Trim());
			mPathSubstitutions.Add(item);
		}
	}

	private void DetermineGameDirection()
	{
		mGameDirection = GameDirection.LeftToRight;
		string strA = mSelectedModeData["gameDirection"];
		if (string.Compare(strA, "RightToLeft", true) == 0)
		{
			mGameDirection = GameDirection.RightToLeft;
		}
	}
}
