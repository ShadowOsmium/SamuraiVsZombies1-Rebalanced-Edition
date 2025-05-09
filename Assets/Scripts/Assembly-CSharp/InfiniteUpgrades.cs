using System;
using Debug = UnityEngine.Debug;

public class InfiniteUpgrades
{
	public static T Extrapolate<T>(SDFTreeNode root, string upgradeFactorAttribute, string attributeName, int level)
	{
		if (level < 1)
		{
			Debug.Log("WARNING: No level data found.");
			return default(T);
		}
		if (root.hasChild(level))
		{
			return Parse<T>(root.to(level)[attributeName]);
		}
		int i;
		for (i = 0; root.hasChild(i + 1); i++)
		{
		}
		if (i >= level)
		{
			Debug.Log("WARNING: Malformed level data encountered.");
			return default(T);
		}
		float num = Parse<float>(root[upgradeFactorAttribute]) * (float)(level - i);
		float num2 = Parse<float>(root.to(i)[attributeName]);
		num2 += num * num2;
		return (T)Convert.ChangeType(num2, typeof(T));
	}

	public static T SnapToHighest<T>(SDFTreeNode root, string attributeName, int level)
	{
		if (level < 1)
		{
			Debug.Log("WARNING: No level data found.");
			return default(T);
		}
		if (root.hasChild(level))
		{
			return Parse<T>(root.to(level)[attributeName]);
		}
		int i;
		for (i = 0; root.hasChild(i + 1); i++)
		{
		}
		return Parse<T>(root.to(i)[attributeName]);
	}

	private static T Parse<T>(string valStr)
	{
		try
		{
			return (T)Convert.ChangeType(valStr, typeof(T));
		}
		catch
		{
			return default(T);
		}
	}
}
