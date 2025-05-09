using System.Collections.Generic;
using UnityEngine;

public class BuffIconManager : WeakGlobalInstance<BuffIconManager>
{
	private Dictionary<string, GameObject> mCachedPrefabs = new Dictionary<string, GameObject>();

	public BuffIconManager()
	{
		SetUniqueInstance(this);
	}

	public GameObject GetPrefab(string iconFile)
	{
		if (!mCachedPrefabs.ContainsKey(iconFile))
		{
			mCachedPrefabs.Add(iconFile, Resources.Load(iconFile) as GameObject);
		}
		return mCachedPrefabs[iconFile];
	}
}
